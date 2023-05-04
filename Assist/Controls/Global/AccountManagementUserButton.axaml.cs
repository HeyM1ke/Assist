using Assist.Controls.Global.ViewModels;
using Assist.Settings;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Global;

public partial class AccountManagementUserButton : UserControl
{
    private readonly AccountManagementUserButtonViewModel _viewModel;

    public AccountManagementUserButton()
    {
        InitializeComponent();
    }
    
    public AccountManagementUserButton(ProfileSettings profileSettings)
    {
        DataContext = _viewModel = new AccountManagementUserButtonViewModel();
        _viewModel.Profile = profileSettings;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void SetDefault_Btn(object? sender, RoutedEventArgs e)
    {
        await _viewModel.SetDefault();
    }

    private async void RemoveAccount_Btn(object? sender, RoutedEventArgs e)
    {
        await _viewModel.RemoveProfile();
        this.IsVisible = false;
    }

    private async void Button_PointerPressed(object? sender, RoutedEventArgs e)
    {
        await _viewModel.SwapProfile();
    }
}