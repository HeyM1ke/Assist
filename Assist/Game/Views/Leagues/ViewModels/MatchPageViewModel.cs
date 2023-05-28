using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Assist.Game.Controls.Live;
using Assist.Game.Controls.Match;
using Assist.Game.Controls.Navigation;
using Assist.Game.Services;
using Assist.Objects.Helpers;
using Assist.ViewModels;
using AssistUser.Lib.Leagues.Models;
using DynamicData.Kernel;
using ReactiveUI;

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


    private async Task UpdatePage(AssistMatch matchData)
    {
        // Check State
        switch (matchData.State)
        {
            case "PREGAME":
                await SetupPregameState(matchData);
                break;
            case "INGAME":
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
            JoinLobbyButtonEnabled = true;
            
            // Once a player joins the lobby, unready them on valorant.
            // Bind to the Pres and Check on If they leave the party.
            // If they leave force them back, if they were kicked Report to server. DO THIS ALL IN THE MATCH SERVICE.
            // Once a player joins the lobby.  DO THIS WHEN THE PLAYER IS IN THE PARTY.
            // Showcase the ready button
        }

        if (matchData.SpecialState.Equals("WAITING"))
        {
            // This means we are waiting for players to join and ready themselves. 
            
        }
        
        
        
    }

    private async Task BindToEvents()
    {
        
    }
    
    private async Task UnbindToEvents()
    {
        
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
        await MatchService.QuitIngameValorantMatch();
    }
}