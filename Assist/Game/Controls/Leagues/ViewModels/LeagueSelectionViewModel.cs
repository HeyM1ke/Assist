using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Services.Leagues;
using Assist.Game.Views.Leagues;
using Assist.ViewModels;
using AssistUser.Lib.Leagues;
using AssistUser.Lib.Leagues.Models;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;
using Serilog;

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

    public async Task Setup()
    {
        new LeagueService();
        
        // Set name of Current League using League Service
        await LeagueService.Instance.GetProfileData();
        
        
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
            
        btn.Click += SelectionDropdownBtn_Click;

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
}