using System;
using Assist.Game.Views.Initial.ViewModels;
using Avalonia.Controls;

namespace Assist.Game.Views.Initial
{
    public partial class GameInitialView : UserControl
    {
        private readonly GameInitialViewViewModel _viewModel;
        public GameInitialView()
        {
            DataContext = _viewModel = new GameInitialViewViewModel();
            InitializeComponent();
        }

        private async void GameInitialView_Init(object? sender, EventArgs e)
        {
            await _viewModel.Setup();
        }
    }
}
