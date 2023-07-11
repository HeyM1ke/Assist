using System;
using Assist.Game.Models;
using Assist.Game.Services;
using Assist.Game.Views.Live.Pages.ViewModels;
using Assist.Objects.RiotSocket;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ValNet.Objects.Local;

namespace Assist.Game.Views.Live.Pages
{
    public partial class MenusPageView : UserControl
    {
        private readonly MenusPageViewModel _viewModel;
        private readonly PresenceV4Message initalMessage;
        private readonly ChatV4PresenceObj.Presence? _initalMessage = null;
        public MenusPageView()
        {
            DataContext = _viewModel = new MenusPageViewModel();
            LiveViewNavigationController.CurrentPage = LivePage.MENUS;
            InitializeComponent();
        }

        public MenusPageView(PresenceV4Message playerPresence)
        {
            DataContext = _viewModel = new MenusPageViewModel();
            LiveViewNavigationController.CurrentPage = LivePage.MENUS;
            InitializeComponent();
            this.initalMessage = playerPresence;
        }
        
        public MenusPageView(ChatV4PresenceObj.Presence playerPresence)
        {
            DataContext = _viewModel = new MenusPageViewModel();
            LiveViewNavigationController.CurrentPage = LivePage.MENUS;
            InitializeComponent();
            this._initalMessage = playerPresence;
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
}
