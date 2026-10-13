using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using demka1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demka1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    static PostgresContext db = new PostgresContext();

    [ObservableProperty] UserControl page;

    [ObservableProperty] string pageTitle;

    // введённый логин
    [ObservableProperty] string login = string.Empty; //делаем не null чтобы не было проблем с привязкой

    // найденный пользователь 
    [ObservableProperty] User? currentUser;

    // исходный полный список
    private List<Product> AllProducts = new();
    // отфильтрованный список
    [ObservableProperty] List<Product> products = new();


    // для показа диалогов
    public Window? HostWindow { get; set; }

    [ObservableProperty] List<Category> categories;      // список категорий для combobox
    [ObservableProperty] Category? selectedCategory;     // выбранная категория
    [ObservableProperty] string searchText = string.Empty; // текст поиска
    [ObservableProperty] string selectedSort = "Без сортировки"; // выбранная сортировка

    // варианты сортировки
    public List<string> SortOptions { get; } = new()
    {
        "Без сортировки",
        "Цена: по возрастанию",
        "Цена: по убыванию"
    };

    public MainViewModel()
    {
        // загружаем категории один раз
        var cats = db.Categories.ToList();

        // все категории по умолчанию загружаются сначала
        cats.Insert(0, new Category { IdCategory = 0, Title = "Все категории" });

        Categories = cats;
        SelectedCategory = Categories[0]; 

        GoToLogin();
    }

    private void GoToLogin()
    {
        Login = string.Empty;   // чистим поле ввода
        CurrentUser = null;     // сбрасываем пользователя
        Page = new Page2();
        PageTitle = "Авторизация";
    }

    private void GoToProducts()
    {
        Page = new Page1();
        PageTitle = "Список товаров";
    }

    [RelayCommand]
    private async Task Login2()
    {
        try
        {
            // пустой логин
            if (string.IsNullOrWhiteSpace(Login))
            {
                await MessageBox.Warning("Пустой логин",
                    "Поле логина не может быть пустым. Введите ваш логин и повторите попытку.");
                return;
            }

            // логин слишком короткий
            if (Login.Length < 3)
            {
                await MessageBox.Warning("Некорректный логин",
                    "Логин должен содержать минимум 3 символа. Проверьте ввод.");
                return;
            }

            // ищем пользователя
            CurrentUser = db.Users
                .Include(x => x.IdRoleNavigation)
                .FirstOrDefault(x => x.Login == Login);

            // пользователь не найден
            if (CurrentUser == null)
            {
                await MessageBox.Error("Пользователь не найден",
                    $"Пользователь с логином «{Login}» в системе не найден.\n" +
                    "Проверьте правильность ввода логина или обратитесь к администратору.");
                return;
            }

            // загружаем товары
            AllProducts = db.Products
                .Include(x => x.IdCategoryNavigation)
                .Include(x => x.IdManufactureNavigation)
                .Include(x => x.Stocks)
                .ToList();

            ApplyFilters();
            GoToProducts();
        }
        catch (Exception ex)
        {
            await MessageBox.Error("Ошибка системы",
                $"Произошла непредвиденная ошибка: {ex.Message}\n" +
                "Попробуйте повторить действие позже или перезапустите приложение.");
        }
    }



    [RelayCommand]
    private void Logout()
    {
        GoToLogin();
    }

    [RelayCommand]
    private void Back()
    {
        GoToLogin();
    }

    // фильтрация, поиск и сортировка

    public void ApplyFilters()
    {
        // если товары ещё не загружены ничего не фильтруем
        if (AllProducts == null || AllProducts.Count == 0)
        {
            Products = new List<Product>();
            return;
        }

        // начинаем с полного списка
        var query = AllProducts.AsEnumerable();

        //  отбираем по категории
        if (SelectedCategory != null && SelectedCategory.IdCategory != 0)
        {
            query = query.Where(p => p.IdCategory == SelectedCategory.IdCategory);
        }

        //по наименованию и описанию
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var text = SearchText.Trim().ToLower();

            query = query.Where(p =>
                (p.Title != null && p.Title.ToLower().Contains(text)) ||
                (p.Description != null && p.Description.ToLower().Contains(text)));
        }

        // сортировка по цене
        if (SelectedSort == "Цена: по возрастанию")
        {
            query = query.OrderBy(p => p.Price);
        }
        else if (SelectedSort == "Цена: по убыванию")
        {
            query = query.OrderByDescending(p => p.Price);
        }

  
        Products = query.ToList();
    }

    // для автоматического изменения параметров
    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnSelectedCategoryChanged(Category? value) => ApplyFilters();
    partial void OnSelectedSortChanged(string value) => ApplyFilters();
}
