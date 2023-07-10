using System;
using Assist.Services;
using Assist.Views.Store.ViewModels;
using Avalonia;
using Avalonia.Controls;
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
        _viewModel.GetPlayerWallet();
    }
}