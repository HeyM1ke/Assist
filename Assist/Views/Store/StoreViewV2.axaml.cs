using System;
using Assist.Services;
using Assist.Views.Store.Pages;
using Assist.Views.Store.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Store;

public partial class StoreViewV2 : UserControl
{
    private readonly StoreViewModel _viewModel;

    public StoreViewV2()
    {
        DataContext = _viewModel = new StoreViewModel();
        InitializeComponent();
        MainViewNavigationController.CurrentPage = Page.STORE;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void StoreView_Init(object? sender, EventArgs e)
    {
        StoreViewNavigationController.ContentControl =
            this.GetControl<TransitioningContentControl>("StoreContentControl");
        _viewModel.GetPlayerStore();
        _viewModel.GetPlayerWallet();
    }

    private async void NMNav_Click(object? sender, RoutedEventArgs e)
    {
        if (StoreViewNavigationController.CurrentPage != StorePage.NIGHTMARKET)
        {
            StoreViewNavigationController.Change(new PlayerBonusMarketView());    
        }
    }

    private void StoreNav_Click(object? sender, RoutedEventArgs e)
    {
        if (StoreViewNavigationController.CurrentPage != StorePage.STORE)
        {
            StoreViewNavigationController.Change(new PlayerStorePageView());    
        }
    }
}