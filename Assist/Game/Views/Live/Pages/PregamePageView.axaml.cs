using System;
using Assist.Game.Services;
using Assist.Game.Views.Live.Pages.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

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

        private void PregamePageControl_OnUnloaded(object? sender, RoutedEventArgs e)
        {
            _viewModel.UnsubscribeFromEvents();
        }
    }
}
