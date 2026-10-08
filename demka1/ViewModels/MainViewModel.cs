using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using demka1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace demka1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    UserControl page = new Page1();
    
    static PostgresContext db= new PostgresContext();
    public void Loading()
    {
        Page = new Page1();
    }

    public void GetOrder()
    {
        Page = new Page2();
    }

    [ObservableProperty] 
    List<Product> products = db.Products
        .Include(x => x.IdCategoryNavigation)
        .Include(x => x.IdManufactureNavigation)
        .Include(x => x.Stocks)
        .ToList();
}
