using System;
using Assist.ViewModels.Game;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Game.Live.Pages;

public partial class IngamePageView : UserControl
{
    private readonly IngamePageViewModel _viewModel;

    public IngamePageView()
    {
        DataContext = _viewModel = new IngamePageViewModel();
        InitializeComponent();
    }

    private async void IngamePage_Init(object? sender, EventArgs e)
    {
        if(Design.IsDesignMode)
            return;

        await _viewModel.PageSetup();
    }

    private void IngamePage_Unloaded(object? sender, RoutedEventArgs e)
    {
        if(Design.IsDesignMode)
            return;
        _viewModel.UnsubscribeFromEvents();
    }
}