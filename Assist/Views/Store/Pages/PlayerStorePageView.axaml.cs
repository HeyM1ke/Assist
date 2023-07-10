using System;
using Assist.Views.Store.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Store.Pages;

public partial class PlayerStorePageView : UserControl
{
    private readonly StoreViewModel _viewModel;

    public PlayerStorePageView()
    {
        DataContext = _viewModel = new StoreViewModel();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void StorePage_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode) return;
        await _viewModel.Setup();
    }
}