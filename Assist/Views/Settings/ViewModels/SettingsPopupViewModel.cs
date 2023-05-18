using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Views.Profile.ViewModels;
using Assist.ViewModels;
using AssistUser.Lib.Profiles.Models;
using ReactiveUI;
using Serilog;

namespace Assist.Views.Settings.ViewModels;

public class SettingsPopupViewModel : ViewModelBase
{
    public bool IsAssistSignedIn => AssistApplication.Current.AssistUser.Account.AccountInfo is not null;
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

    public async Task SetupSignedInGrid()
    {
        if (!IsAssistSignedIn)
            return;
        
        var resp = await AssistApplication.Current.AssistUser.Profile.GetProfile();

        if (resp.Code != 200)
        {
            Log.Error("Bad Request on Profile Get");
            Log.Error(resp.Message);
        }

        ProfilePageViewModel.ProfileData = JsonSerializer.Deserialize<AssistProfile>(resp.Data.ToString());
        
        DisplayName = ProfilePageViewModel.ProfileData.DisplayName;
        DisplayImage = ProfilePageViewModel.ProfileData.ProfileImage;
        AccountEmail = AssistApplication.Current.AssistUser.Account.AccountInfo.email;
    }
}