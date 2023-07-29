using System;
using Assist.Game.Views.Initial.ViewModels;
using Assist.Objects.Helpers;
using Assist.ViewModels;
using Avalonia.Controls;

namespace Assist.Game.Views.Initial
{
    public partial class GameInitialView : UserControl
    {
        private readonly GameInitialViewViewModel _viewModel;
        public GameInitialView()
        {
            AssistApplication.Current.Mode = AssistMode.GAME;
            DataContext = _viewModel = new GameInitialViewViewModel();
            InitializeComponent();
        }

        private async void GameInitialView_Init(object? sender, EventArgs e)
        {
            await _viewModel.Setup();
        }
    }
}
