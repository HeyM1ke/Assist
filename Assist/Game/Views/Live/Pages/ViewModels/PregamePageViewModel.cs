using System;
using System.Collections.Generic;
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
using Assist.Services;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;
using Serilog;
using ValNet.Objects.Exceptions;
using ValNet.Objects.Local;
using ValNet.Objects.Pregame;

namespace Assist.Game.Views.Live.Pages.ViewModels
{
    internal class PregamePageViewModel : ViewModelBase
    {
        // Control Container for User Controls
        private List<GameUserControl> _userControls = new List<GameUserControl>();

        public List<GameUserControl> UserControls
        {
            get => _userControls;
            set => this.RaiseAndSetIfChanged(ref _userControls, value);
        }

        List<string> SelectedAgentIds = new List<string>();
        private string? MatchId;

        private string? _errorMessage;

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        private string? _mapImage = "https://content.assistapp.dev/maps/Ascent_BWlistview.png";

        public string? MapImage
        {
            get => _mapImage;
            set => this.RaiseAndSetIfChanged(ref _mapImage, value);
        }

        private string? _mapName = "Loading...";

        public string? MapName
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
        
        private bool setupSucc = false;

        public async Task Setup()
        {
            // Get the current match ID for the game. 
            try
            {
                var getPlayerResp = await AssistApplication.Current.CurrentUser.Pregame.GetPlayer();
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
                Log.Fatal("Error on getting player pregame");
                Log.Fatal("PREGAME ERROR: " + e.StatusCode);
                Log.Fatal("PREGAME ERROR: " + e.Content);
                Log.Fatal("PREGAME ERROR: " + e.Message);
                
                if(e.StatusCode == HttpStatusCode.BadRequest){
                    Log.Fatal("TOKEN ERROR: " + e.Message);
                    await AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                    await this.Setup();
                }
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
                
                AssistApplication.Current.RiotWebsocketService.PregameMessageEvent += RiotWebsocketServiceOnPregameMessageEvent;
                
                setupSucc = true;
            }
        }

        private async void RiotWebsocketServiceOnPregameMessageEvent(object obj)
        {
            // On message recieved Check if it is a PREGAME Message.
            await UpdateData();
        }

        public async Task UpdateData()
        {
            
            
            if (string.IsNullOrEmpty(MatchId))
            {
                Setup();
            }
            Log.Error("Updating Data");
            PregameMatch MatchResp = new PregameMatch();
            ChatV4PresenceObj PresenceResp = new ChatV4PresenceObj();
            try
            {
                MatchResp = await AssistApplication.Current.CurrentUser.Pregame.GetMatch(MatchId!);
                PresenceResp = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
                if (MatchResp == null || PresenceResp == null)
                {
                    Log.Error("Failed on getting update data. Match or Presence");
                    ErrorMessage = "Failed on getting update data. Match or Presence";
                    return;
                }
            }
            catch (RequestException e)
            {
                Log.Fatal("Error on getting player match or Pres");
                Log.Fatal("PREGAME ERROR: " + e.StatusCode);
                Log.Fatal("PREGAME ERROR: " + e.Content);
                Log.Fatal("PREGAME ERROR: " + e.Message);
                
                if(e.StatusCode == HttpStatusCode.BadRequest){
                    Log.Fatal("PREGAME TOKEN ERROR: ");
                    AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                }
                return;
            }

            if(MatchResp.AllyTeam == null)
                return;

            // Log MatchID to Log
            Log.Information($"PREGAME MATCH ID: {MatchResp.ID}");
            
            // With the presences grab all team members presenses

            // get all ids from team
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var teamIds = MatchResp.AllyTeam.Players.Select(p => p.Subject).ToList();

                var allTeamPresences =
                    PresenceResp.presences.FindAll(pres =>
                        teamIds.Contains(pres.puuid)); // Contains every presence from the team.

                //TODO Request Data for teammatesID and store them in the LiveViewViewModel Class to the ReputationUserV2s List. 
                await LiveViewViewModel.GetUserReputations(teamIds);
                
                // Determine partys that are greater than 2 that are on the same team. 
                // Mark them and store the ids in a list.

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
            

            // Now that we have the match data lets go through it. 
            // First check if this is our first time around.
            if (UserControls.Count == 0) // Checking if the List is empty to signify if this is our first time around
            {
                foreach (var allyPlayer in MatchResp.AllyTeam.Players)
                {
                    Log.Information("Adding Player to pregame. : " + allyPlayer.Subject);
                    await AddUserToList(allyPlayer, parties);
                }
            }
            else
            {
                // For every other time around, Update the Players that already excist.
                foreach (var allyPlayer in MatchResp.AllyTeam.Players)
                {
                    Log.Information("Updating Player to pregame. : " + allyPlayer.Subject);
                    await UpdateUserInList(allyPlayer, parties);
                }
            }
            });

            // Update Map Data
            HandleMapData(MatchResp);
            
