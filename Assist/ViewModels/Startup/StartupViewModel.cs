﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Controls.Assist.Account;
using Assist.Controls.Assist.Authentication;
using Assist.Controls.Infobars;
using Assist.Controls.Navigation;
using Assist.Controls.Startup;
using Assist.Core.Helpers;
using Assist.Core.Settings.Options;
using Assist.Services.Navigation;
using Assist.Shared.Services.Utils;
using Assist.Shared.Settings;
using Assist.Shared.Settings.Accounts;
using Assist.ViewModels.Infobars;
using Assist.Views.Assist;
using Assist.Views.Dashboard;
using Assist.Views.Extras;
using Assist.Views.Game;
using Assist.Views.RAccount;
using Assist.Views.Setup;
using Assist.Views.Startup;
using AssistUser.Lib.V2.Models;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Svg.Skia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using SkiaSharp;
using Svg.Skia;
using ValNet;
using ValNet.Enums;
using ValNet.Objects;
using ValNet.Objects.Authentication;
using Velopack;

namespace Assist.ViewModels.Startup;

public partial class StartupViewModel : ViewModelBase
{
    [ObservableProperty]
    private Control _currentContent = new BasicStartupControl();

    [ObservableProperty] private string _attemptProfileId = String.Empty;

    public async Task Startup()
    {  
        NavigationContainer.ViewModel.HideAllButtons();
        // Check for Dependency
        await DependencyUtils.CheckDepends();
        var newVersion = await AssistApplication.CheckForUpdates();
        
        if(newVersion)
        {
            Log.Information("New Version Available. Redirecting to Update Page");
            AssistApplication.OpenUpdateWindow();
            return;
        }
        // Check if setting have completed the tutorial.
        
        if (!AssistSettings.Default.CompletedSetup)
        {
            // Start Setup Guide
            Log.Information("Settings is Reading that the setup has not been completed. Starting Setup");
            AssistApplication.ChangeMainWindowView(new SetupView());
            return;
        }

        Log.Information("Checking for Assist Account Login");
        if (!string.IsNullOrEmpty(AssistSettings.Default.AssistUserCode) && string.IsNullOrEmpty(AssistApplication.AssistUser.userTokens.AccessToken))
        {
            Log.Information("Settings is Reading that an Assist Account exists. Attempting to login");
            try
            {
                var t = await AssistApplication.AssistUser.Authentication.AuthenticateWithRefreshToken(AssistSettings
                    .Default
                    .GetAssistUserCode());
#if DEBUG
                Log.Information($"Assist Access: {t.AccessToken}");
#endif
                
                Log.Information("Assist Account Login Successful");
                Log.Information("Saving Account Code...");
                AssistSettings.Default.SaveAssistUserCode(AssistApplication.AssistUser.userTokens.RefreshToken);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                Log.Error(e.StackTrace);
                Log.Information("Assist Account Login Unsuccessful");
                AssistSettings.Default.AssistUserCode = ""; // Doing this since this will make it only show once.
                Dispatcher.UIThread.Invoke(() =>
                {
                    AssistApplication.ChangeMainWindowPopupView(new AssistAuthenticationView());
                });
                
                
            }
        }

        if (!string.IsNullOrEmpty(AssistApplication.AssistUser.userTokens.AccessToken))
        {
            Log.Information("Checking Assist Account Information");
            try
            {
                var accountInfo = await AssistApplication.AssistUser.Account.GetAccountInfo();

                if (accountInfo.Code == 200)
                {
                    var _accountInfo = JsonSerializer.Deserialize<AAccount>(accountInfo.Data.ToString());
                    if (string.IsNullOrEmpty(_accountInfo.Personalization.DisplayName))
                    {
                        AssistApplication.ChangeMainWindowPopupView(new CustomizeAssistDisplayNameControl(DisplayNameCompletedCommand));
                        return; 
                    }
                }
                
                
            }
            catch (Exception e)
            {
                
            }
            
        }

        if (!string.IsNullOrEmpty(AssistApplication.AssistUser.userTokens.AccessToken))
        {
            try
            {
                Log.Information("Attempting to connect to the Assist Server.");
                await AssistApplication.Server.Connect();
            }
            catch (Exception e)
            {
                Log.Error("Failed to connect to Assist Server");
            }
        }
        
        switch (AssistSettings.Default.AppType)
        {
            case AssistApplicationType.GAME_ONLY:
                Log.Information("Assist is in Game only mode. Switching to Game Mode");
                AssistApplication.ChangeMainWindowView(new GameInitialStartupView());
                return;
            default:
                Log.Information("Assist is in Launcher Only/Complete . Continuing Basic Procedure.");
                break;
        }

        if (IsValorantRunning())
        {
            Log.Information("Valorant is running, swapping to game mode.");
            AssistApplication.ChangeMainWindowView(new GameInitialStartupView());
            return;
        }
        
        Log.Information("Launcher Setup Starting...");

        await LauncherSetup();
    }

