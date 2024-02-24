using Assist.Services.Navigation;
using Assist.ViewModels.Navigation;
using Assist.ViewModels.Settings;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Settings;

public partial class SettingsView : UserControl
{
    private readonly SettingsViewModel _viewModel;

    public SettingsView()
    {
        DataContext = _viewModel = new SettingsViewModel();
        InitializeComponent();
    }

    private async void SettingsView_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)
            return;
        _viewModel.SwitchToGeneral();
    }

    private void SettingsView_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)
            return;

        _viewModel.SetUnknown();
    }
}