using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Assist.Controls.Navigation;
using Assist.Controls.Startup;
using Assist.Services.Navigation;
using Assist.Shared.Services.Utils;
using Assist.Shared.Settings.Accounts;
using Assist.ViewModels.Navigation;
using Assist.Views.RAccount;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using ValNet;
using ValNet.Enums;
using ValNet.Objects;

namespace Assist.ViewModels.ProfileSwap;

public partial class SwapPageViewModel : ViewModelBase
{
    [ObservableProperty] private Control _currentContent;
    
    public string ProfileId { get; set; }

    public async Task SwapProfile()
    {
        NavigationViewModel.CurrentPage = AssistPage.SWAP;
        Log.Information("Looking for Account Profile");
        var account = AccountSettings.Default.Accounts.Find(x => x.Id == ProfileId);

        if (account is not null && !account.IsExpired && account.CanAssistBoot)
        {
            Log.Information("Default Account Attempting to Login");
            Log.Information("Create UI Preview");

            await CreateUiAccountPreview(account);
            try
            {
                await AuthenticateProfile(account); // This method handles the navigation to the Dashboard View.
                return;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return;
        }
        
        Log.Information("Account is not Valid to be swapped.");
        if (account.IsExpired)
        {
            Log.Information("Account is expired, sending to login page.");

            if (!string.IsNullOrEmpty(account.Username))
            {
                AssistApplication.ChangeMainWindowView(new RAccountAddPage(account.Username));
            }
            else
                AssistApplication.ChangeMainWindowView(new RAccountAddPage());
            return;
        }
    }
    
    private async Task CreateUiAccountPreview(AccountProfile account)
    {
        
        CurrentContent = new AccountPreviewStartupControl()
        {
            Icon = $"https://content.assistapp.dev/playercards/{account.Personalization.PlayerCardId}_DisplayIcon.png",
            AccountName = !string.IsNullOrEmpty(account.Personalization.AccountNickName)
                ? account.Personalization.AccountNickName
                : account.Personalization.RiotId,
            AccountRegion = $"Region: {account.Region.ToString()}"
        };
    }
    
    private async Task AuthenticateProfile(AccountProfile profile)
    {
        Log.Information($"SWAPPAGE: Attempting to login to AccountProfile Riot Account with Code | ID: {profile.Personalization.GameName}//{profile.Id}");
        string curlPath = Path.Exists(Path.Combine(DependencyUtils.CurlDependencyFolder, "curl.exe")) ? Path.Combine(DependencyUtils.CurlDependencyFolder, "curl.exe") : "curl";
        RiotUser usr = new RiotUserBuilder().WithCustomCurl(curlPath).WithRegion(profile.Region).WithSettings(new RiotUserSettings() { AuthenticationMethod = AuthenticationMethod.CURL }).Build();

        try
        {
            var cookies = profile.Convert64ToCookies();
            var v = new Dictionary<string, Cookie>();
            
            usr.GetAuthClient().SaveCookies(cookies);

            await usr.Authentication.ReAuthWithCookies();
        }
        catch (Exception e)
        {
            Log.Error("Failed to Authenticate with Cookies");
            Log.Error("Message: " + e.Message);
            Log.Error("Source: " + e.Source);
            Log.Error("Stack: " + e.StackTrace);

            profile.CanAssistBoot = false;
            profile.IsExpired = true;
            await AccountSettings.Default.UpdateAccount(profile);
            throw new Exception("Failed to Authenticate");
        }
        
        Log.Information("Account Successfully Logged in!");
        await HandleSuccessfulLogin(usr: usr);
        
        Log.Information("Going to Dashboard.");
        await AssistApplication.SetupComplete_Launcher();
    }

    private async Task HandleSuccessfulLogin(RiotUser usr)
    {
        Log.Information("SWAPPAGE: Successful login with Riot Account with Username/Password");
        AccountProfile profile = new AccountProfile();
        if (AccountSettings.Default.Accounts.Exists(x => x.Id == usr.UserData.sub))
            profile = AccountSettings.Default.Accounts.Find(x => x.Id == usr.UserData.sub);
        
        try
        {
            profile.Id = usr.UserData.sub;
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
            
            profile.ConvertCookiesTo64(usr.GetAuthClient().ClientCookies);
        }
        catch (Exception e)
        {
            Log.Error("Failed to Setup Account");
            Log.Error(e.Message);
            Log.Error(e.Source);
            Log.Error(e.StackTrace);
        }

        AssistApplication.ActiveUser = usr;
        AssistApplication.ActiveAccountProfile = profile;
        await AccountSettings.Default.UpdateAccount(profile);
        
        Log.Information("Finished Setting up Riot Account as the Main User & To the settings.");
    }
}