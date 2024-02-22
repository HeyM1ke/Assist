using System;
using System.Linq;
using System.Windows.Input;
using Assist.Shared.Settings.Accounts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Assist.ViewModels.ProfileSwap;

public partial class ProfileManagementViewModel : ViewModelBase
{
    [ObservableProperty] private string _profileId;
    [ObservableProperty] private ICommand? _closePopup;

    [ObservableProperty] private string _profilePlayercard;
    [ObservableProperty] private string _profileRiotName;
    
    [ObservableProperty] private bool _assistEnabled;
    [ObservableProperty] private bool _gameLaunchEnabled;
    [ObservableProperty] private bool _accountExpired;
    [ObservableProperty] private bool _defaultAccount;
    public void Setup()
    {
        var profile = AccountSettings.Default.Accounts.FirstOrDefault(x => x.Id == ProfileId);

        ProfileRiotName = profile.Personalization.RiotId;
        ProfilePlayercard =  $"https://content.assistapp.dev/playercards/{profile.Personalization.PlayerCardId}_DisplayIcon.png";
        GameLaunchEnabled = profile.CanLauncherBoot;
        AssistEnabled = profile.CanAssistBoot;
        AccountExpired = profile.IsExpired;
        DefaultAccount = AccountSettings.Default.DefaultAccount.Equals(ProfileId, StringComparison.OrdinalIgnoreCase);
    }

    public void MakeAccountDefault()
    {
        AccountSettings.Default.DefaultAccount = ProfileId;
        AccountSettings.Save();
    }
}
