using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Assist.Game.Views.Profile.ViewModels;
using Assist.ViewModels;
using AssistUser.Lib.Profiles.Models;
using Avalonia.Controls.Notifications;
using ReactiveUI;
using Serilog;

namespace Assist.Views.Settings.ViewModels;

public class SettingsPopupViewModel : ViewModelBase
{
    public bool IsAssistSignedIn => AssistApplication.Current.AssistUser.Account.AccountInfo is not null;
    private bool _riotAccountLinked = true;

    public bool RiotAccountLinked
    {
        get => _riotAccountLinked;
        set => this.RaiseAndSetIfChanged(ref _riotAccountLinked, value);
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
    
    private string _accountEmail;
    public string AccountEmail
    {
        get => _accountEmail;
        set => this.RaiseAndSetIfChanged(ref _accountEmail, value);
    }
    
    private string _accountCreated;
    public string AccountCreated
    {
        get => _accountCreated;
        set => this.RaiseAndSetIfChanged(ref _accountCreated, value);
    }
    
    private string _linkedAccountText;

    public string LinkedAccountText
    {
        get => _linkedAccountText;
        set => this.RaiseAndSetIfChanged(ref _linkedAccountText, value);
    }

    public async Task SetupSignedInGrid()
    {
        if (!IsAssistSignedIn)
            return;
        
        var resp = await AssistApplication.Current.AssistUser.Profile.GetProfile();

        if (resp.Code != 200)
        {
            Log.Error("Bad Request on Profile Get");
            Log.Error(resp.Message);
            return;
        }

        ProfilePageViewModel.ProfileData = JsonSerializer.Deserialize<AssistProfile>(resp.Data.ToString());
        
        DisplayName = ProfilePageViewModel.ProfileData.DisplayName;
        DisplayImage = ProfilePageViewModel.ProfileData.ProfileImage;
        string emailHidePattern = @"(?<=[\w]{1})[\w\-._\+%]*(?=[\w]{1}@)";
        string hiddenEmail = Regex.Replace(AssistApplication.Current.AssistUser.Account.AccountInfo.email, emailHidePattern, m => new string('*', m.Length));
        
        AccountEmail = hiddenEmail;
        RiotAccountLinked = ProfilePageViewModel.ProfileData.LinkedRiotAccounts.Count > 0;

        if (RiotAccountLinked)
            LinkedAccountText =
                $"{ProfilePageViewModel.ProfileData.LinkedRiotAccounts[0].Gamename}#{ProfilePageViewModel.ProfileData.LinkedRiotAccounts[0].TagLine}";
        else
            LinkedAccountText = "No account linked.";
    }
}