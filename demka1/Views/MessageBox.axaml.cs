using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Threading.Tasks;

namespace demka1;

public partial class MessageBox : Window
{
    public MessageBox()
    {
        InitializeComponent();
    }

    public MessageBox(string title, string message, string type) : this()
    {
        Title = title;
        MessageText.Text = message;

        switch (type)
        {
            case "error":
                IconText.Text = "😒";
                IconText.Foreground = Brushes.Red;
                break;
            case "warning":
                IconText.Text = "😊😊😊";
                IconText.Foreground = Brushes.Orange;
                break;
            default:
                IconText.Text = "💕";
                IconText.Foreground = Brushes.SteelBlue;
                break;
        }
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e) => Close();

    
    public static Task Error(string title, string message)
        => new MessageBox(title, message, "error").ShowDialog(GetOwner());

    public static Task Warning(string title, string message)
        => new MessageBox(title, message, "warning").ShowDialog(GetOwner());

    public static Task Info(string title, string message)
        => new MessageBox(title, message, "info").ShowDialog(GetOwner());

    private static Window GetOwner()
    {
        var desktop = Avalonia.Application.Current?.ApplicationLifetime
            as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
        return desktop?.MainWindow ?? throw new System.InvalidOperationException("Нет главного окна");
    }
}