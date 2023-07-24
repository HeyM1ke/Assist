using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Models;
using Assist.Game.Services;
using Assist.Objects.Helpers;
using Assist.Settings;
using Assist.ViewModels;
using AssistUser.Lib.Reputations.Models;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.Utilities;
using ReactiveUI;
using Serilog;
using ValNet.Objects.Coregame;
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

        private string? _playerId;

        private PregameMatch.Player? _player;
        public PregameMatch.Player? Player { get => _player; set => this.RaiseAndSetIfChanged(ref _player, value); }

        private CoregameMatch.Player? _corePlayer;
        public CoregameMatch.Player? CorePlayer { get => _corePlayer; set => this.RaiseAndSetIfChanged(ref _corePlayer, value); }

        public bool PlayerIsHidden = false;


        private string? _playerLevel;

        public string? PlayerLevel
        {
            get => _playerLevel;
            set => this.RaiseAndSetIfChanged(ref _playerLevel, value);
        }
        
        
        
        private string? _playerRankIcon ;

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

        private Bitmap? _playerReputationImage;

        public Bitmap? PlayerReputationImage
        {
            get => _playerReputationImage;
            set => this.RaiseAndSetIfChanged(ref _playerReputationImage, value);
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
        
        private string? _playerTag = "";

        public string? PlayerTag
        {
            get => _playerTag;
            set => this.RaiseAndSetIfChanged(ref _playerTag, value);
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

        private bool? _trackerEnabled = false;
        private bool startUp = true;

        public bool? TrackerEnabled
        {
            get => _trackerEnabled;
            set => this.RaiseAndSetIfChanged(ref _trackerEnabled, value);
        }

        private bool _reputationChecked = false;
        public bool ReputationChecked
        {
            get => _reputationChecked;
            set => this.RaiseAndSetIfChanged(ref _reputationChecked, value);
        }
        
        private string? _shotcallerReputation = "0";

        public string? ShotcallerReputation
        {
            get => _shotcallerReputation;
            set => this.RaiseAndSetIfChanged(ref _shotcallerReputation, value);
        }
        
        private string? _goodteammateReputation = "0";

        public string? GoodteammateReputation
        {
            get => _goodteammateReputation;
            set => this.RaiseAndSetIfChanged(ref _goodteammateReputation, value);
        }
        
        private string? _calmReputation = "0";

        public string? CalmReputation
        {
            get => _calmReputation;
            set => this.RaiseAndSetIfChanged(ref _calmReputation, value);
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
                        {
                            PlayerTag = $"#{data.game_tag}";
                            PlayerName = data.game_name;
                            PlayerIsHidden = false;
                            TrackerEnabled = true;
                            //CheckTracker();
                        }
                            

                    if (data != null)
                    {
                        var t = await GetPresenceData(data);
                        // Set rank icon
                        PlayerRankIcon = $"https://content.assistapp.dev/ranks/TX_CompetitiveTier_Large_{t.competitiveTier}.png";
                        PlayerLevel = $"{t.accountLevel}";
                        _playerId = data.puuid;
                    }
                }
                


                if (!string.IsNullOrEmpty(Player.CharacterID))
                {
                    try
                    {
                        // Set Agent Icon
                        PlayerAgentIcon = $"https://content.assistapp.dev/agents/{Player.CharacterID}_displayicon.png";
                        // Set Agent Name
                        PlayerAgentName = AgentNames.AgentIdToNames?[Player.CharacterID.ToLower()];
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
                    AssistApplication.Current.AssistUser.Dodge.GlobalDodgeUsers.Find(player => player.id == Player.Subject);
                if (user != null)
                {
                    // This means the user was found on the dodge list.
                    IsPlayerDodge = true;
                    Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        DodgeBorder = new SolidColorBrush(new Color(255, 246, 30, 81));
                    });

                }

                if (checkGlobal != null && GameSettings.Current.GlobalListEnabled)
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

                if (!ReputationChecked) SetupReputation();


            }
        }

        /// <summary>
        /// Updates the binded user control data with the PlayerData set in control's Player variable.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateCorePlayerData()
        {
            if (CorePlayer != null)
            {
                if (string.Equals(PlayerName, "Player") || string.IsNullOrEmpty(PlayerRankIcon))
                {
                    var pres = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
                    var data = pres.presences.Find(user => user.puuid == CorePlayer.Subject);

                    if (!CorePlayer.PlayerIdentity.Incognito) // If Incognito is True, then streamer mode is enabled.
                        if (data != null)
                        {
                            PlayerName = data.game_name;
                            PlayerTag = $"#{data.game_tag}";
                            PlayerIsHidden = false;
                            TrackerEnabled = true;
                            //CheckTracker();
                        }
                            
                    
                    if (data != null)
                    {
                        var t = await GetPresenceData(data);
                        // Set rank icon
                        PlayerRankIcon = $"https://content.assistapp.dev/ranks/TX_CompetitiveTier_Large_{t.competitiveTier}.png";
                        PlayerLevel = $"{t.accountLevel}";
                        _playerId = data.puuid;
                    }
                }
                /*if (!Player.PlayerIdentity.Incognito) // If Incognito is false, means that the player has their name publicly shown.
                {
                    var pres = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
                    var data = pres.presences.Find(user => user.puuid == Player.Subject);
                    
                    if (data != null)
                        PlayerName = data.game_name;
                }

                if (string.IsNullOrEmpty(PlayerRankIcon))
                {
                    var pres = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
                    var data = pres.presences.Find(user => user.puuid == Player.Subject);
                    
                    if (data != null)
                    {
                        var t = await GetPresenceData(data);
                        // Set rank icon
                        PlayerRankIcon = $"https://content.assistapp.dev/ranks/TX_CompetitiveTier_Large_{t.competitiveTier}.png";
                    }
                }*/

                if (!string.IsNullOrEmpty(CorePlayer.CharacterID))
                {
                    try
                    {
                        // Set Agent Icon
                        PlayerAgentIcon = $"https://content.assistapp.dev/agents/{CorePlayer.CharacterID.ToLower()}_displayicon.png";
                        // Set Agent Name
                        PlayerAgentName = AgentNames.AgentIdToNames?[CorePlayer.CharacterID.ToLower()];
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex.Message);
                    }
                }

                // Check if user is on dodge list
                // if so enable red border and icon popup.
                var user = DodgeService.Current.UserList.Find(player => player.UserId == CorePlayer.Subject);
                var checkGlobal =
                    AssistApplication.Current.AssistUser.Dodge.GlobalDodgeUsers.Find(player => player.id == CorePlayer.Subject);
                if (user != null)
                {
                    // This means the user was found on the dodge list.
                    IsPlayerDodge = true;
                    Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        DodgeBorder = new SolidColorBrush(new Color(255, 246, 30, 81));
                    });

                }

                if (checkGlobal != null && GameSettings.Current.GlobalListEnabled)
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
                
                if (!ReputationChecked) SetupReputation();

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


        public async Task OpenTracker()
        {
            if (!PlayerIsHidden)
            {
                var trackerName = PlayerName.Replace(" ", "%20");
                var trackerTag = PlayerTag.Replace("#", "");
                string url = $"https://tracker.gg/valorant/profile/riot/{trackerName}%23{trackerTag}/overview";
            
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
        }

        private async Task CheckTracker()
        {
            var trackerName = PlayerName.Replace(" ", "%20");
            string url = $"https://tracker.gg/valorant/profile/riot/{trackerName}%23{PlayerTag}/overview";

            HttpClient h = new HttpClient();

            var resp = await h.GetAsync(url);

            TrackerEnabled = resp.StatusCode == HttpStatusCode.OK;
        }
        
        private async Task SetupReputation()
        {
            var d = await AssistApplication.Current.AssistUser.Reputation.GetUserReputationV2(_playerId);

            if (d.Code != 200)
            {
                Log.Error($"Unable to Locate reputation of player ID : {Player.Subject}");
                Log.Error($"message from request: {d.Message}");
                return;
            }

            var rep = JsonSerializer.Deserialize<AssistReputationUserV2>(d.Data.ToString());

            var t = rep.SeasonalReputation.TryGetValue(AssistApplication.EpisodeId, out AssistSeasonalReputationV2 reputation);

            if (!t)
            {
                Log.Error($"Unable to find seasonal data for ID of {AssistApplication.EpisodeId}. For reputation of player ID : {Player.Subject}");
                return;
            }

            var bitmap = new Bitmap(AssetLoader.Open(new Uri($@"avares://Assist/Resources/Game/Assist_EndorseLevel{reputation.Level}.png")));
            PlayerReputationImage = bitmap;
            
            ReputationChecked = true;
        }
    }
}
