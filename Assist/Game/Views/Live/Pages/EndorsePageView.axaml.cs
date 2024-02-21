using System;
using Assist.Game.Services;
using Assist.Game.Views.Live.Pages.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Assist.Game.Views.Live.Pages;

public partial class EndorsePageView : UserControl
{
    private readonly EndorsePageViewModel _viewModel;

    public EndorsePageView()
    {
        DataContext = _viewModel = new EndorsePageViewModel();
        GameViewNavigationController.CurrentPage = Page.ENDORSE;
        InitializeComponent();
    }
    
    private async void EndorsePage_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode)return;
        
        await _viewModel.Setup();
    }

    private void BackToParty_Click(object? sender, RoutedEventArgs e)
    {
        GameViewNavigationController.Change(new LiveView());
    }
}