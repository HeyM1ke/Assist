using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Controls.Live;
using Assist.Game.Models;
using Assist.Game.Models.Recent;
using Assist.Game.Services;
using Assist.Game.Views.Live.ViewModels;
using Assist.Objects.Helpers;
using Assist.Objects.RiotSocket;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using DiscordRPC.Message;
using DynamicData.Binding;
using ReactiveUI;
using Serilog;
using ValNet.Objects.Coregame;
using ValNet.Objects.Exceptions;
using ValNet.Objects.Local;
using ValNet.Objects.Pregame;

namespace Assist.Game.Views.Live.Pages.ViewModels
{
    internal class IngameViewModel : ViewModelBase
    {
        private ObservableCollectionExtended<GameUserControl> _allyTeamControls = new ObservableCollectionExtended<GameUserControl>();

        public ObservableCollectionExtended<GameUserControl> AllyTeamControls
        {
            get => this._allyTeamControls;
            set => this.RaiseAndSetIfChanged(ref _allyTeamControls, value);
        }
        
        private ObservableCollectionExtended<GameUserControl> _deathTeamControls = new ObservableCollectionExtended<GameUserControl>();

        public ObservableCollectionExtended<GameUserControl> DeathTeamControls
        {
            get => this._deathTeamControls;
            set => this.RaiseAndSetIfChanged(ref _deathTeamControls, value);
        }

        private ObservableCollectionExtended<GameUserControl> _enemyTeamControls = new ObservableCollectionExtended<GameUserControl>();

        public ObservableCollectionExtended<GameUserControl> EnemyTeamControls
        {
            get => this._enemyTeamControls;
            set => this.RaiseAndSetIfChanged(ref _enemyTeamControls, value);
        }

        private string _allyScore = "0";

        public string AllyScore
        {
            get => _allyScore;
            set => this.RaiseAndSetIfChanged(ref _allyScore, value);
        }

        private string _enemyScore = "0";

        public string EnemyScore
        {
            get => _enemyScore;
            set => this.RaiseAndSetIfChanged(ref _enemyScore, value);
        }

        private string _mapName = "Loading..";

        public string MapName
        {
            get => _mapName;
            set => this.RaiseAndSetIfChanged(ref _mapName, value);
        }

        private string? _serverName = "Loading...";

        public string? ServerName
        {
            get => _serverName;
            set => this.RaiseAndSetIfChanged(ref _serverName, value);
        }
        
        private string _mapImage;

        public string MapImage
        {
            get => _mapImage;
            set => this.RaiseAndSetIfChanged(ref _mapImage, value);
        }

        private string _queueName = "Loading..";

        public string QueueName
        {
            get => _queueName;
            set => this.RaiseAndSetIfChanged(ref _queueName, value);
        }

        private string? MatchId;

        private string? _errorMessage;

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }
        
        private bool _isDeathmatch = false;

        public bool IsDeathmatch
        {
            get => _isDeathmatch;
            set => this.RaiseAndSetIfChanged(ref _isDeathmatch, value);
        }
        

        private bool setupSucc = false;

        public async Task Setup()
        {
            // Get the current match ID for the game. 
            try
            {
                var getPlayerResp = await AssistApplication.Current.CurrentUser.CoreGame.FetchPlayer();
                if (getPlayerResp != null)
                {
                    MatchId = getPlayerResp.MatchID;
                }

                if (string.IsNullOrEmpty(MatchId))
                {
                    Log.Error("Failed to get MatchID");
                    ErrorMessage = "Failed to Get MatchID";
                    return;
                }
            }
            catch (RequestException e)
            {
                Log.Fatal("Error on getting player Coregame");
                Log.Fatal("COREGAME ERROR: " + e.StatusCode);
                Log.Fatal("COREGAME ERROR: " + e.Content);
                Log.Fatal("COREGAME ERROR: " + e.Message);
                
                if (e.StatusCode == HttpStatusCode.BadRequest)
                {
                    Log.Fatal("TOKEN ERROR: " + e.Content);
                    await AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                    await this.Setup();
                    return;
                }
                Log.Fatal("Failed to Reauth" );
            }

            /*try
            {
                await AssistApplication.Current.AssistUser.Dodge.GetGlobalDodgeList();
            }
            catch (Exception e)
            {

            }*/

            if (setupSucc == false)
            {
                // First Subscribe to Updates from the PREGAME Api on Websocket. To Update the Data for whenever there is an UPDATE.
                await UpdateData(); // Do inital Pregame Check

                AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent += RiotWebsocketServiceOnUserPresenceMessageEvent;

                setupSucc = true;
            }
        }

