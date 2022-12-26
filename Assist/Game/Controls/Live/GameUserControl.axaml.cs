using System;
using System.Threading.Tasks;
using Assist.Game.Controls.Live.ViewModels;
using Avalonia.Controls;
using ValNet.Objects.Pregame;

namespace Assist.Game.Controls.Live
{
    public partial class GameUserControl : UserControl
    {
        private readonly GameUserViewModel _viewModel;
        public string? PlayerId = null;
        public GameUserControl()
        {
            DataContext = _viewModel = new GameUserViewModel();
            InitializeComponent();
        }

        public GameUserControl(PregameMatch.Player player)
        {
            DataContext = _viewModel = new GameUserViewModel();
            _viewModel.Player = player;
            PlayerId = player.Subject;
            InitializeComponent();
        }


        public async Task UpdatePlayer(PregameMatch.Player player)
        {
            _viewModel.Player = player;

            await _viewModel.UpdatePlayerData();
        }

        private async void GameUser_Init(object? sender, EventArgs e)
        {
            await _viewModel.UpdatePlayerData();
        }
    }
}
