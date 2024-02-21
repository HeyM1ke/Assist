using System;
using Assist.Services;
using Assist.Views.Store.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Store.Pages;

public partial class PlayerBonusMarketView : UserControl
{
    private readonly StoreViewModel _viewModel;

    public PlayerBonusMarketView()
    {
        StoreViewNavigationController.CurrentPage = StorePage.NIGHTMARKET;
        InitializeComponent();
        DataContext = _viewModel = new StoreViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void BonusMarket_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode)
        {
            return;
        }
        await _viewModel.NightMarketSetup();
    }
}