using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using demka1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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


    // товар, выбранный для просмотра
    [ObservableProperty] Product? selectedProduct;

    // доступные размеры для выбранного товара, где count будет больше 0
    [ObservableProperty] List<Size> availableSizes;

    // выбранный размер
    [ObservableProperty] Size? selectedSize;

    // количество для добавления
    [ObservableProperty] string orderCount = "1";

    // список позиций текущего заказа 
      

    public ObservableCollection<CartItem> Cart { get; } = new();

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


    [RelayCommand]
    private void OpenProduct(Product product)
    {
        if (product == null) return;

        SelectedProduct = product;

        // загружаем доступные размеры: сначала Include, потом Where, потом Select
        AvailableSizes = db.Stocks
            .Include(s => s.IdSizeNavigation)             // Include до Where
            .Where(s => s.IdProduct == product.IdProduct && s.Count > 0)
            .Select(s => s.IdSizeNavigation)
            .ToList()                                   
            .GroupBy(s => s.IdSize)                       
            .Select(g => g.First())
            .ToList();

        // сколько размеров нашлось
        System.Diagnostics.Debug.WriteLine(
            $"[OpenProduct] Товар {product.IdProduct}, размеров: {AvailableSizes.Count}");

        // сброс
        SelectedSize = null;
        OrderCount = "1";

        Page = new Page3();
        PageTitle = "Просмотр товара";
    }


    [RelayCommand]
    private async Task AddToCart()
    {
        // проверки
        if (SelectedProduct == null) return;

        if (SelectedSize == null)
        {
            await MessageBox.Warning("Размер не выбран",
                "Выберите размер товара из списка перед добавлением в заказ.");
            return;
        }

        if (!int.TryParse(OrderCount, out int count) || count <= 0)
        {
            await MessageBox.Warning("Некорректное количество",
                "Введите целое положительное число в поле «Количество».");
            return;
        }

        // сколько доступно на складе для этого товара+размера
        int available = db.Stocks
            .Where(s => s.IdProduct == SelectedProduct.IdProduct && s.IdSize == SelectedSize.IdSize)
            .Sum(s => s.Count);

        if (count > available)
        {
            await MessageBox.Warning("Недостаточно товара",
                $"На складе доступно только {available} шт. Уменьшите количество.");
            return;
        }

        // проверяем, нет ли уже такого товара с таким размером в корзине
        var existing = Cart.FirstOrDefault(c =>
            c.Product.IdProduct == SelectedProduct.IdProduct &&
            c.Size.IdSize == SelectedSize.IdSize);

        if (existing != null)
        {
            // суммируем
            if (existing.Count + count > available)
            {
                await MessageBox.Warning("Недостаточно товара",
                    $"С учётом уже добавленного в заказ доступно только {available} шт.");
                return;
            }

            // убираем старый элемент и добавляем новый с увеличенным количеством
            
            Cart.Remove(existing);
            Cart.Add(new CartItem
            {
                Product = SelectedProduct,
                Size = SelectedSize,
                Count = existing.Count + count
            });
        }
        else
        {
            // просто добавляем новый
            Cart.Add(new CartItem
            {
                Product = SelectedProduct,
                Size = SelectedSize,
                Count = count
            });
        }

        await MessageBox.Info("Добавлено", "Товар добавлен в заказ.");
    }

    [RelayCommand]
    private async Task ConfirmOrder()
    {
        if (CurrentUser == null)
        {
            await MessageBox.Warning("Не выполнен вход",
                "Для оформления заказа необходимо авторизоваться.");
            return;
        }

        if (Cart.Count == 0)
        {
            await MessageBox.Warning("Пустой заказ",
                "Добавьте хотя бы одну позицию перед оформлением.");
            return;
        }

        try
        {
            // создаём заказ
            var order = new Order
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                IdUser = CurrentUser.Login
            };
            db.Orders.Add(order);
            db.SaveChanges(); // теперь у order есть IdOrder

            // добавляем позиции
            foreach (var item in Cart)
            {
                db.ProductsOrders.Add(new ProductsOrder
                {
                    IdOrder = order.IdOrder,
                    IdProduct = item.Product.IdProduct,
                    IdSize = item.Size.IdSize,
                    Count = item.Count
                });

                // уменьшаем остатки на складе
                var stock = db.Stocks
                    .FirstOrDefault(s => s.IdProduct == item.Product.IdProduct
                                      && s.IdSize == item.Size.IdSize);
                if (stock != null)
                {
                    stock.Count -= item.Count;
                    if (stock.Count < 0) stock.Count = 0; // защита
                }
            }

            db.SaveChanges();

            // очищаем корзину
            Cart.Clear();

            await MessageBox.Info("Заказ оформлен",
                $"Заказ №{order.IdOrder} успешно сохранён.");

            // обновляем список товаров — перечитываем из БД
            AllProducts = db.Products
                .Include(x => x.IdCategoryNavigation)
                .Include(x => x.IdManufactureNavigation)
                .Include(x => x.Stocks)
                .ToList();
            ApplyFilters();

            // возвращаемся к списку товаров
            GoToProducts();
        }
        catch (Exception ex)
        {
            await MessageBox.Error("Ошибка оформления",
                $"Не удалось сохранить заказ: {ex.Message}");
        }
    }
    [RelayCommand]
    private void CancelOrder()
    {
        Cart.Clear();
        GoToProducts();
    }

    [RelayCommand]
    private void BackToProducts()
    {
        GoToProducts();
    }

}
