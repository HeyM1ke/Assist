using System;
using Assist.Game.Services;
using Assist.Game.Views.Live.Pages.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Serilog;

namespace Assist.Game.Views.Live.Pages
{
    public partial class IngamePageView : UserControl
    {
        private readonly IngameViewModel _viewModel;

        public IngamePageView()
        {
            LiveViewNavigationController.CurrentPage = LivePage.INGAME;
            DataContext = _viewModel = new IngameViewModel();
            InitializeComponent();
        }


        private async void IngamePage_Init(object? sender, EventArgs e)
        {
            if (Design.IsDesignMode)
                return;

            await _viewModel.Setup();
        }

        private void Control_OnUnloaded(object? sender, RoutedEventArgs e)
        {
            Log.Error("Control Unloaded");
            _viewModel.UnsubscribeFromEvents();
            Log.Error("Events Unsubbed Unloaded");
        }

        private async void IngamePage_Loaded(object? sender, RoutedEventArgs e)
        {
            if (Design.IsDesignMode)
                return;

            await _viewModel.Setup();
        }
    }
}