            HandlePregameMatchTracking(MatchResp);
        }

        private async Task HandlePregameMatchTracking(PregameMatch pregameMatch)
        {
            var recentM = new RecentMatch
            {
                MatchId = pregameMatch.ID,
                AllyTeamId = pregameMatch.AllyTeam.TeamID,
                IsCompleted = false,
                Result = RecentMatch.MatchResult.IN_PROGRESS,
                MapId = pregameMatch.MapID,
                QueueId = pregameMatch.QueueID,
                DateOfMatch = DateTime.UtcNow,
                LengthOfMatchInSeconds = 0,
                AllyTeamScore = 0,
                EnemyTeamScore = 0,
                MatchTrack_LastState = "PREGAME",
                OwningPlayer = AssistApplication.Current.CurrentUser.UserData.sub
            };
            
            HandlePlayers(pregameMatch, recentM);
            HandleRecentMatchPlayers(pregameMatch, recentM);
            
            if (RecentService.Current.RecentMatches.Exists(x => x.MatchId.Equals(recentM.MatchId)))
            {
                RecentService.Current.UpdateMatch(recentM);
                return;
            }
            
            RecentService.Current.AddMatch(recentM);
        }
        
        private async void HandlePlayers(PregameMatch matchDetails , RecentMatch recentMatch)
        {
            var players = matchDetails.AllyTeam.Players;

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
                recentPlayerData.LastSeenMatchId = matchDetails.ID;
                recentPlayerData.Matches.Add(matchDetails.ID);

                if (!recentPlayerData.TimesSeen.TryAdd(matchDetails.ID, recentPlayerData.LastSeen))
                {
                    recentPlayerData.TimesSeen[matchDetails.ID] = recentPlayerData.LastSeen;
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
        private async void HandleRecentMatchPlayers(PregameMatch matchDetails, RecentMatch recentMatch)
        {
            var players = matchDetails.AllyTeam.Players;

            foreach (var playerObj in players)
            {
                var data = UserControls.Find(x =>
                    x.PlayerId.Equals(playerObj.Subject, StringComparison.OrdinalIgnoreCase));

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
                    PlayerRealName = data._viewModel.PlayerRealName,
                    PlayerName = !data._viewModel.UsingAssistProfile ? data._viewModel.PlayerName : data._viewModel.PlayerAgentName.Split('#')[0],
                    PlayerTag = !data._viewModel.UsingAssistProfile ? $"{data._viewModel.PlayerTag}" : data._viewModel.PlayerAgentName.Split('#')[^1],
                    TeamId = matchDetails.AllyTeam.TeamID,
                    Statistics = null
                };
            
                recentMatch.Players.Add(nP);
            }
        }

        private async Task<Dictionary<string, IBrush>> MarkPlayerSimilarParties(List<ChatV4PresenceObj.Presence> PlayerPresences)
        {
            
            // Create a dictionary that relates an id to a brush color.
            // id will be checked within the object to check if the player belongs to a party.
            Dictionary<string, IBrush> partyRelations = new Dictionary<string, IBrush>(); // PlayerID to Color

            // Decode Pres here, to prevent multiple excess decodings.
            Dictionary<string, List<string>> partyIDtoPlayerList  = new Dictionary<string, List<string>>();

            // Decode and Add pres
            foreach (var pres in PlayerPresences)
            {
                if (pres.Private == null)
                {
                    continue;
                }

                var playerPres = await GetPresenceData(pres);




                if(partyIDtoPlayerList.Count == 0)
                    partyIDtoPlayerList.Add(playerPres.partyId.ToLower(), new List<string>() { pres.puuid });
                else
                {
                    if (partyIDtoPlayerList.ContainsKey(playerPres.partyId.ToLower()))
                    {
                        partyIDtoPlayerList[playerPres.partyId.ToLower()].Add(pres.puuid);
                        Log.Information($"Player ID of : {pres.puuid} | Belongs to Party ID of : {playerPres.partyId.ToLower()}");
                    }
                    else
                    {
                        partyIDtoPlayerList.Add(playerPres.partyId.ToLower(), new List<string>() { pres.puuid });
                        Log.Information($"Player ID of : {pres.puuid} | Belongs to Party ID of : {playerPres.partyId.ToLower()}");
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
                            default:
                                assignedColor = new SolidColorBrush(new Color(255, 220, 20, 60));
                                party.Value.ForEach(playerInParty => partyRelations.TryAdd(playerInParty, assignedColor));
                                break;
                        }

                }
            }
            
            
            return partyRelations;
        }

        private async Task AddUserToList(PregameMatch.Player Player, Dictionary<string, IBrush> dic)
        {

            dic.TryGetValue(Player.Subject, out IBrush? color);


            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var temp = new List<GameUserControl>();
                temp.Add(new GameUserControl(Player, color));
                UserControls = UserControls.Concat(temp).ToList();
            });
        }

        private async Task UpdateUserInList(PregameMatch.Player Player, Dictionary<string, IBrush> dic)
        {
            var control = UserControls.Find(control => control.PlayerId == Player.Subject);
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
            await AddUserToList(Player, dic);

        }

        private async Task HandleMapData(PregameMatch Match)
        {
            if (Match.MapID != null)
            {
                Log.Information("Getting map data for ID of: " +  Match.MapID.ToLower());
                MapName = MapNames.MapsByPath?[Match.MapID.ToLower()].ToUpper();
                MapImage =
                    $"https://content.assistapp.dev/maps/{MapNames.MapsByPath?[Match.MapID.ToLower()]}_BWlistview.png";
                try
                {
                    ServerName = ServerNames.ValorantServers[Match.GamePodID];
                }
                catch (Exception e)
                {
                    Log.Error("Failed to get server from ServerNames List.");
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

        public void UnsubscribeFromEvents()
        {
            Log.Information("Page is Unloaded, Unsubbing from Events from PregamePageView");
            AssistApplication.Current.RiotWebsocketService.PregameMessageEvent -= RiotWebsocketServiceOnPregameMessageEvent;
            AssistApplication.Current.RiotWebsocketService.PregameMessageEvent -= RiotWebsocketServiceOnPregameMessageEvent;
        }
    }
}
