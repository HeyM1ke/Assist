using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Assist.Core.Helpers;
using Assist.Models.Game;
using Assist.Properties;
using Assist.Services.Assist;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using ValNet.Objects.Coregame;
using ValNet.Objects.Pregame;

namespace Assist.ViewModels.Game;

public partial class LivePlayerPreviewViewModel : ViewModelBase
{
    private const string DefaultPlayerName = "Player";
    [ObservableProperty] private string _agentIconUrl = $"https://cdn.assistval.com/agents/unknown_displayicon.png";
    [ObservableProperty] private string _levelText;
    
    [ObservableProperty] private string _playerName = DefaultPlayerName;
    [ObservableProperty] private string _tagLineText = "";
    [ObservableProperty] private bool _tagLineVisible = false;
    [ObservableProperty] private string _secondaryText = Resources.Common_Selecting; // Agent Text or Player Name
    [ObservableProperty] private string _dodgeText = "";
    [ObservableProperty] private bool _dodgeVisible = false;
    // Right Hand Details
    [ObservableProperty] private string _lastSeenText= "";
    [ObservableProperty] private bool _lastSeenVisible = false;
    [ObservableProperty] private string _pathToReputation = "";
    [ObservableProperty] private string _rankIcon = "";
    [ObservableProperty] private int _playerCompetitiveTier = 0;
    
    [ObservableProperty] private PregameMatch.Player? _player; 
    [ObservableProperty] private CoregameMatch.Player? _corePlayer;
    [ObservableProperty] private IBrush? _playerBrush = null;
    
    //ContextMenu Properties
    [ObservableProperty] private bool _playerIsHidden = true;
    [ObservableProperty] private bool _trackerEnabled = false;
    [ObservableProperty]private bool _usingAssistProfile = false;
    
    
    private string _playerId;
    private string _currentAgentId = "0";
    [ObservableProperty]private string _playerRealName = "";
    private bool _basePlayerDataFlag = false;
    private bool _recentChecked = false;
    


