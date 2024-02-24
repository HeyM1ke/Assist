using System;
using Assist.Models.Socket;
using Assist.ViewModels.Game;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ValNet.Objects.Local;

namespace Assist.Views.Game.Live.Pages;

public partial class MenusPageView : UserControl
{
    private readonly MenusPageViewModel _viewModel;
    private readonly PresenceV4Message initalMessage;
    private readonly ChatV4PresenceObj.Presence? _initalMessage = null;
    
    public MenusPageView()
    {
        DataContext = _viewModel = new MenusPageViewModel();
        InitializeComponent();
    }
    
    public MenusPageView(PresenceV4Message? message)
    {
        DataContext = _viewModel = new MenusPageViewModel();
        InitializeComponent();
        this.initalMessage = message;
    }
    
    public MenusPageView(ChatV4PresenceObj.Presence? message)
    {
        DataContext = _viewModel = new MenusPageViewModel();
        InitializeComponent();
        this._initalMessage = message;
    }
    
    private async void MenuPage_Init(object? sender, EventArgs e)
    {
        if (_initalMessage is not null)
        {
            _viewModel.SetupWithLocalPresence(_initalMessage);
            return;
        }
        _viewModel.Setup(initalMessage);
    }
    
    private void MenuPageControl_OnUnloaded(object? sender, RoutedEventArgs e)
    {
        _viewModel.UnsubscribeFromEvents();
    }
}