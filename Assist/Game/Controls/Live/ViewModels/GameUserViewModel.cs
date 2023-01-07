using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Models;
using Assist.Game.Services;
using Assist.Objects.Helpers;
using Assist.ViewModels;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Utilities;
using ReactiveUI;
using Serilog;
using ValNet.Objects.Local;
using ValNet.Objects.Pregame;

namespace Assist.Game.Controls.Live.ViewModels
{
    internal class GameUserViewModel : ViewModelBase
    {
        private IBrush? _brush = null;

        public IBrush PlayerBrush
        {
            get => _brush;
            set => this.RaiseAndSetIfChanged(ref _brush, value);
        }


        private PregameMatch.Player? _player;
        public PregameMatch.Player? Player { get => _player; set => this.RaiseAndSetIfChanged(ref _player, value); }

        private string? _playerRankIcon = null;

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


        private IBrush? _globalDodgeBorder;

        public IBrush? GlobalDodgeBorder
        {
            get => _globalDodgeBorder;
            set => this.RaiseAndSetIfChanged(ref _globalDodgeBorder, value);
        }


        public async Task Setup()
        {

        }

        /// <summary>
        /// Updates the binded user control data with the PlayerData set in control's Player variable.
        /// </summary>
        /// <returns></returns>
        public async Task UpdatePlayerData()
        {
            if (Player != null)
            {

                // Get player name from Presence.
                if (string.Equals(PlayerName, "Player") || string.IsNullOrEmpty(PlayerRankIcon))
                {
                    var pres = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
                    var data = pres.presences.Find(user => user.puuid == Player.Subject);

                    if (!Player.PlayerIdentity.Incognito) // If Incognito is True, then streamer mode is enabled.
                        if (data != null)
                            PlayerName = data.game_name;

                    if (data != null)
                    {
                        var t = await GetPresenceData(data);
                        // Set rank icon
                        PlayerRankIcon = $"https://content.assistapp.dev/ranks/TX_CompetitiveTier_Large_{t.competitiveTier}.png";
                    }
                }
                


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
                var checkGlobal =
                    AssistApplication.Current.AssistUser.GlobalDodgeUsers.Find(player => player.id == Player.Subject);
                if (user != null)
                {
                    // This means the user was found on the dodge list.
                    IsPlayerDodge = true;
                    Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        DodgeBorder = new SolidColorBrush(new Color(255, 246, 30, 81));
                    });

                }

                if (checkGlobal != null)
                {
                    // This means the user was found on the global dodge list.
                    IsPlayerDodge = true;
                    PlayerRankRating = checkGlobal.category.ToUpper();
                    Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        DodgeBorder = new SolidColorBrush(new Color(255, 246, 30, 81));
                        GlobalDodgeBorder = new SolidColorBrush(new Color(255, 255, 255, 255));
                    });

                }

            }
        }

        private async Task<PlayerPresence> GetPresenceData(ChatV4PresenceObj.Presence data)
        {
            if (string.IsNullOrEmpty(data.Private))
                return new PlayerPresence();
            byte[] stringData = Convert.FromBase64String(data.Private);
            string decodedString = Encoding.UTF8.GetString(stringData);
            return JsonSerializer.Deserialize<PlayerPresence>(decodedString);
        }


    }
}
