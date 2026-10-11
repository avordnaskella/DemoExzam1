using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using demka1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace demka1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    static PostgresContext db = new PostgresContext();

    [ObservableProperty]
    UserControl page;

    [ObservableProperty]
    string pageTitle;

    // введённый логин
    [ObservableProperty]
    string login = string.Empty;

    // найденный пользователь (для отображения ФИО)
    [ObservableProperty]
    User? currentUser;

    [ObservableProperty]
    List<Product> products;

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
    private void Login2()  // имя команды Login не может совпасть со свойством
    {
        if (string.IsNullOrWhiteSpace(Login))
            return;

        // ищем пользователя по логину
        CurrentUser = db.Users
            .Include(x => x.IdRoleNavigation)
            .FirstOrDefault(x => x.Login == Login);

        // если пользователь не найден — ничего не делаем
        if (CurrentUser == null)
            return;

        // загружаем товары
        Products = db.Products
            .Include(x => x.IdCategoryNavigation)
            .Include(x => x.IdManufactureNavigation)
            .Include(x => x.Stocks)
            .ToList();

        GoToProducts();
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
