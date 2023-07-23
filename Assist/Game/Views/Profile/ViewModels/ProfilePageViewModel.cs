using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Controls.Profile;
using Assist.ViewModels;
using AssistUser.Lib.Leagues.Models;
using AssistUser.Lib.Profiles.Models;
using ReactiveUI;
using Serilog;

namespace Assist.Game.Views.Profile.ViewModels;

public class ProfilePageViewModel : ViewModelBase
{
    public static AssistProfile ProfileData; // Static Data to not use multiple requests across different pages. Only refreshes on Profile Page on Reopen.

    public async Task Setup()
    {
        await UpdateProfileData();

        await GenerateContent();
    }

    private async Task GenerateContent()
    {
        DisplayName = ProfileData.DisplayName;
        DisplayImage = ProfileData.ProfileImage;
        DisplayStatus = ProfileData.Status;

        if (ProfileData.Leagues.Count > 0)
        {
            var listOfLeagues = new List<ProfileLeagueShowcase>();
            for (int i = 0; i < ProfileData.Leagues.Count; i++)
            {
                var data = await AssistApplication.Current.AssistUser.League.GetLeagueInfo(ProfileData.Leagues[i].Id);

                if (data.Code != 200)
                {
                    Log.Fatal("Failed to Get league data.");
                    Log.Fatal(data.Message);
                }
                var d = JsonSerializer.Deserialize<AssistLeague>(data.Data.ToString());
                
                
                listOfLeagues.Add(new ProfileLeagueShowcase()
                {
                    LeagueName = d.Name,
                    LeagueStatText = $"{ProfileData.Leagues[i].CurrentLeaguePoints} LP - {ProfileData.Leagues[i].Matches.Count} Matches"
                });
            }

            LeagueShowcases = listOfLeagues;
        }
    }

    public static async Task UpdateProfileData()
    {
        var resp = await AssistApplication.Current.AssistUser.Profile.GetProfile();

        if (resp.Code != 200)
        {
            Log.Error("Bad Request on Profile Get");
            Log.Error(resp.Message);
        }

        ProfileData = JsonSerializer.Deserialize<AssistProfile>(resp.Data.ToString());
    }
    
    private string _displayName;

    public string DisplayName
    {
        get => _displayName;
        set => this.RaiseAndSetIfChanged(ref _displayName, value);
    }
    
    private string _displayImage;
    public string DisplayImage
    {
        get => _displayImage;
        set => this.RaiseAndSetIfChanged(ref _displayImage, value);
    }
    
    private string _displayStatus;
    public string DisplayStatus
    {
        get => _displayStatus;
        set => this.RaiseAndSetIfChanged(ref _displayStatus, value);
    }

    private List<ProfileLeagueShowcase> _leagueShowcases;

    public List<ProfileLeagueShowcase> LeagueShowcases
    {
        get => _leagueShowcases;
        set => this.RaiseAndSetIfChanged(ref _leagueShowcases, value);
    }

    
}