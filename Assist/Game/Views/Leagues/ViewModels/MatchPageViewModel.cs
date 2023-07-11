using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Controls.Live;
using Assist.Game.Controls.Match;
using Assist.Game.Controls.Navigation;
using Assist.Game.Models;
using Assist.Game.Services;
using Assist.Objects.Helpers;
using Assist.Objects.RiotSocket;
using Assist.ViewModels;
using AssistUser.Lib.Leagues.Models;
using Avalonia.Threading;
using DynamicData.Kernel;
using NuGet.Versioning;
using ReactiveUI;
using Serilog;
using ValNet.Objects.Exceptions;
using ValNet.Objects.Local;
using ValNet.Objects.Pregame;

namespace Assist.Game.Views.Leagues.ViewModels;

public class MatchPageViewModel : ViewModelBase
{
    #region Variables
    private ObservableCollection<MatchPlayerControl> _teamAControls = new ObservableCollection<MatchPlayerControl>();
    public ObservableCollection<MatchPlayerControl> TeamAControls
    {
        get => this._teamAControls;
        set => this.RaiseAndSetIfChanged(ref _teamAControls, value);
    }
    private ObservableCollection<MatchPlayerControl> _teamBControls = new ObservableCollection<MatchPlayerControl>();
    public ObservableCollection<MatchPlayerControl> TeamBControls
    {
        get => this._teamBControls;
        set => this.RaiseAndSetIfChanged(ref _teamBControls, value);
    }
    private string _teamAName = "Team A";
    public string TeamAName
    {
        get => this._teamAName;
        set => this.RaiseAndSetIfChanged(ref _teamAName, value);
    }
    
    private string _teamBName = "Team B";
    public string TeamBName
    {
        get => this._teamBName;
        set => this.RaiseAndSetIfChanged(ref _teamBName, value);
    }
    
    
    private string _matchHeaderTitle = "Loading...";
    public string MatchHeaderTitle
    {
        get => this._matchHeaderTitle;
        set => this.RaiseAndSetIfChanged(ref _matchHeaderTitle, value);
    }
    
    private ObservableCollection<MatchSelectionControl> _mapControls = new ObservableCollection<MatchSelectionControl>();
    public ObservableCollection<MatchSelectionControl> MapControls
    {
        get => this._mapControls;
        set => this.RaiseAndSetIfChanged(ref _mapControls, value);
    }
    
    private bool _mapPickBanEnabled = false;
    public bool MapPickBanEnabled
    {
        get => this._mapPickBanEnabled;
        set => this.RaiseAndSetIfChanged(ref _mapPickBanEnabled, value);
    }
    
    private bool _currentlyInMatch = false;
    public bool CurrentlyInMatch
    {
        get => this._currentlyInMatch;
        set => this.RaiseAndSetIfChanged(ref _currentlyInMatch, value);
    }
    
    
    private string _matchMapName = "";
    public string MatchMapName
    {
        get => this._matchMapName;
        set => this.RaiseAndSetIfChanged(ref _matchMapName, value);
    }
    
    private string _matchMapImage = "";
    public string MatchMapImage
    {
        get => this._matchMapImage;
        set => this.RaiseAndSetIfChanged(ref _matchMapImage, value);
    }
    
    private string _matchServerName = "";
    public string MatchServerName
    {
        get => this._matchServerName;
        set => this.RaiseAndSetIfChanged(ref _matchServerName, value);
    }
    
