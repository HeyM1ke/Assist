using Assist.Game.Controls.Navigation;
using Assist.Game.Views.Modules;
using Assist.Services;
using Assist.Services.Popup;
using Assist.Views.Dashboard;
using Assist.Views.Store;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Global.Navigation;

public partial class LauncherVerticalNavigationBar : UserControl
{

    private StackPanel _buttonHolder;
    public LauncherVerticalNavigationBar()
    {
        
        InitializeComponent();
        _buttonHolder = this.FindControl<StackPanel>("ButtonHolder");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void AccountsBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        PopupSystem.SpawnCustomPopup(new AccountManagementPopup());
    }

    private void DashboardBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        ClearSelected();

        if (MainViewNavigationController.CurrentPage != Services.Page.DASHBOARD)
            MainViewNavigationController.Change(new DashboardView());

        (sender as NavButton).IsSelected = true;
    }
    
    private void StoreBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        ClearSelected();
        
        if (MainViewNavigationController.CurrentPage != Services.Page.STORE)
        MainViewNavigationController.Change(new StoreView());

        (sender as NavButton).IsSelected = true;
    }

    private void ModulesBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        ClearSelected();
        
        if (MainViewNavigationController.CurrentPage != Services.Page.MODULES)
            MainViewNavigationController.Change(new ModulesView());

        (sender as NavButton).IsSelected = true;
    }
    
    private void ClearSelected()
    {
        foreach (NavButton buttonHolderChild in _buttonHolder.Children)
        {
            buttonHolderChild.IsSelected = false;
        }
    }
}