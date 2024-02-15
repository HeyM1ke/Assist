using Assist.Controls.Infobars;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Extras;

public partial class VLRLaunchedPage : UserControl
{
    public VLRLaunchedPage()
    {
        InitializeComponent();
    }

    private void VLRLaunchPage_Loaded(object? sender, RoutedEventArgs e)
    {
        Titlebar.ViewModel.SettingsEnabled = false;
        Titlebar.ViewModel.AccountSwapEnabled = false;
    }

    private void VLRLaunchPage_Unloaded(object? sender, RoutedEventArgs e)
    {
    }
}