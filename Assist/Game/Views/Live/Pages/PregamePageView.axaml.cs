using System;
using Assist.Game.Services;
using Assist.Game.Views.Live.Pages.ViewModels;
using Avalonia.Controls;

namespace Assist.Game.Views.Live.Pages
{
    public partial class PregamePageView : UserControl
    {
        private readonly PregamePageViewModel _viewModel;

        public PregamePageView()
        {
            LiveViewNavigationController.CurrentPage = LivePage.PREGAME;
            DataContext = _viewModel = new PregamePageViewModel();
            InitializeComponent();
        }

        private async void PreGamePage_Init(object? sender, EventArgs e)
        {
            if(Design.IsDesignMode)
                return;

            await _viewModel.Setup();
        }
    }
}