        private async void RiotWebsocketServiceOnUserPresenceMessageEvent(PresenceV4Message obj)
        {
            // On message recieved Check if it is a PREGAME Message.
                Log.Information("CORE GAME UPDATE GOTTEN");
                await UpdateData();
        }


        private async Task UpdateData()
        {
            Log.Information("Updating Coregame Data");
            CoregameMatch MatchResp = new CoregameMatch();
            ChatV4PresenceObj PresenceResp = new ChatV4PresenceObj();

            try
            {
                if (string.IsNullOrEmpty(MatchId))
                {
                    await Setup();
                }
                
                MatchResp = await AssistApplication.Current.CurrentUser.CoreGame.FetchMatch(MatchId!);
                PresenceResp = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
                if (MatchResp == null || PresenceResp == null)
                {
                    Log.Error("Failed on getting update data. Coregame Match or Presence");
                    ErrorMessage = "Failed on getting update data. Coregame Match or Presence";
                    return;
                }
            }
            catch (RequestException e)
            {
                Log.Fatal("Error on getting COREGAME player match or Pres");
                Log.Fatal("COREGAME ERROR: " + e.StatusCode);
                Log.Fatal("COREGAME ERROR: " + e.Content);
                Log.Fatal("COREGAME ERROR: " + e.Message);
                
                if(e.StatusCode == HttpStatusCode.BadRequest){
                    Log.Fatal("TOKEN ERROR: ");
                    await AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                    return;
                }
            }

            
                
            
            if (MatchResp.Players == null || MatchResp.Players.Count == 0)
                return;
            
            Log.Information($"COREGAME MATCH ID: {MatchResp.MatchID}");
            
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var allyTeam = await GetAllyTeam(MatchResp.Players);
                var enemyTeam = await GetEnemyTeam(MatchResp.Players);
                var allMatchPlayerIds = MatchResp.Players.Select(p => p.Subject).ToList();

                
                await LiveViewViewModel.GetUserReputations(allMatchPlayerIds);
                
                var allTeamPresences =
                    PresenceResp.presences.FindAll(pres =>
                        allMatchPlayerIds.Contains(pres.puuid));

                Dictionary<string, IBrush> parties = new Dictionary<string, IBrush>();
                try
                {
                    parties = await MarkPlayerSimilarParties(allTeamPresences);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    Log.Error(e.StackTrace);
                    parties.Clear();
                }

                
                if (AllyTeamControls.Count == 0) 
                {
                    foreach (var allyPlayer in allyTeam)
                    {
                        Log.Information("Adding Ally Player to Coregame. : " + allyPlayer.Subject);
                        await AddUserToAllyList(allyPlayer, parties);
                    }
                }
                else
                {
                    // For every other time around, Update the Players that already excist.
                    foreach (var allyPlayer in allyTeam)
                    {
                        Log.Information("Updating Ally Player to Coregame. : " + allyPlayer.Subject);
                        await UpdateUserInAllyList(allyPlayer, parties);
                    }
                }

                if (EnemyTeamControls.Count == 0)
                {
                    foreach (var enemyPlayer in enemyTeam)
                    {
                        Log.Information("Adding Enemy Player to Coregame. : " + enemyPlayer.Subject);
                        await AddUserToEnemyList(enemyPlayer, parties);
                    }
                }
                else
                {
                    // For every other time around, Update the Players that already excist.
                    foreach (var enemyPlayer in enemyTeam)
                    {
                        Log.Information("Updating Enemy Player to Coregame. : " + enemyPlayer.Subject);
                        await UpdateUserInEnemyList(enemyPlayer, parties);
                    }
                }
            });


            
            // Update Map Data
            HandleMatchData(MatchResp, PresenceResp.presences.Find(p => p.puuid == AssistApplication.Current.CurrentUser.UserData.sub));
            
            HandleIngameMatchTracking(MatchResp, PresenceResp.presences.Find(p => p.puuid == AssistApplication.Current.CurrentUser.UserData.sub));
        }
        
        private async Task<Dictionary<string, IBrush>> MarkPlayerSimilarParties(List<ChatV4PresenceObj.Presence> allTeamPresences)
        {
            // Create a dictionary that relates an id to a brush color.
            // id will be checked within the object to check if the player belongs to a party.
            Dictionary<string, IBrush> partyRelations = new Dictionary<string, IBrush>(); // PlayerID to Color

            // Decode Pres here, to prevent multiple excess decodings.
            Dictionary<string, List<string>> partyIDtoPlayerList = new Dictionary<string, List<string>>();

            // Decode and Add pres
            foreach (var pres in allTeamPresences)
            {
                if (pres.Private == null)
                {
                    continue;
                }

                var playerPres = await GetPresenceData(pres);




                if (partyIDtoPlayerList.Count == 0)
                    partyIDtoPlayerList.Add(playerPres.partyId.ToLower(), new List<string>() { pres.puuid });
                else
                {
                    if (partyIDtoPlayerList.ContainsKey(playerPres.partyId))
                    {
                        partyIDtoPlayerList[playerPres.partyId.ToLower()].Add(pres.puuid);
                        Log.Information($"Player ID of : {pres.puuid} | Belongs to Party ID of : {playerPres.partyId.ToLower()}");
                    }
                    else
                    {
                        partyIDtoPlayerList.Add(playerPres.partyId.ToLower(), new List<string>() { pres.puuid });
                    }
                }


                // Creates a Dictionary to relate each party ID to a list of users in the party.
            }

            // Now As we want the ID to Color Translation, convert the list to a new list.
            foreach (var party in partyIDtoPlayerList)
            {
                
                if (party.Value.Count > 1)
                {
                    // The party has value with multiple people on the team being in the party.
                    IBrush assignedColor = null;

                    switch (partyRelations.Count)
                    {
                        // Assign Color to each party, as there are 5 different partys just have 5 diff cases
                        case 0:
                            assignedColor = new SolidColorBrush(new Color(255, 0, 255, 127));
                            party.Value.ForEach(playerInParty => partyRelations.TryAdd(playerInParty, assignedColor));
                            break;
                        case 1:
                            assignedColor = new SolidColorBrush(new Color(255, 255, 215, 0));
                            party.Value.ForEach(playerInParty => partyRelations.TryAdd(playerInParty, assignedColor));
                            break;
                        case 2:
                            assignedColor = new SolidColorBrush(new Color(255, 32, 178, 170));
                            party.Value.ForEach(playerInParty => partyRelations.TryAdd(playerInParty, assignedColor));
                            break;
                        case 3:
                            assignedColor = new SolidColorBrush(new Color(255, 148, 0, 211));
                            party.Value.ForEach(playerInParty => partyRelations.TryAdd(playerInParty, assignedColor));
                            break;
                        case 4:
                            assignedColor = new SolidColorBrush(new Color(255, 112, 128, 144));
                            party.Value.ForEach(playerInParty => partyRelations.TryAdd(playerInParty, assignedColor));
                            break;
                        case 5:
                            assignedColor = new SolidColorBrush(new Color(255, 0, 128, 128));
                            party.Value.ForEach(playerInParty => partyRelations.TryAdd(playerInParty, assignedColor));
                            break;
                        case 6:
                            assignedColor = new SolidColorBrush(new Color(255, 221, 160, 221));
                            party.Value.ForEach(playerInParty => partyRelations.TryAdd(playerInParty, assignedColor));
                            break;
                        case 7:
                            assignedColor = new SolidColorBrush(new Color(255, 210, 105, 30));
                            party.Value.ForEach(playerInParty => partyRelations.TryAdd(playerInParty, assignedColor));
                            break;
                        case 8:
                            assignedColor = new SolidColorBrush(new Color(255, 128, 128, 0));
                            party.Value.ForEach(playerInParty => partyRelations.TryAdd(playerInParty, assignedColor));
                            break;
                        default:
                            assignedColor = new SolidColorBrush(new Color(255, 220, 20, 60));
                            party.Value.ForEach(playerInParty => partyRelations.TryAdd(playerInParty, assignedColor));
                            break;
                    }

                }
            }


            return partyRelations;
        }

        private async Task<List<CoregameMatch.Player>> GetEnemyTeam(List<CoregameMatch.Player> matchRespPlayers)
        {
            var currentPlayer = matchRespPlayers.Find(player => player.Subject == AssistApplication.Current.CurrentUser.UserData.sub);

            var enemyTeam = matchRespPlayers.FindAll(player => player.TeamID != currentPlayer.TeamID);

            return enemyTeam;
        }

        private async Task<List<CoregameMatch.Player>> GetAllyTeam(List<CoregameMatch.Player> matchRespPlayers)
        {
            var currentPlayer = matchRespPlayers.Find(player => player.Subject == AssistApplication.Current.CurrentUser.UserData.sub);

            var allyTeamMates = matchRespPlayers.FindAll(player => player.TeamID == currentPlayer.TeamID);

            return allyTeamMates;
        }

        private async Task HandleMatchData(CoregameMatch Match, ChatV4PresenceObj.Presence currentUser)
        {
            if (Match.MapID != null)
            {
                Log.Information("Getting map data for ID of: " + Match.MapID.ToLower());
                MapName = MapNames.MapsByPath?[Match.MapID.ToLower()].ToUpper();
                MapImage =
                    $"https://content.assistapp.dev/maps/{MapNames.MapsByPath?[Match.MapID.ToLower()]}_Featured.png";
            }

            if (Match.MatchData != null)
            {
                Log.Information("Getting queue data for ID of: " + Match.MatchData.QueueID.ToLower());

                var queueName =  QueueNames.DetermineQueueKey(Match.MatchData.QueueID.ToLower());
                
                // Check if the Queue is Deathmatch.
                IsDeathmatch = Match.MatchData.QueueID.ToLower() == "deathmatch";
                
                QueueName = queueName.ToUpper();

                try
                {
                    ServerName = ServerNames.ValorantServers[Match.GamePodID];
                }
                catch (Exception e)
                {
                    Log.Error("Failed to get Gamepod Server Name from Dictionary");
                }
            }

            if (Match.ProvisioningFlow == "CustomGame")
            {
                Log.Information("Getting ProvisioningFlow data for ID of: " + Match.ProvisioningFlow);

                QueueName = "CUSTOM GAME";
                
                try
                {
                    ServerName = ServerNames.ValorantServers[Match.GamePodID];
                }
                catch (Exception e)
                {
                    Log.Error("Failed to get Gamepod Server Name from Dictionary");
                }
            }

            if (currentUser != null)
            {
                var pres = await GetPresenceData(currentUser);

                if (pres != null)
                {
                    AllyScore = pres.partyOwnerMatchScoreAllyTeam.ToString();
                    EnemyScore = pres.partyOwnerMatchScoreEnemyTeam.ToString();
                }
            }

            
        }

        public async Task<PlayerPresence> GetPresenceData(ChatV4PresenceObj.Presence data)
        {
            if (string.IsNullOrEmpty(data.Private))
                return new PlayerPresence();
            byte[] stringData = Convert.FromBase64String(data.Private);
            string decodedString = Encoding.UTF8.GetString(stringData);
            return JsonSerializer.Deserialize<PlayerPresence>(decodedString);
        }

        private async Task AddUserToAllyList(CoregameMatch.Player Player, Dictionary<string, IBrush> dic)
        {
            var checkForExcisting =
                AllyTeamControls.ToList().Find(control => control.PlayerId == Player.Subject);
            if (checkForExcisting != null)
                return;

            dic.TryGetValue(Player.Subject, out IBrush? color);


            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (IsDeathmatch)
                    DeathTeamControls.Add(new GameUserControl(Player, color));
                else
                    AllyTeamControls.Add(new GameUserControl(Player, color));
            });
        }

        private async Task UpdateUserInAllyList(CoregameMatch.Player Player, Dictionary<string, IBrush> dic)
        {
            GameUserControl control;
            
            if (IsDeathmatch)
                control = DeathTeamControls.ToList().Find(control => control.PlayerId == Player.Subject);
            else
                control = AllyTeamControls.ToList().Find(control => control.PlayerId == Player.Subject);
            
            dic.TryGetValue(Player.Subject, out IBrush? color);
            if (control != null)
            {
                Log.Information("Updating Data for Previously found player for Coregame. : " + Player.Subject);
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await control.UpdatePlayer(Player, color);
                });

                return;
            }

            Log.Fatal("Tried Updating Control for User that does not exist?");
            Log.Fatal("Adding User to list.");
            await AddUserToAllyList(Player, dic);

        }

        private async Task AddUserToEnemyList(CoregameMatch.Player Player, Dictionary<string, IBrush> dic)
        {

            var checkForExcisting =
                EnemyTeamControls.ToList().Find(control => control.PlayerId == Player.Subject);
            if (checkForExcisting != null)
                return;

            dic.TryGetValue(Player.Subject, out IBrush? color);


            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                EnemyTeamControls.Add(new GameUserControl(Player, color));
            });
        }

        private async Task UpdateUserInEnemyList(CoregameMatch.Player Player, Dictionary<string, IBrush> dic)
        {
            var control = EnemyTeamControls.ToList().Find(control => control.PlayerId == Player.Subject);
            dic.TryGetValue(Player.Subject, out IBrush? color);
            if (control != null)
            {
                Log.Information("Updating Data for Previously found player for pregame. : " + Player.Subject);
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await control.UpdatePlayer(Player, color);
                });

                return;
            }

            Log.Fatal("Tried Updating Control for User that does not exist?");
            Log.Fatal("Adding User to list.");
            await AddUserToAllyList(Player, dic);

        }

        private async Task HandleIngameMatchTracking(CoregameMatch matchResp, ChatV4PresenceObj.Presence? currentUser)
        {

            var existingMatch = RecentService.Current.RecentMatches?.Find(x => x.MatchId.Equals(matchResp.MatchID));
            var pres = await GetPresenceData(currentUser);
            
            var recentM = new RecentMatch
            {
                MatchId = matchResp.MatchID,
                AllyTeamId = matchResp.Players?.Find(x => x.Subject.Equals(AssistApplication.Current.CurrentUser.UserData.sub,
                    StringComparison.OrdinalIgnoreCase))?.TeamID ?? "",
                IsCompleted = false,
                Result = RecentMatch.MatchResult.IN_PROGRESS,
                MapId = matchResp.MapID,
                QueueId = matchResp.MatchData.QueueID,
                DateOfMatch = DateTime.UtcNow,
                LengthOfMatchInSeconds = 0,
                AllyTeamScore = pres.partyOwnerMatchScoreAllyTeam,
                EnemyTeamScore = pres.partyOwnerMatchScoreEnemyTeam,
                MatchTrack_LastState = "INGAME",
                OwningPlayer = AssistApplication.Current.CurrentUser.UserData.sub
            };

            if (existingMatch is not null)
            {
                recentM = existingMatch;
            }

            if (matchResp.Players?.Count == 0)
                return;
            
            HandlePlayers(matchResp, recentM);
            HandleRecentMatchPlayers(matchResp, recentM);
            
            if (RecentService.Current.RecentMatches.Exists(x => x.MatchId.Equals(recentM.MatchId)))
            {
                RecentService.Current.UpdateMatch(recentM);
                return;
            }
            
            RecentService.Current.AddMatch(recentM);
        }
        
        private async void HandlePlayers(CoregameMatch matchDetails , RecentMatch recentMatch)
        {
            var players = matchDetails.Players;

            foreach (var playerObj in players)
            {
                var recentPlayerData = RecentService.Current.RecentPlayers?.Find(ply => ply.PlayerId.Equals(playerObj.Subject, StringComparison.OrdinalIgnoreCase));
                if (recentPlayerData is null)
                {
                    recentPlayerData = new RecentPlayer()
                    {
                        FirstSeen = DateTime.UtcNow,
                        PlayerId = playerObj.Subject
                    };
                }


                recentPlayerData.LastSeen = DateTime.UtcNow;
                recentPlayerData.LastSeenMatchId = matchDetails.MatchID;
                recentPlayerData.Matches.Add(matchDetails.MatchID);

                if (!recentPlayerData.TimesSeen.TryAdd(matchDetails.MatchID, recentPlayerData.LastSeen))
                {
                    recentPlayerData.TimesSeen[matchDetails.MatchID] = recentPlayerData.LastSeen;
                }
                
                int index = RecentService.Current.RecentPlayers.FindIndex(ply => ply.PlayerId.Equals(recentPlayerData.PlayerId));
                if (index < 0)
                    RecentService.Current.RecentPlayers?.Add(recentPlayerData);
                else
                    RecentService.Current.RecentPlayers[index] = recentPlayerData;
            }
        
        
        }
        
        /// <summary>
        /// Creates the Player for the RecentMatch Object.
        /// </summary>
        /// <param name="matchDetails"></param>
        /// <param name="recentMatch"></param>
        private async void HandleRecentMatchPlayers(CoregameMatch matchDetails, RecentMatch recentMatch)
        {
            var players = matchDetails.Players;

            if (players.Count == 0)
            {
                return;
            }
            
            foreach (var playerObj in players)
            {
                GameUserControl? data = null;
                if (IsDeathmatch)
                {
                    data = DeathTeamControls.ToList().Find(x =>
                        x.PlayerId.Equals(playerObj.Subject, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    data = AllyTeamControls.ToList().Find(x =>
                        x.PlayerId.Equals(playerObj.Subject, StringComparison.OrdinalIgnoreCase));

                    if (data is null)
                    {
                        data = EnemyTeamControls.ToList().Find(x =>
                            x.PlayerId.Equals(playerObj.Subject, StringComparison.OrdinalIgnoreCase));
                    }
                }

                if (data is null)
                {
                    continue;
                }

                if (recentMatch.Players.Exists(x => x.PlayerId.Equals(playerObj.Subject)))
                {
                    continue;
                }
                
                var nP = new RecentMatch.Player()
                {
                    PlayerId = playerObj.Subject,
                    CompetitiveTier = (int)data._viewModel.PlayerCompetitiveTier,
                    PlayerAgentId = playerObj.CharacterID,
                    PlayerName = !data._viewModel.UsingAssistProfile ? data._viewModel.PlayerName : data._viewModel.PlayerAgentName.Split('#')[0],
                    PlayerTag = !data._viewModel.UsingAssistProfile ? data._viewModel.PlayerTag : $"{data._viewModel.PlayerAgentName.Split('#')[^1]}",
                    TeamId = playerObj.TeamID,
                    PlayerRealName = data._viewModel.PlayerRealName,
                    Statistics = null
                };
            
                recentMatch.Players.Add(nP);
            }
        }

        public void UnsubscribeFromEvents()
        {
            Log.Information("Page is Unloaded, Unsubbing from Events from IngameView");
            AllyTeamControls.Clear();
            EnemyTeamControls.Clear();
            DeathTeamControls.Clear();
            
            AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent -= RiotWebsocketServiceOnUserPresenceMessageEvent;
        }
    }
}
