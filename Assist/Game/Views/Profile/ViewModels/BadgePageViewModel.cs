using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Controls.Profile;
using Assist.ViewModels;
using AssistUser.Lib.Profiles.Models;
using Avalonia.Interactivity;
using ReactiveUI;
using Serilog;

namespace Assist.Game.Views.Profile.ViewModels;

public class BadgePageViewModel : ViewModelBase
{
    
    private string _badgeName;

    public string BadgeName
    {
        get => _badgeName;
        set => this.RaiseAndSetIfChanged(ref _badgeName, value);
    }
    
    private string _badgeImage;
    public string BadgeImage
    {
        get => _badgeImage;
        set => this.RaiseAndSetIfChanged(ref _badgeImage, value);
    }
    
    private string _badgeEarnedDate;
    public string BadgeEarnedDate
    {
        get => _badgeEarnedDate;
        set => this.RaiseAndSetIfChanged(ref _badgeEarnedDate, value);
    }
    
    private string _featuredButtonText;
    public string FeaturedButtonText
    {
        get => _featuredButtonText;
        set => this.RaiseAndSetIfChanged(ref _featuredButtonText, value);
    }
    
    private bool _featuredButtonVisable = false;
    public bool FeaturedButtonVisable
    {
        get => _featuredButtonVisable;
        set => this.RaiseAndSetIfChanged(ref _featuredButtonVisable, value);
    }

    private List<ProfileBadgeShowcase> _badgeShowcases;

    public List<ProfileBadgeShowcase> BadgeShowcases
    {
        get => _badgeShowcases;
        set => this.RaiseAndSetIfChanged(ref _badgeShowcases, value);
    }

    public string CurrentShowcasedId;
    public bool TrueToSet = false;
    public async Task Setup()
    {
        var badges = new List<ProfileBadgeShowcase>();
        
        if (ProfilePageViewModel.ProfileData.FeaturedBadges.Count > 0)
        {
            for (int i = 0; i < ProfilePageViewModel.ProfileData.FeaturedBadges.Count; i++)
            {
                var bdge = new ProfileBadgeShowcase()
                {
                    IsFeatured = true,
                    BadgeId = ProfilePageViewModel.ProfileData.FeaturedBadges[i].Id,
                    BadgeImageUrl =
                        $"https://content.assistapp.dev/badges/{ProfilePageViewModel.ProfileData.FeaturedBadges[i].Id}.png"
                };
                
                bdge.Click += ShowcaseBadge_OnClick;

                badges.Add(bdge);  
            }
        }


        if (ProfilePageViewModel.ProfileData.OwnedBadges.Count > 0)
        {
            for (int i = 0; i < ProfilePageViewModel.ProfileData.OwnedBadges.Count; i++)
            {
                if (ProfilePageViewModel.ProfileData.FeaturedBadges.Find(bdg => bdg.Id == ProfilePageViewModel.ProfileData.OwnedBadges[i].Id) is not null) continue;

                var bdge = new ProfileBadgeShowcase()
                {
                    IsFeatured = false,
                    BadgeId = ProfilePageViewModel.ProfileData.OwnedBadges[i].Id,
                    BadgeImageUrl =
                        $"https://content.assistapp.dev/badges/{ProfilePageViewModel.ProfileData.OwnedBadges[i].Id}.png",
                };
                
                bdge.Click += ShowcaseBadge_OnClick;
                
                badges.Add(bdge);  
            }
        }

        BadgeShowcases = badges;
    }

    private void ShowcaseBadge_OnClick(object? sender, RoutedEventArgs e)
    {
        var badgeControlData = sender as ProfileBadgeShowcase; // Convert the object to the badge

        var badgeData = ProfilePageViewModel.ProfileData.OwnedBadges.Find(bdge => bdge.Id == badgeControlData.BadgeId);

        if (badgeData is null)
        {
            Log.Error("Cannot find Badge Data!");
        }

        BadgeImage = badgeControlData.BadgeImageUrl;
        BadgeName = badgeData.Title;
        CurrentShowcasedId = badgeData.Id;
        BadgeEarnedDate = $"{Properties.Resources.Profile_Badge_EarnedAt} {badgeData.EarnedAt.ToShortDateString()}";
        FeaturedButtonVisable = true;
        if (badgeControlData.IsFeatured)
        {
            FeaturedButtonText = Properties.Resources.Profile_Badge_RemoveFeatured;
            TrueToSet = false;
        }
        else
        {
            FeaturedButtonText = Properties.Resources.Profile_Badge_SetFeatured;
            TrueToSet = true;
        }
        
    }

    public async Task UpdateFeaturedBadge(object? sender)
    {
        if (TrueToSet)
        {
            var t = await AssistApplication.Current.AssistUser.Profile.AddFeaturedBadge(CurrentShowcasedId);

            if (t.Code != 200)
            {
                Log.Error("ERROR SETTING FEATURED BADGE");
                Log.Error(t.Message);
                return;
            }
            
            Log.Information("FEATURED BADGE SET");

            FeaturedButtonVisable = false;
            BadgeName = string.Empty;
            BadgeEarnedDate = string.Empty;
            BadgeImage = string.Empty;

            
        }
        else
        {
            var t = await AssistApplication.Current.AssistUser.Profile.RemoveFeaturedBadge(CurrentShowcasedId);

            if (t.Code != 200)
            {
                Log.Error("ERROR REMOVING FEATURED BADGE");
                Log.Error(t.Message);
                return;
            }
            
            Log.Information("FEATURED BADGE REMOVED");

            FeaturedButtonVisable = false;
            BadgeName = string.Empty;
            BadgeEarnedDate = string.Empty;
            BadgeImage = string.Empty;
            
        }

        var resp = await AssistApplication.Current.AssistUser.Profile.GetProfile();
        if (resp.Code != 200)
        {
            Log.Error("Bad Request on Profile Get");
            Log.Error(resp.Message);
            await Setup();
            return;
        }

        ProfilePageViewModel.ProfileData = JsonSerializer.Deserialize<AssistProfile>(resp.Data.ToString());
         
        await Setup();
    }
}