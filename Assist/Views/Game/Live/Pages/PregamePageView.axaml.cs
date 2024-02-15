using System;
using Assist.ViewModels.Game;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Game.Live.Pages;

public partial class PregamePageView : UserControl
{
    private readonly PregamePageViewModel _viewModel;

    public PregamePageView()
    {
        DataContext = _viewModel = new PregamePageViewModel();
        InitializeComponent();
    }

    private async void PregamePage_Init(object? sender, EventArgs e)
    {
        if(Design.IsDesignMode)
            return;

        await _viewModel.PageSetup();
    }

    private void PregamePage_Unloaded(object? sender, RoutedEventArgs e)
    {
        if(Design.IsDesignMode)
            return;
        _viewModel.UnsubscribeFromEvents();
    }
}