using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Game.Services;
using Assist.Objects.Helpers;
using Assist.ViewModels;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Utilities;
using ReactiveUI;
using Serilog;
using ValNet.Objects.Pregame;

namespace Assist.Game.Controls.Live.ViewModels
{
    internal class GameUserViewModel : ViewModelBase
    {
        private PregameMatch.Player? _player;
        public PregameMatch.Player? Player { get => _player; set => this.RaiseAndSetIfChanged(ref _player, value); }

        private string? _playerRankIcon;

        public string? PlayerRankIcon
        {
            get => _playerRankIcon;
            set => this.RaiseAndSetIfChanged(ref _playerRankIcon, value);
        }

        private string? _playerAgentIcon;

        public string? PlayerAgentIcon
        {
            get => _playerAgentIcon;
            set => this.RaiseAndSetIfChanged(ref _playerAgentIcon, value);
        }

        private string? _playerAgentName = "Selecting...";

        public string? PlayerAgentName
        {
            get => _playerAgentName;
            set => this.RaiseAndSetIfChanged(ref _playerAgentName, value);
        }

        private string? _playerName = "Player";

        public string? PlayerName
        {
            get => _playerName;
            set => this.RaiseAndSetIfChanged(ref _playerName, value);
        }

        private string? _playerRankRating = "";

        public string? PlayerRankRating
        {
            get => _playerRankRating;
            set => this.RaiseAndSetIfChanged(ref _playerRankRating, value);
        }

        private bool? _isPlayerDodge = false;

        public bool? IsPlayerDodge
        {
            get => _isPlayerDodge;
            set => this.RaiseAndSetIfChanged(ref _isPlayerDodge, value);
        }

        private IBrush? _dodgeBorder;

        public IBrush? DodgeBorder
        {
            get => _dodgeBorder;
            set => this.RaiseAndSetIfChanged(ref _dodgeBorder, value);
        }


        public async Task Setup()
        {

        }

        public async Task UpdatePlayerData()
        {
            if (Player != null)
            {
                // Get player name from name service.
                if (!Player.PlayerIdentity.Incognito && string.Equals(PlayerName, "Player")) // If Incognito is True, then streamer mode is enabled.
                {
                    var pres = await AssistApplication.Current.CurrentUser.Presence.GetPresences();

                    var data = pres.presences.Find(user => user.puuid == Player.Subject);

                    if (data != null)
                    {
                        PlayerName = data.game_name;
                    }
                }

                // Set rank icon
                PlayerRankIcon = $"https://content.assistapp.dev/ranks/TX_CompetitiveTier_Large_{Player.CompetitiveTier}.png";
                if (!string.IsNullOrEmpty(Player.CharacterID))
                {
                    try
                    {
                        // Set Agent Icon
                        PlayerAgentIcon = $"https://content.assistapp.dev/agents/{Player.CharacterID}_displayicon.png";
                        // Set Agent Name
                        PlayerAgentName = AgentNames.AgentIdToNames?[Player.CharacterID];
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex.Message);
                    }
                }

                // Check if user is on dodge list
                // if so enable red border and icon popup.
                var user = DodgeService.Current.UserList.Find(player => player.UserId == Player.Subject);

                if (user != null)
                {
                    // This means the user was found on the dodge list.
                    IsPlayerDodge = true;
                    Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        DodgeBorder = new SolidColorBrush(new Color(255, 246, 30, 81));
                    });

                }

            }
        }

        
    }
}
