using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Models;
using Assist.Game.Services;
using Assist.Game.Services.Leagues;
using Assist.Game.Views.Leagues;
using Assist.Game.Views.Leagues.Pages;
using Assist.ViewModels;
using AssistUser.Lib.Leagues;
using AssistUser.Lib.Leagues.Models;
using AssistUser.Lib.Profiles.Models;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;
using Serilog;
using ValNet.Enums;

namespace Assist.Game.Controls.Leagues.ViewModels;

public class LeagueSelectionViewModel : ViewModelBase
{
    private bool popupOpen = false;
    public bool PopupOpen
    {
        get => popupOpen;
        set => this.RaiseAndSetIfChanged(ref popupOpen, value);
    }
     
    
    private string _currentLeagueName;
    public string CurrentLeagueName
    {
        get => _currentLeagueName;
        set => this.RaiseAndSetIfChanged(ref _currentLeagueName, value);
    }
    
    private ObservableCollection<LeagueSelectionDropdownButton> _leagueSelectionControls = new ObservableCollection<LeagueSelectionDropdownButton>();
    public ObservableCollection<LeagueSelectionDropdownButton> LeagueSelectionControls
    {
        get => _leagueSelectionControls;
        set => this.RaiseAndSetIfChanged(ref _leagueSelectionControls, value);
    }

    public static List<AssistLeague> LeagueData = new List<AssistLeague>();

    private string _errorMessage = string.Empty;
    public async Task Setup()
    {
        new LeagueService();
        
        // Set name of Current League using League Service
        await LeagueService.Instance.GetProfileData();
        
        // Check for Errors on loading leagues.
        var allG = await CheckForLeagueErrors();

        if (!allG) 
        {
            // Show Leagues Error Screen with Message.
            GameViewNavigationController.Change(new LeaguesErrorPage(_errorMessage));
            return;
        }
        
        var leagueIds = LeagueService.Instance.ProfileData.Leagues;
        if (leagueIds.Count <= 0)
        {
            Log.Information("No Leagues found in the user.");
            return;
        }
        
        if (!string.IsNullOrEmpty(LeagueService.Instance.CurrentLeagueId))
        {
            var currentLeagueData = LeagueData.Find(ob => ob.Id == LeagueService.Instance.CurrentLeagueId);
            if (currentLeagueData is null)
            {
                Log.Information("Making request to get league information for current ");
                var data = await AssistApplication.Current.AssistUser.League.GetLeagueInfo(LeagueService.Instance.CurrentLeagueId);

                if (data.Code == 200)
                {
                    var d = JsonSerializer.Deserialize<AssistLeague>(data.Data.ToString());
                    LeagueData.Add(d);
                }
                else
                {
                    Log.Fatal("Failed to Get league data.");    
                    Log.Fatal(data.Message);
                }
            }
            
            currentLeagueData = LeagueData.Find(ob => ob.Id == LeagueService.Instance.CurrentLeagueId);
            
            // Set up the name
            CurrentLeagueName = currentLeagueData.Name;

            LeagueSelectionControls.Add(CreateBtn(currentLeagueData.Id,currentLeagueData.Name));
        }
        else
        {
            LeagueService.Instance.CurrentLeagueId = leagueIds[0].Id;
            
            var currentLeagueData = LeagueData.Find(ob => ob.Id == leagueIds[0].Id);

            if (currentLeagueData is null)
            {
                // Data has already been stored. 
                var data = await AssistApplication.Current.AssistUser.League.GetLeagueInfo(leagueIds[0].Id);

                if (data.Code == 200)
                {
                    var d = JsonSerializer.Deserialize<AssistLeague>(data.Data.ToString());
                    LeagueData.Add(d);
                    currentLeagueData = d;
                }
                else
                {
                    Log.Fatal("Failed to Get league data.");
                    Log.Fatal(data.Message);    
                }
            }
            CurrentLeagueName = currentLeagueData.Name;
            LeagueSelectionControls.Add(CreateBtn(currentLeagueData.Id,currentLeagueData.Name));
        }

        for (int i = 0; i < leagueIds.Count; i++)
        {
            if (leagueIds[i].Id == LeagueService.Instance.CurrentLeagueId) continue;
            
            var currentLeagueData = LeagueData.Find(ob => ob.Id == leagueIds[i].Id);

            if (currentLeagueData is null)
            {
                // Data has already been stored. 
                var data = await AssistApplication.Current.AssistUser.League.GetLeagueInfo(leagueIds[i].Id);

                if (data.Code == 200)
                {
                    var d = JsonSerializer.Deserialize<AssistLeague>(data.Data.ToString());
                    LeagueData.Add(d);
                    currentLeagueData = d;
                }
                else
                {
                    Log.Fatal("Failed to Get league data.");
                    Log.Fatal(data.Message);    
                }
            }

            if (currentLeagueData is null) continue;
            
            LeagueSelectionControls.Add(CreateBtn(currentLeagueData.Id,currentLeagueData.Name));
        }
        
        LeagueSelectionControls.Add(CreateBtn("0", "Find a League."));

        if (leagueIds.Count == 0)
        {
            LeagueNavigationController.Change(new LeagueFinderPage());
            return;
        }
        
        // Load the page
        LeagueNavigationController.Change(new LeaguesPage());
        
    }

    