    private bool _joinLobbyButtonEnabled = false;
    public bool JoinLobbyButtonEnabled
    {
        get => this._joinLobbyButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _joinLobbyButtonEnabled, value);
    }
    
    private bool _readyButtonEnabled = false;
    public bool ReadyLobbyButtonEnabled
    {
        get => this._readyButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _readyButtonEnabled, value);
    }


    private string _matchId;
    
    
    
    private string _currentGameScoreTeamOne = "0";
    public string CurrentGameScoreTeamOne
    {
        get => this._currentGameScoreTeamOne;
        set => this.RaiseAndSetIfChanged(ref _currentGameScoreTeamOne, value);
    }
    
    private string _currentGameScoreTeamTwo = "0";
    public string CurrentGameScoreTeamTwo
    {
        get => this._currentGameScoreTeamTwo;
        set => this.RaiseAndSetIfChanged(ref _currentGameScoreTeamTwo, value);
    }
    #endregion
    
    public async Task SetupBasePage()
    {
        new MatchService();
        // Confirm that all navigation is disabled.
        VerticalGameNavigation.Instance.DisableAll();
        
        // Get Match Data
        var MatchData = await MatchService.Instance.GetCurrentMatch();
        // Create Player & Match 
        await CreatePlayerControls(MatchData);
        // Subscribe to Match Data Server
        await BindToEvents();
        
            // Unready Valorant Client
        await AssistApplication.Current.CurrentUser.Party.SetPartyReadiness(false);

        await UpdatePage(MatchData);
    }

    private void MatchUpdateRecieved(string? obj)
    {
        try
        {
            var MatchData = JsonSerializer.Deserialize<AssistMatch>(obj);
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await UpdatePage(MatchData);
            });
        }
        catch (Exception e)
        {
            Log.Error("Failed to Parse Match Data from Server");
            Log.Error($"Exception: {e.Message}");
            Log.Error($"Stack: {e.StackTrace}");
        }
    }


    private async Task UpdatePage(AssistMatch matchData)
    {
        // Check State
        switch (matchData.State)
        {
            case "PREGAME":
                await SetupPregameState(matchData);
                break;
            case "INGAME":
                await SetupIngameState(matchData);
                break;
            case "POSTGAME":
                break;
        }
        // If State is PREGAME
        
        await UpdatePlayerControls(matchData);
    }
    

    private async Task SetupPregameState(AssistMatch matchData)
    {
        // Check the special state to see where the game currently is.
        if (matchData.SpecialState.Equals("TEAM1BAN") || matchData.SpecialState.Equals("TEAM2BAN"))
        {
            MapPickBanEnabled = true;
            
            // Determine If The Client Player is the current team banning
                // If so, Check if they are the captain.
                    // If so Enable The viewing of the ban button.
            
                    // Else Disable Ban Button
                    
            // Update and Start Timer, Update Banned Maps on List of Maps, Update Banned Maps on the bottom right (small square)
            
        }
        
        if (matchData.SpecialState.Equals("FORMING") || matchData.SpecialState.Equals("WAITING"))
        {
            MapPickBanEnabled = false;


            SetupMapNameAndServerName(matchData);
            
            // Check if PregameDetails Contains a PartyID
            if (string.IsNullOrEmpty(matchData.PregameDetails.ValorantPartyId))
            {
                MatchHeaderTitle = "Waiting...";
                JoinLobbyButtonEnabled = false;
                return;
            }
            
            
            // If So Enable the Join Lobby button.
            MatchHeaderTitle = "Ready Up!";
            ReadyLobbyButtonEnabled = true;
                
            // Once a player joins the lobby, unready them on valorant.
            // Bind to the Pres and Check on If they leave the party.
            // If they leave force them back, if they were kicked Report to server. DO THIS ALL IN THE MATCH SERVICE.
            // Once a player joins the lobby.  DO THIS WHEN THE PLAYER IS IN THE PARTY.
            // Showcase the ready button
            
            
        }
        
        
        
    }

    private async Task SetupIngameState(AssistMatch matchData)
    {
        
    }
    
    public async Task BindToEvents()
    {
        AssistApplication.Current.GameServerConnection.MATCH_MatchUpdateMessageReceived += MatchUpdateRecieved;
        AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent += PlayerPresenceMessageReceived;
        AssistApplication.Current.RiotWebsocketService.PregameMessageEvent += PregameMessageReceived;
    }
    

    public async Task UnbindToEvents()
    {
        AssistApplication.Current.GameServerConnection.MATCH_MatchUpdateMessageReceived -= MatchUpdateRecieved;
        AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent -= PlayerPresenceMessageReceived;
        AssistApplication.Current.RiotWebsocketService.PregameMessageEvent -= PregameMessageReceived;
    }
    
    
    private async Task CreatePlayerControls(AssistMatch matchData)
    {
        foreach (var player in matchData.TeamOne.Players)
        {
            var ctrl = new MatchPlayerControl()
            {
                PlayerId = player.Id,
                PlayerName = player.Username,
                LeaguePointText = $"{player.Points:n0} LP",
                ImageUrl = player.DisplayImage,
                IsReady = matchData.PregameDetails.ReadyPlayers != null && matchData.PregameDetails.ReadyPlayers.Contains(player.Id)
            };
            TeamAControls.Add(ctrl);
        }
        
        foreach (var player in matchData.TeamTwo.Players)
        {
            var ctrl = new MatchPlayerControl()
            {
                PlayerId = player.Id,
                PlayerName = player.Username,
                LeaguePointText = $"{player.Points:n0} LP",
                ImageUrl = player.DisplayImage,
                IsReady = matchData.PregameDetails.ReadyPlayers != null && matchData.PregameDetails.ReadyPlayers.Contains(player.Id)
            };
            TeamBControls.Add(ctrl);
        }
        
        // Setup Team names
        TeamAName = matchData.TeamOne.TeamName;
        TeamBName = matchData.TeamTwo.TeamName;
    }
    
    private async Task UpdatePlayerControls(AssistMatch matchData)
    {
        foreach (var player in matchData.TeamOne.Players)
        {
            var indexOfItem = TeamAControls.AsList().FindIndex(x => x.PlayerId == player.Id);

            var item = TeamAControls[indexOfItem];

            item.IsReady = matchData.PregameDetails.ReadyPlayers != null &&
                           matchData.PregameDetails.ReadyPlayers.Contains(player.Id);
            item.PlayerName = player.Username;
            item.ImageUrl = player.DisplayImage;
            item.LeaguePointText = $"{player.Points:n0} LP";
            
            TeamAControls[indexOfItem] = item;
        }
        
        foreach (var player in matchData.TeamTwo.Players)
        {
            var indexOfItem = TeamBControls.AsList().FindIndex(x => x.PlayerId == player.Id);

            var item = TeamBControls[indexOfItem];

            item.IsReady = matchData.PregameDetails.ReadyPlayers != null &&
                           matchData.PregameDetails.ReadyPlayers.Contains(player.Id);
            item.PlayerName = player.Username;
            item.ImageUrl = player.DisplayImage;
            item.LeaguePointText = $"{player.Points:n0} LP";
            
            TeamBControls[indexOfItem] = item;
        }
        
        // Setup Team names
        TeamAName = matchData.TeamOne.TeamName;
        TeamBName = matchData.TeamTwo.TeamName;
    }

    private async Task SetupMapNameAndServerName(AssistMatch matchData)
    {
        if (!string.IsNullOrEmpty(matchData.PregameDetails.SelectedServer))
            MatchServerName = ServerNames.ValorantServers[matchData.PregameDetails.SelectedServer.ToLower()];

        if (!string.IsNullOrEmpty(matchData.PregameDetails.SelectedMap))
        {
            MatchMapName = MapNames.MapsByPath[matchData.PregameDetails.SelectedMap.ToLower()].ToUpper();
            MatchMapImage =
                $"https://content.assistapp.dev/maps/{MapNames.MapsByPath[matchData.PregameDetails.SelectedMap.ToLower()]}_Featured.png";
        }
            
        
    }
    public async Task JoinOrReadyInMatch()
    {
        ReadyLobbyButtonEnabled = false; // Disable the button to prevent multiple requests.
        var pty = await AssistApplication.Current.CurrentUser.Party.FetchParty();
        
        if (!pty.ID.Equals(MatchService.Instance.CurrentMatchData.PregameDetails.ValorantPartyId))
        {
            Log.Error("Player is not in the right party!");
            Log.Error("Player is not in the right party!");
            Log.Error("Player is not in the right party!");
            Log.Error("Player is not in the right party!");
            ReadyLobbyButtonEnabled = true; // reenable the btn
            return;
        }

        try
        {
            var resp = await AssistApplication.Current.AssistUser.League.ReadyInPreMatch(MatchService.Instance.CurrentMatchData
                .Id);

            if (resp.Code != 200)
            {
                Log.Error("Failed to ready up in the pregame");
                Log.Error("Message: " + resp.Message);
                ReadyLobbyButtonEnabled = true; // reenable the btn
                return;
            }

            try
            {
                await AssistApplication.Current.CurrentUser.Party.SetPartyReadiness(true); // Set ready in party.
            }
            catch (Exception e)
            {
                Log.Error("We caught a bad exception when swapping ready status in the match.");
                Log.Error("Exception Message: " + e.Message);
                Log.Error("Exception Stack: " + e.StackTrace);
                ReadyLobbyButtonEnabled = true; // reenable the btn
                return;
            }

            ReadyLobbyButtonEnabled = false; // Disable the button to prevent multiple requests.
        }
        catch (Exception e)
        {
            Log.Error("We caught a bad exception within the ready process for the player in the match.");
            Log.Error("Exception Message: " + e.Message);
            Log.Error("Exception Stack: " + e.StackTrace);
            ReadyLobbyButtonEnabled = true; // reenable the btn
            return;
        }
        
        
        
    }
    
    
    
    /// <summary>
    /// Not Used.
    /// </summary>
    /// <param name="obj"></param>
    private void PregameMessageReceived(object obj)
    {
        
    }

    private async void PlayerPresenceMessageReceived(PresenceV4Message obj)
    {
        // Check if the player is in the party & is in a custom game.
        // Do a client check and a server check.

        var playerPre = obj.data.presences.Find(x => x.puuid == AssistApplication.Current.CurrentUser.UserData.sub);
        var privatePlayerPres = await GetPresenceData(playerPre);

        if (privatePlayerPres.partyId != MatchService.Instance.CurrentMatchData.PregameDetails.ValorantPartyId)
        {
            Log.Error("Player is currently in the wrong party than the specified match party");
            if (privatePlayerPres.sessionLoopState.Equals("PREGAME", StringComparison.OrdinalIgnoreCase) || privatePlayerPres.sessionLoopState.Equals("INGAME", StringComparison.OrdinalIgnoreCase))
            {
                Log.Fatal("Player is in a match while being in the wrong party. This is not supposed to happen.");
            }
            return;
        }
        
        Log.Information("Player is in the right party.");
        if (!privatePlayerPres.sessionLoopState.Equals("MENUS", StringComparison.OrdinalIgnoreCase))
        {
            Log.Fatal("Player is in a match while being in the right party.");
            Log.Fatal("Checking Match data to see if the correct players are in the match.");

            if (CurrentlyInMatch)
            {
                
                Log.Information("Already Determined if the player was in the match.");
                await UpdateIngameData(privatePlayerPres);
                return;
            }
            
            switch (privatePlayerPres.sessionLoopState)
            {
                case "PREGAME":
                    await HandlePregameMatchData();
                    break;
                case "INGAME":
                    break;
            }

           
        }
    }

    private async Task UpdateIngameData(PlayerPresence privatePlayerPres)
    {
            if (MatchService.Instance.CurrentMatchData.TeamOne.Players.Exists(x => x.ValorantId.Equals(AssistApplication.Current.CurrentUser.UserData.sub, StringComparison.OrdinalIgnoreCase)))
        {
            CurrentGameScoreTeamOne = $"{privatePlayerPres.partyOwnerMatchScoreAllyTeam}";
            CurrentGameScoreTeamTwo = $"{privatePlayerPres.partyOwnerMatchScoreEnemyTeam}";
        }
        else
        {
            CurrentGameScoreTeamOne = $"{privatePlayerPres.partyOwnerMatchScoreEnemyTeam}";
            CurrentGameScoreTeamTwo = $"{privatePlayerPres.partyOwnerMatchScoreAllyTeam}";
        }
        
        //TODO: Send Assist the Match Data and Presence Data
    }

    private async Task HandlePregameMatchData()
    {
        if (string.IsNullOrEmpty(_matchId))
        {
            GetMatchDetails();
        }
        Log.Error("Updating Data");
        PregameMatch MatchResp = new PregameMatch();
        ChatV4PresenceObj PresenceResp = new ChatV4PresenceObj();
        try
        {
            MatchResp = await AssistApplication.Current.CurrentUser.Pregame.GetMatch(_matchId!);
            PresenceResp = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
            if (MatchResp == null || PresenceResp == null)
            {
                Log.Error("Failed on getting update data. Match or Presence");
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
        
        
            var teamIds = MatchResp.AllyTeam.Players.Select(p => p.Subject).ToList();

            var allTeamPresences =
                PresenceResp.presences.FindAll(pres =>
                    teamIds.Contains(pres.puuid)); // Contains every presence from the team.
            
            Log.Information("Checking if all players on the team are on the Match Team");
            var possPlayer = MatchService.Instance.CurrentMatchData.TeamOne.Players.Find(x =>
                x.Id == AssistApplication.Current.AssistUser.Account.AccountInfo.id);
            List<AssistMatchPlayer> teamMates = new List<AssistMatchPlayer>();
            if (possPlayer is null)
                teamMates = MatchService.Instance.CurrentMatchData.TeamTwo.Players;
            else
                teamMates = MatchService.Instance.CurrentMatchData.TeamOne.Players;


            int teamMatesCorrect = 0;
            foreach (var teamMate in teamMates)
            {
                if (teamIds.Contains(teamMate.ValorantId))
                {
                    Log.Information($"Team Contains Teammate: {teamMate.Username}");
                    teamMatesCorrect += 1;
                }
                else
                {
                    Log.Information($"Teammate MISSING: {teamMate.Username}");
                    Log.Information($"Teammate MISSING: {teamMate.Username}");
                    Log.Information($"Teammate MISSING: {teamMate.Username}");
                }
            }


            if (teamMatesCorrect >= (MatchService.Instance.CurrentMatchData.TeamOne.Players.Count * .5))
            {
                Log.Information("More than half of the teamMates are correct on the team.");
                CurrentlyInMatch = true;
            }
    }

    private async void GetMatchDetails()
    {
        try
        {
            var getPlayerResp = await AssistApplication.Current.CurrentUser.Pregame.GetPlayer();
            if (getPlayerResp != null)
            {
                _matchId = getPlayerResp.MatchID;
            }

            if (string.IsNullOrEmpty(_matchId))
            {
                Log.Error("Failed to get MatchID");
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
                Log.Fatal("TOKEN ERROR: ");
                AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                       
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
}