    public async Task UpdatePlayerData()
    {
        if (Player != null)
        {
            _playerId = Player.Subject;

            if (!_basePlayerDataFlag)
            {
                #region Get Presence Data

                
                var pres = await AssistApplication.ActiveUser.Presence.GetPresences();

                if (pres is null)
                {
                    Log.Error("Failed to get presence data.");
                }

                var pData = pres.presences.FirstOrDefault(x => x.puuid == Player.Subject);
                #endregion
                
                if (pData is null) // Presence data for the player does not exist, this is a common issue.
                {
                    Log.Information("Player Presence could not be found for ID of " + Player.Subject);
                    Log.Information("Retrieving Data from Server");
                    
                    await SetupPlayerContent();
                }
                else
                {
                    Log.Information("Player Presence was found.");
                    
                    if (!Player.PlayerIdentity.Incognito || AssistApplication.AssistUser.Authentication.Roles.Contains("ASS-developer-testing")) // If Incognito is True, then streamer mode is enabled.
                        if (pData != null)
                        {
                            if (!_usingAssistProfile)
                            {
                                TagLineVisible = true;
                                TagLineText = $"#{pData.game_tag}";
                                PlayerName = pData.game_name;
                            }

                            PlayerIsHidden = false;
                            TrackerEnabled = true;
                        }
                    
                    // Ran Regardless of any settings, roles, or anything. This is basic data.
                    if (pData != null)
                    {
                        var t = await ValorantHelper.GetPresenceData(pData);
                        PlayerCompetitiveTier = t.competitiveTier;
                        RankIcon =
                            $"https://content.assistapp.dev/ranks/TX_CompetitiveTier_Large_{t.competitiveTier}.png";
                        LevelText = $"{t.accountLevel:N0}";
                        _playerId = pData.puuid;
                        _playerRealName = $"{pData.game_name}#{pData.game_tag}";
                    }
                }

                _basePlayerDataFlag = true; // All player base data has been handled.
            }
            
            // Handle Agent Data
            if (!string.IsNullOrEmpty(Player.CharacterID) && !Player.CharacterID.Equals(_currentAgentId))
            {
                _currentAgentId = Player.CharacterID;
                try
                {
                    // Set Agent Icon
                    AgentIconUrl = $"https://content.assistapp.dev/agents/{Player.CharacterID.ToLower()}_displayicon.png";
                    // Set Agent Name
                    if (!UsingAssistProfile)
                        SecondaryText = ValorantHelper.AgentIdToNames?[Player.CharacterID.ToLower()];
                    
                    Log.Information("Player Agent Locked State: " + Player.CharacterSelectionState);
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex.Message);
                }
            }
            
            SetupAssistFeatures(_playerId);
        }
    }

    public async Task UpdateCorePlayerData()
    {
        if (CorePlayer != null)
        {
            _playerId = CorePlayer.Subject;

            if (!_basePlayerDataFlag)
            {
                #region Get Presence Data


                var pres = await AssistApplication.ActiveUser.Presence.GetPresences();

                if (pres is null)
                {
                    Log.Error("Failed to get presence data.");
                }

                var pData = pres.presences.FirstOrDefault(x => x.puuid == CorePlayer.Subject);

                #endregion

                if (pData is null) // Presence data for the CorePlayer does not exist, this is a common issue.
                {
                    Log.Information("CorePlayer Presence could not be found for ID of " + CorePlayer.Subject);
                    Log.Information("Retrieving Data from Server");

                    await SetupPlayerContent();
                }
                else
                {
                    Log.Information("CorePlayer Presence was found.");

                    if (!CorePlayer.PlayerIdentity.Incognito ||
                        AssistApplication.AssistUser.Authentication.Roles
                            .Contains("ASS-developer-testing")) // If Incognito is True, then streamer mode is enabled.
                        if (pData != null)
                        {
                            if (!_usingAssistProfile)
                            {
                                TagLineVisible = true;
                                TagLineText = $"#{pData.game_tag}";
                                PlayerName = pData.game_name;
                            }

                            PlayerIsHidden = false;
                            TrackerEnabled = true;
                        }

                    // Ran Regardless of any settings, roles, or anything. This is basic data.
                    if (pData != null)
                    {
                        var t = await ValorantHelper.GetPresenceData(pData);
                        PlayerCompetitiveTier = t.competitiveTier;
                        RankIcon =
                            $"https://cdn.assistval.com/ranks/TX_CompetitiveTier_Large_{t.competitiveTier}.png";
                        LevelText = $"{t.accountLevel:N0}";
                        _playerId = pData.puuid;
                        _playerRealName = $"{pData.game_name}#{pData.game_tag}";
                    }
                }

                _basePlayerDataFlag = true; // All CorePlayer base data has been handled.
            }

            // Handle Agent Data
            if (!string.IsNullOrEmpty(CorePlayer.CharacterID) && !CorePlayer.CharacterID.Equals(_currentAgentId))
            {
                _currentAgentId = CorePlayer.CharacterID;
                try
                {
                    // Set Agent Icon
                    AgentIconUrl = $"https://cdn.assistval.com/agents/{CorePlayer.CharacterID.ToLower()}_displayicon.png";
                    // Set Agent Name
                    if (!UsingAssistProfile)
                        SecondaryText = ValorantHelper.AgentIdToNames?[CorePlayer.CharacterID.ToLower()];
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex.Message);
                }
            }

            SetupAssistFeatures(_playerId);
        }
    }

    private async Task SetupPlayerContent()
    {
        // First Check if the player's data already exists.
        if (LiveViewViewModel.ValorantPlayers.TryGetValue(_playerId, out var possiblePlayerData))
        {
            if (!possiblePlayerData.IsOld())
            {
                await ApplyPlayerContent(possiblePlayerData);
            }
            else
            {
                await possiblePlayerData.Setup();
                await ApplyPlayerContent(possiblePlayerData);
            }

            return;
        }

        // If it doesnt exist create and set, and add to dictionary.
        var ply = new ValorantPlayerStorage(_playerId);
        await ply.Setup();
        await ApplyPlayerContent(ply);
        LiveViewViewModel.ValorantPlayers.TryAdd(_playerId, ply);

    }
    private async Task ApplyPlayerContent(ValorantPlayerStorage playerStorage)
        {
            if (Player != null)
            {
                if (!Player.PlayerIdentity.Incognito || AssistApplication.AssistUser.Authentication.Roles.Contains("ASS-developer-testing"))
                {
                    if (!_usingAssistProfile)
                    {
                        TagLineVisible = true;
                        TagLineText = $"#{playerStorage.Tagline}";
                        PlayerName = playerStorage.GameName;
                    }

                    PlayerIsHidden = false;
                    TrackerEnabled = true;
                }
                

                PlayerCompetitiveTier = playerStorage.CompetitiveTier;
                RankIcon =
                    $"https://cdn.assistval.com/ranks/TX_CompetitiveTier_Large_{playerStorage.CompetitiveTier}.png";
                LevelText = $"{_player.PlayerIdentity.AccountLevel:N0}";
                _playerRealName = $"{playerStorage.GameName}#{playerStorage.Tagline}";
            }

            if (CorePlayer != null)
            {
                if (!CorePlayer.PlayerIdentity.Incognito || AssistApplication.AssistUser.Authentication.Roles.Contains("ASS-developer-testing"))
                {
                    if (!_usingAssistProfile)
                    {
                        TagLineVisible = true;
                        TagLineText = $"#{playerStorage.Tagline}";
                        PlayerName = playerStorage.GameName;
                    }

                    PlayerIsHidden = false;
                    TrackerEnabled = true;
                }
                

                PlayerCompetitiveTier = playerStorage.CompetitiveTier;
                RankIcon =
                    $"https://cdn.assistval.com/ranks/TX_CompetitiveTier_Large_{playerStorage.CompetitiveTier}.png";
                LevelText = $"{CorePlayer.PlayerIdentity.AccountLevel:N0}";
                _playerRealName = $"{playerStorage.GameName}#{playerStorage.Tagline}";
            }
        }
    
    private void SetupAssistFeatures(string userId)
        {
            if (!_recentChecked)
            {
                var p = RecentService.Current.RecentPlayers.Find(x => x.PlayerId.Equals(userId));

                if (p != null)
                {
                    if (p.TimesSeen.Count > 1)
                    {
                        var dateTimesDescending = p.TimesSeen.OrderBy(d => d.Value);

                        var lastMat = dateTimesDescending.LastOrDefault(x => x.Key != p.LastSeenMatchId);
                        var t = AsTimeAgo(lastMat.Value);
                        LastSeenVisible = true;
                        LastSeenText = t;
                    }
                }
                
                _recentChecked = true;
            }
            
            
            /*// Check if user is on dodge list
            // if so enable red border and icon popup.
            var user = DodgeService.Current.UserList.Find(player => player.UserId == userId);
            var checkGlobal =
                AssistApplication.AssistUser.Dodge.GlobalDodgeUsers.Find(player => player.id == userId);
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

            if (!ReputationChecked && RecentService.Current.RecentPlayers.Exists(x => x.PlayerId.Equals(userId)))
            {
                var p = RecentService.Current.RecentPlayers.Find(x => x.PlayerId.Equals(userId));

                if (p != null)
                {
                    if (p.TimesSeen.Count > 1)
                    {
                        var dateTimesDescending = p.TimesSeen.OrderBy(d => d.Value);

                        var lastMat = dateTimesDescending.LastOrDefault(x => x.Key != p.LastSeenMatchId);
                        var t = AsTimeAgo(lastMat.Value);
                        PlayerLastSeenEnabled = true;
                        PlayerLastSeen = t;
                    }
                }
            }*/

            //if (!ReputationChecked) SetupReputation();

            //if (!ProfileChecked) SetupProfile();

        }
    
    private static string AsTimeAgo(DateTime dateTime)
    {
        TimeSpan timeSpan = DateTime.UtcNow.Subtract(dateTime);

        return timeSpan.TotalSeconds switch
        {
            <= 60 => $"{timeSpan.Seconds} Seconds",

            _ => timeSpan.TotalMinutes switch
            {
                <= 1 => $"{timeSpan.Minutes} {Resources.Common_Minute}",
                < 60 => $"{timeSpan.Minutes} {Resources.Common_Minute_Plural}",
                _ => timeSpan.TotalHours switch
                {
                    <= 1 => $"1 {Resources.Common_Hour}",
                    < 24 => $"{timeSpan.Hours} {Resources.Common_Hour_Plural}",
                    _ => timeSpan.TotalDays switch
                    {
                        <= 1 => $"1 {Resources.Common_Day}",
                        <= 30 => $"{timeSpan.Days} {Resources.Common_Day_Plural}",

                        <= 60 => $"1 {Resources.Common_Month}",
                        < 365 => $"{timeSpan.Days / 30} {Resources.Common_Month_Plural}",

                        <= 365 * 2 => $"1 {Resources.Common_Year}",
                        _ => $"{timeSpan.Days / 365} {Resources.Common_Years_Plural}"
                    }
                }
            }
        };
    }

    [RelayCommand]
    public async Task OpenTracker()
    {
        if (!PlayerIsHidden)
        {
            var names = PlayerRealName.Split("#");
            var trackerName = names[0].Replace(" ", "%20");
            string url = $"https://tracker.gg/valorant/profile/riot/{trackerName}%23{names[1]}/overview";

            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}