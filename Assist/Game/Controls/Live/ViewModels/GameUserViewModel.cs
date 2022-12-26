using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.ViewModels;
using Avalonia.Utilities;
using ReactiveUI;
using ValNet.Objects.Pregame;

namespace Assist.Game.Controls.Live.ViewModels
{
    internal class GameUserViewModel : ViewModelBase
    {
        private PregameMatch.Player _player = new PregameMatch.Player();
        public PregameMatch.Player Player { get => _player; set => this.RaiseAndSetIfChanged(ref _player, value); }


        public async Task Setup()
        {

        }

        public async Task UpdatePlayerData()
        {
            
        }
    }
}
