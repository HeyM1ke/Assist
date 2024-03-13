using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Assist.Shared.Services.Utils;
using Assist.Shared.Settings.Accounts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using ValNet;
using ValNet.Enums;
using ValNet.Objects;
using ValNet.Objects.Authentication;
using ValNet.Objects.Exceptions;

namespace Assist.ViewModels.RAccount;

public partial class RAccountUsernameLoginViewModel : ViewModelBase
{
    
    [ObservableProperty] private ICommand? _loginButtonCommand;
    [ObservableProperty] private ICommand? _loginCompletedCommand;
    [ObservableProperty] private string _passwordText = "";
    [ObservableProperty] private string _usernameText = "";
    [ObservableProperty] private string _multifactorText = "";
    [ObservableProperty] private bool _showLaunchAuthChoice = false;
    [ObservableProperty] private bool _isProcessing = false;
    [ObservableProperty] private bool _multiFactorEnabled = false;
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private bool _errorMessageVisible = false;

    
    [RelayCommand]
    private async Task LoginButtonPressed()
    {
        Log.Information("Logging in with Riot Account with Username/Password");
        
        IsProcessing = true;
/*#if DEBUG
        Log.Information("Riot Login Username: " + UsernameText);
        Log.Information("Riot Login Password: " + PasswordText);
#endif*/

        await LoginWithRiotUsername();
    }
    
    [RelayCommand]
    private async Task SubmitButtonPressed()
    {
        Log.Information("Multifactor Code was submitted.");
        
        IsProcessing = true;
/*#if DEBUG
        Log.Information("Riot Login Username: " + UsernameText);
#endif*/

        await HandleMultifactorAuthentication();
    }
    private RiotUser usr;
    
    private async Task LoginWithRiotUsername()
    {
        Log.Information("Attempting to login with Riot Account with Username/Password");
        string curlPath = Path.Exists(Path.Combine(DependencyUtils.CurlDependencyFolder, "curl.exe")) ? Path.Combine(DependencyUtils.CurlDependencyFolder, "curl.exe") : "curl";
        usr = new RiotUserBuilder().WithCredentials(new RiotLogin()
        {
            password = PasswordText,
            username = UsernameText
        }).WithCustomCurl(curlPath).WithSettings(new RiotUserSettings()
        {
            AuthenticationMethod = AuthenticationMethod.CURL
        }).Build();

        try
        {
           var result =  await usr.Authentication.AuthenticateWithCloud();
           if (result.IsAuthenticationSuccessful == false)
           {
               if (result.type != null && result.type.Equals("multifactor"))
               {
                   MultiFactorEnabled = true;
                   IsProcessing = false;
                   return;
               }
           }
        }
        catch (Exception e)
        {
            Log.Error("Failed to Login with Username/Password");
            Log.Error(e.Message);
            Log.Error(e.Source);
            Log.Error(e.StackTrace);

            ChangeErrorMessage(e.Message);
            return;
        }
        
        
        await HandleSuccessfulLogin(usr);
    }
    
    private async Task HandleMultifactorAuthentication()
    {
        Log.Information("Attempting to login with Riot Account with Username/Password");
        AuthenticationResult r;
        try
        {
            r  = await usr.Authentication.AuthenticateTwoFactorCode(MultifactorText);
            if (!r.IsAuthenticationSuccessful)
            {
                ChangeErrorMessage("Multifactor Code Failed");
                return;
            }
        }
        catch (ValNetException ex)
        {
            ChangeErrorMessage(ex.Message);
            Log.Error("Error Message: " + ex.Message);
            if (ex is RequestException e)
            {
                Log.Error("Status Code: " + e.StatusCode);
                Log.Error("Content: " + e.Content);
            }

            return;
        }
        
        await HandleSuccessfulLogin(usr);
    }
    
    private async Task HandleSuccessfulLogin(RiotUser usr)
    {
        Log.Information("Successful login with Riot Account with Username/Password");
        AccountProfile profile = new AccountProfile();
        if (AccountSettings.Default.Accounts.Exists(x => x.Id == usr.UserData.sub))
            profile = AccountSettings.Default.Accounts.Find(x => x.Id == usr.UserData.sub);
        
        try
        {
            profile.Id = usr.UserData.sub;
            profile.Username = UsernameText;
            profile.Region = usr.GetRegion();
            profile.LastLoginTime = DateTime.UtcNow;
            profile.Personalization = new AccountProfile.AccountProfilePersonalization()
            {
                GameName = usr.UserData.acct.game_name,
                TagLine = usr.UserData.acct.tag_line
            };
            try
            {
                var inventory = await usr.Inventory.GetPlayerInventory();
                profile.Personalization.PlayerCardId = inventory.PlayerData.PlayerCardID;
                profile.Personalization.PlayerLevel = inventory.PlayerData.AccountLevel;
            }
            catch (Exception e)
            {
                Log.Error("Failed to Get Inventory Data when setting up profile");
            }
            
            try
            {
                var pMmr = await usr.Player.GetPlayerMmr();
                profile.Personalization.ValRankTier = pMmr.LatestCompetitiveUpdate.TierAfterUpdate;
            }
            catch (Exception e)
            {
                Log.Error("Failed to Get MMR Data when setting up profile");
            }
            profile.CanAssistBoot = true;
            profile.CanLauncherBoot = false;
            profile.IsExpired = false;
            profile.ConvertCookiesTo64(usr.GetAuthClient().ClientCookies);
            
        }
        catch (Exception e)
        {
            Log.Error("Failed to Setup Account");
            Log.Error(e.Message);
            Log.Error(e.Source);
            Log.Error(e.StackTrace);

            ChangeErrorMessage(e.Message);
            return;
        }

        AssistApplication.ActiveUser = usr;
        AssistApplication.ActiveAccountProfile = profile;

        AccountSettings.Default.DefaultAccount = string.IsNullOrEmpty(AccountSettings.Default.DefaultAccount) ? profile.Id : AccountSettings.Default.DefaultAccount;
        await AccountSettings.Default.UpdateAccount(profile);
        AccountSettings.Save();
        
        Log.Information("Finished Setting up Riot Account as the Main User & To the settings.");
        LoginCompletedCommand?.Execute("");
    }
    
    private void ChangeErrorMessage(string message)
    {
        ErrorMessageVisible = true;
        ErrorMessage = message;
        IsProcessing = false;
    }
}