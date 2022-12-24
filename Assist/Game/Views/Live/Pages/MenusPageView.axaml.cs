using System;
using Assist.Game.Models;
using Assist.Game.Services;
using Assist.Game.Views.Live.Pages.ViewModels;
using Assist.Objects.RiotSocket;
using Avalonia.Controls;

namespace Assist.Game.Views.Live.Pages
{
    public partial class MenusPageView : UserControl
    {
        private readonly MenusPageViewModel _viewModel;
        private readonly PresenceV4Message initalMessage;
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

        private async void MenuPage_Init(object? sender, EventArgs e)
        {
            _viewModel.Setup(initalMessage!);
        }
    }
}