    private async Task LauncherSetup()
    {
        Log.Information("Checking for any Accounts Stored");

        if (AccountSettings.Default.Accounts.Count == 0)
        {
            Log.Information("No Accounts are stored, Redirecting to Riot Authentication Page");
            AssistApplication.ChangeMainWindowView(new RAccountAddPage());
            return;
        }
        
        if (IsValorantRunning())
        {
            Log.Information("Valorant is Running. Switching to Game Mode");
            AssistApplication.ChangeMainWindowView(new GameInitialStartupView());
            return;
        }
        

        await AttemptAuthentication();
    }

    private bool IsValorantRunning()
    {
        return Process.GetProcessesByName("VALORANT-Win64-Shipping").Any(process => process.Id != Process.GetCurrentProcess().Id);
    }

    private async Task AttemptAuthentication()
    {

        if (!string.IsNullOrEmpty(AttemptProfileId))
        {
            Log.Information("Attempt Profile ID Passed In, Attempting to Login");
            var passedProfile = AccountSettings.Default.Accounts.Find(x => x.Id == AttemptProfileId);
            Log.Information("Create UI Preview");

            if (passedProfile is not null)
            {
                await CreateUiAccountPreview(passedProfile);
                try
                {
                    await AuthenticateProfile(passedProfile); // This method handles the navigation to the Dashboard View.
                    return;
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }
           
        }
        
        
        Log.Information("Checking for Default Account");
        var defaultAccount = AccountSettings.Default.Accounts.Find(x => x.Id == AccountSettings.Default.DefaultAccount);

        if (defaultAccount is not null && !defaultAccount.IsExpired && defaultAccount.CanAssistBoot)
        {
            Log.Information("Default Account Attempting to Login");
            Log.Information("Create UI Preview");
            
            await CreateUiAccountPreview(defaultAccount);
            try
            {
                await AuthenticateProfile(defaultAccount); // This method handles the navigation to the Dashboard View.
                return;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        Log.Information("Starting to Loop through other accounts.");
        for (int i = 0; i < AccountSettings.Default.Accounts.Count; i++)
        {
            var acc = AccountSettings.Default.Accounts[i];
            if (acc.Id.Equals(AccountSettings.Default.DefaultAccount) || acc.IsExpired || !acc.CanAssistBoot) // This has already been attempted. No Reason for it to be twice.
                continue;
            
            Log.Information("New Account Attempting to Login");
            Log.Information("Create UI Preview");
            
            await CreateUiAccountPreview(acc);
            try
            {
                await AuthenticateProfile(acc); // This method handles the navigation to the Dashboard View.
                return;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
        
        Log.Information("This has been hit, meaning all accounts are not valid. Moving to Adding Page");

        
        AssistApplication.ChangeMainWindowView(new RAccountAddPage());
        
    }

    private async Task CreateUiAccountPreview(AccountProfile defaultAccount)
    {
        
        CurrentContent = new AccountPreviewStartupControl()
        {
            Icon = $"https://cdn.assistval.com/playercards/{defaultAccount.Personalization.PlayerCardId}_DisplayIcon.png",
            AccountName = !string.IsNullOrEmpty(defaultAccount.Personalization.AccountNickName)
                ? defaultAccount.Personalization.AccountNickName
                : defaultAccount.Personalization.RiotId,
            AccountRegion = $"Region: {defaultAccount.Region.ToString()}"
        };
    }


    private async Task AuthenticateProfile(AccountProfile profile)
    {
        Log.Information($"Attempting to login to AccountProfile Riot Account with Code | ID: {profile.Personalization.GameName}//{profile.Id}");
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
        Log.Information("Successful login with Riot Account with Username/Password");
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

    [RelayCommand]
    private async void DisplayNameCompleted()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            AssistApplication.ChangeMainWindowPopupView(null);
            AssistApplication.ChangeMainWindowView(new StartupView());
        });
    }
}