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

    [ObservableProperty] List<Product> products;

    // ссылка на главное окно (для показа диалогов)
    public Window? HostWindow { get; set; }

    public MainViewModel()
    {
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
            // Проверка 1: пустой логин
            if (string.IsNullOrWhiteSpace(Login))
            {
                await MessageBox.Warning("Пустой логин",
                    "Поле логина не может быть пустым. Введите ваш логин и повторите попытку.");
                return;
            }

            // Проверка 2: логин слишком короткий
            if (Login.Length < 3)
            {
                await MessageBox.Warning("Некорректный логин",
                    "Логин должен содержать минимум 3 символа. Проверьте ввод.");
                return;
            }

            // Ищем пользователя
            CurrentUser = db.Users
                .Include(x => x.IdRoleNavigation)
                .FirstOrDefault(x => x.Login == Login);

            // Проверка 3: пользователь не найден
            if (CurrentUser == null)
            {
                await MessageBox.Error("Пользователь не найден",
                    $"Пользователь с логином «{Login}» в системе не найден.\n" +
                    "Проверьте правильность ввода логина или обратитесь к администратору.");
                return;
            }

            // Загружаем товары
            Products = db.Products
                .Include(x => x.IdCategoryNavigation)
                .Include(x => x.IdManufactureNavigation)
                .Include(x => x.Stocks)
                .ToList();

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
}
