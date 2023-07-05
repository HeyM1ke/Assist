using System;
using System.Diagnostics;
using Assist.Game.Controls.Navigation;
using Assist.Game.Views.Modules;
using Assist.Services;
using Assist.Services.Popup;
using Assist.ViewModels;
using Assist.Views.Dashboard;
using Assist.Views.Settings;
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

    private void LauncherVertNavBar_Init(object? sender, EventArgs e)
    {
        if(Design.IsDesignMode)
            return;
        var t = this.FindControl<AccountManagementNavBtn>("AccountsBtn");

        t.PlayercardImage = $"https://cdn.assistapp.dev/PlayerCards/{AssistApplication.Current.CurrentProfile.PlayerCardId}_DisplayIcon.png";
    }

    private void SettingsBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        PopupSystem.SpawnCustomPopup(new SettingsPopup());
    }

    private void SocialBtn_Click(object? sender, RoutedEventArgs e)
    {
        var btn = sender as SocialButton;
        
        if (btn.LinkTo == null)
            return;

        Process.Start(new ProcessStartInfo
        {
            FileName = btn.LinkTo,
            UseShellExecute = true
        });
    }
}