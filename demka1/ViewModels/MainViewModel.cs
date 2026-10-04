using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace demka1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    UserControl page = new Page1();
    
    public void Loading()
    {
        Page = new Page1();
    }

    public void GetOrder()
    {
        Page = new Page2();
    }


}
