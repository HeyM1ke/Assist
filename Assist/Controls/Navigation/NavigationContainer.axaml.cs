using Assist.ViewModels.Navigation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Navigation;

public partial class NavigationContainer : UserControl
{
    public static NavigationViewModel ViewModel;

    public NavigationContainer()
    {
        DataContext = ViewModel = new NavigationViewModel();
        InitializeComponent();
    }
}