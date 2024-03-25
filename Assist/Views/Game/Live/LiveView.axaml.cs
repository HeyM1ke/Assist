using System;
using Assist.Controls.Infobars;
using Assist.Models.Enums;
using Assist.Services.Navigation;
using Assist.ViewModels.Game;
using Assist.ViewModels.Navigation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Game.Live;

public partial class LiveView : UserControl
{
    private readonly LiveViewViewModel _viewModel;
    
    public LiveView()
    {
        if (!Design.IsDesignMode) { NavigationViewModel.SetActivePage(AssistPage.LIVE); }
        DataContext = _viewModel = new LiveViewViewModel();
        InitializeComponent();
    }

    private void LivePage_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode) return;
        _viewModel.Setup();
    }

    private void LivePage_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode) return;
        _viewModel.AttemptCurrentPage();
    }

    private void LivePage_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode) return;
        _viewModel.CurrentPage = ELivePage.UNKNOWN;
        _viewModel.Unsubscribe();
    }
}