    private LeagueSelectionDropdownButton CreateBtn(string leagueId, string leagueName)
    {
        var btn = new LeagueSelectionDropdownButton()
        {
            LeagueId = leagueId,
            LeagueName = leagueName,
            Width = 400
        };

        if (leagueId != "0")
            btn.Click += SelectionDropdownBtn_Click;
        else
            btn.Click += SelectionDropdownBtn_Click_LeagueFinder;

        return btn;
    }
    private void SelectionDropdownBtn_Click(object? sender, RoutedEventArgs e)
    {
        Log.Information("Changing League");
        var btn = sender as LeagueSelectionDropdownButton;
        if (btn.LeagueId == LeagueService.Instance.CurrentLeagueId)
        {
            Log.Information("Cannot Swap to the current league.");
            return;
        }

        LeagueService.Instance.CurrentLeagueId = btn.LeagueId;
        CurrentLeagueName = btn.LeagueName;
        
        // Load the page
        LeagueNavigationController.Change(new LeaguesPage());

    }
    
    private void SelectionDropdownBtn_Click_LeagueFinder(object? sender, RoutedEventArgs e)
    {
        Log.Information("Changing to League Finder.");
        
        var btn = sender as LeagueSelectionDropdownButton;
        if (btn.LeagueId == LeagueService.Instance.CurrentLeagueId)
        {
            Log.Information("Cannot Swap to the current league.");
            return;
        }
        LeagueService.Instance.CurrentLeagueId = "0";
        CurrentLeagueName = "League Finder";

        // Load the page
        LeagueNavigationController.Change(new LeagueFinderPage());

    }
    
    /// <summary>
    /// Checks for Errors in regards to loading up leagues.
    /// </summary>
    /// <returns>true if there are no errors</returns>
    private async Task<bool> CheckForLeagueErrors()
    {
        // Check if the player is currently have valorant open
        if (!IsValorantRunning())
        {
            _errorMessage = "Valorant is not open. Please reopen Leagues, when VALORANT is open.";
            return false;
        }

        // Check if The Player is authenticated with a socket.
        if (AssistApplication.Current.CurrentUser.Authentication.AuthType != EAuthType.LOCAL)
        {
            _errorMessage = "Something went wrong, please reload Assist.";
            return false;
        }
        
        
        // Check if there is a connection to the GameServer
        if (!AssistApplication.Current.GameServerConnection.IsConnected)
        {
            _errorMessage = "Please restart your Assist Client.";
            return false;
        }
        
        // Check if there is an Assist User Logged in
        if (AssistApplication.Current.AssistUser.Account.AccountInfo is null)
        {
            _errorMessage = "You are not logged into an Assist Account.";
            return false;
        }
        
        
        
        // Check if a player has a riot account linked
        if (LeagueService.Instance.ProfileData.LinkedRiotAccounts.Count <= 0)
        {
            _errorMessage = "Please link a Riot Account to your Assist Account in Settings.";
            return false;
        }
        // Check if the current authenticated account is the same one tied to the assist account.
        if (!LeagueService.Instance.ProfileData.LinkedRiotAccounts[0].Id
                .Equals(AssistApplication.Current.CurrentUser.UserData.sub))
        {
            _errorMessage = "Please launch Assist with the same account you have linked to your assist account.";
            return false;
        }
        // Check if the player is Pregame or in-game, if so dont allow loading.
        var playerPres = await GetCurrentPlayerPres();
        
        // Request Match Endpoint to see if there is a match.
        var p = await AssistApplication.Current.AssistUser.League.PREMATCH_GetUserMatch();
        if (p.Code == 200)
        {
            GameViewNavigationController.Change(new MatchPage());
            return true;
        }
        p = await AssistApplication.Current.AssistUser.League.MATCH_GetUserMatch();
        if (p.Code == 200)
        {
            GameViewNavigationController.Change(new MatchPage());
            return true;
        }
        
        if (!playerPres.sessionLoopState.Equals("MENUS", StringComparison.OrdinalIgnoreCase))
        {
            _errorMessage = "You are currently in a game, please leave your game to play leagues.";
            return false;
        }
        
        return true;
    }
    
    private bool IsValorantRunning()
    {
        var processlist = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Where(process => process.Id != Process.GetCurrentProcess().Id).ToList();
        processlist.AddRange(Process.GetProcessesByName("VALORANT-Win64-Shipping"));

        return processlist.Any();
    }
    private async Task<PlayerPresence> GetCurrentPlayerPres()
    {
        var p = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
        var pP= p.presences.Find(x => x.puuid.Equals(AssistApplication.Current.CurrentUser.UserData.sub));
        if(string.IsNullOrEmpty(pP.Private))
            return new PlayerPresence();
        byte[] stringData = Convert.FromBase64String(pP.Private);
        string decodedString = Encoding.UTF8.GetString(stringData);
        return JsonSerializer.Deserialize<PlayerPresence>(decodedString);
    }
}