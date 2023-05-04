using System;
using System.Threading.Tasks;
using Assist.Services;
using Assist.Services.Popup;
using Assist.Settings;
using Assist.ViewModels;
using Assist.Views.Authentication;
using ReactiveUI;
using ValNet.Enums;

namespace Assist.Controls.Global.ViewModels;

public class AccountManagementUserButtonViewModel : ViewModelBase
{

    public string RankIcon
    {
        get => $"https://content.assistapp.dev/ranks/TX_CompetitiveTier_Large_{Profile.ValRankTier}.png";
    }
    
    public string ProfilePicture
    {
        get => $"https://cdn.assistapp.dev/PlayerCards/{Profile.PlayerCardId}_DisplayIcon.png";
    }
    public bool isExpired => _profile.isExpired;
    public bool IsDefault => AssistSettings.Current.DefaultAccount == Profile.ProfileUuid;
    public string Username => _profile.RiotId;
#pragma warning disable CS8603 // Possible null reference return.
    public string Region => Enum.GetName(typeof(RiotRegion), _profile.Region);
#pragma warning restore CS8603 // Possible null reference return.

    private ProfileSettings _profile = new ProfileSettings()
    {
        Gamename = "0000000000000000",
        Tagline = "00000",
        Region = RiotRegion.UNKNOWN
    };

    public ProfileSettings Profile
    {
        get => _profile;
        set => this.RaiseAndSetIfChanged(ref _profile, value);
    }

    public async Task SetDefault()
    {
        AssistSettings.Current.DefaultAccount = Profile.ProfileUuid;
    }

    public async Task RemoveProfile()
    {
        var r = AssistSettings.Current.Profiles.Remove(Profile);

        if (AssistSettings.Current.Profiles.Count == 0)
        {
            MainWindowContentController.Change(new AuthenticationView());
            PopupSystem.KillPopups();
            return;
        }

        if (AssistApplication.Current.CurrentProfile.ProfileUuid != Profile.ProfileUuid)
            return;

        AssistApplication.Current.SwapCurrentProfile(AssistSettings.Current.Profiles[0], true);
    }

    public async Task SwapProfile()
    {
        if (AssistApplication.Current.CurrentProfile.ProfileUuid != Profile.ProfileUuid)
        {
            AssistApplication.Current.SwapCurrentProfile(Profile);    
        }
        
        
    }
}