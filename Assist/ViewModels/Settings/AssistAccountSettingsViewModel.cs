using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Assist.Controls.Assist.Account;
using Assist.Services.Assist;
using Assist.Shared.Settings;
using Assist.Views.Assist;
using AssistUser.Lib.Base.Exceptions;
using AssistUser.Lib.V2;
using AssistUser.Lib.V2.Models;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Assist.ViewModels.Settings;

public partial class AssistAccountSettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _currentlyLoggedIn = !AssistApplication.AssistUser.Authentication.Roles.IsNullOrEmpty();

    [ObservableProperty] 
    private bool _failedToGetInfo = false;

    [ObservableProperty] private bool _canChangeDisplayName = false;
    [ObservableProperty] private string _displayName;
    [ObservableProperty] private string _displayImage;
    [ObservableProperty] private string _emailText;

    private AAccount? _accountInfo = null;
    
    public async Task SetupPage()
    {
        if (!CurrentlyLoggedIn) { return; }

        await GetAccountInfo();

        if (_accountInfo is null) return;

        DisplayName = _accountInfo.Personalization.DisplayName;
        DisplayImage = _accountInfo.Personalization.AvatarUrl;
        HandleEmailText();
        
        CanChangeDisplayName = _accountInfo.Personalization.LastDisplaySet.AddDays(14) < DateTime.UtcNow;
    }


    private void HandleEmailText()
    {
        string pattern = @"(?<=[\w]{1})[\w\-._\+%]*(?=[\w]{1}@)";
        string result = Regex.Replace(_accountInfo.Email, pattern, m => new string('*', m.Length));
        EmailText = result;
    }
    
    private async Task GetAccountInfo()
    {
        try
        {
            var accountInfo = await AssistApplication.AssistUser.Account.GetAccountInfo();

            if (accountInfo.Code != 200)
            {
                FailedToGetInfo = true;
                return;
            }
            
            _accountInfo = JsonSerializer.Deserialize<AAccount>(accountInfo.Data.ToString());
        }
        catch (RequestException e)
        {
            Log.Information("Failed to get User Account Info");
            Log.Error(e.Message);
            Log.Error($"Status Code: " + e.StatusCode);
            FailedToGetInfo = true;   
            return;
        }
    }

    [RelayCommand]
    public async void OpenChangeDisplayName()
    {
        Log.Information("Player Requested to Open Change Displayname Page");
        
        
        Dispatcher.UIThread.Invoke(() =>
        {
            AssistApplication.ChangeMainWindowPopupView(new CustomizeAssistDisplayNameControl(CompletedDisplayNameCommand, ClosePopupCommand));
        });
    }

    [RelayCommand]
    private async void ClosePopup()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            AssistApplication.ChangeMainWindowPopupView(null);
        });
    }
    
    [RelayCommand]
    private async void CompletedDisplayName()
    {
        await this.SetupPage();
        Dispatcher.UIThread.Invoke(() =>
        {
            AssistApplication.ChangeMainWindowPopupView(null);
        });
        
        
    }

    [RelayCommand]
    public async void OpenAssistLoginPopup()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            AssistApplication.ChangeMainWindowPopupView(new AssistAuthenticationView());
        });
    }
    
    [RelayCommand]
    public async void HandleAssistLogout() // TODO: Move this to a centralized Location
    { 
        Log.Information("User Requested to Logout");

        AssistApplication.AssistUser = new AUserBuilder().Build();
        AssistSettings.Default.SaveAssistUserCode(string.Empty);
        new DodgeService();
        //TODO: Terminate Server Connections
        
       CurrentlyLoggedIn = !AssistApplication.AssistUser.Authentication.Roles.IsNullOrEmpty();
    }
}