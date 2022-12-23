using System;
using Assist.Game.Services;
using Assist.Game.Views.Live.ViewModels;
using Avalonia.Controls;

namespace Assist.Game.Views.Live
{
    public partial class LiveView : UserControl
    {
        private readonly LiveViewViewModel _viewModel;

        public LiveView()
        {
            GameViewNavigationController.CurrentPage = Page.LIVE;
            DataContext = _viewModel = new LiveViewViewModel();
            InitializeComponent();
            LiveViewNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("content");
        }

        private void LiveView_Init(object? sender, EventArgs e)
        {
            _viewModel.Setup();
        }
    }
}
