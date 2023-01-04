using System;
using System.Drawing;
using System.Threading.Tasks;
using Assist.Game.Controls.Live.ViewModels;
using Avalonia.Controls;
using Avalonia.Media;
using Serilog;
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

        public GameUserControl(PregameMatch.Player player, IBrush color)
        {
            DataContext = _viewModel = new GameUserViewModel();
            _viewModel.Player = player;
            
            if (_viewModel.PlayerBrush == null)
                _viewModel.PlayerBrush = color;
            
            PlayerId = player.Subject;
            InitializeComponent();
        }


        public async Task UpdatePlayer(PregameMatch.Player player, IBrush playerColor)
        {
            _viewModel.Player = player;

            if (_viewModel.PlayerBrush == null)
                _viewModel.PlayerBrush = playerColor;

            

            await _viewModel.UpdatePlayerData();
        }

        private async void GameUser_Init(object? sender, EventArgs e)
        {
            await _viewModel.UpdatePlayerData();
        }
    }
}
