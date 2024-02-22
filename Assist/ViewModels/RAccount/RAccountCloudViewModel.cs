﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Assist.Shared.Controls;
using Assist.Shared.Services.Utils;
using Assist.Shared.Settings;
using Assist.Shared.Settings.Accounts;
using Avalonia.Controls;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Serilog;
using ValNet;
using ValNet.Enums;
using ValNet.Objects;
using ValNet.Objects.Authentication;

namespace Assist.ViewModels.RAccount;


/// <summary>
/// Integration of Custom Webview2 Class.
/// </summary>
public partial class RAccountCloudViewModel : ViewModelBase
{
    [ObservableProperty] private WebView _currentContent = new();
    [ObservableProperty] private bool _webViewVisible = true;
    [ObservableProperty] private ICommand? _loginCompletedCommand;

    private const string authUrl = "https://auth.riotgames.com/authorize?redirect_uri=https%3A%2F%2Fplayvalorant.com%2Fopt_in&client_id=play-valorant-web-prod&response_type=token%20id_token&nonce=1";
    private string cacheLoc = System.IO.Path.Combine(AssistSettings.FolderPath, "Cache", "web");
    private const string socialGuide = "https://github.com/HeyM1ke/Assist/wiki/Social-Login-Guide";

    public async Task Setup()
    {
        CurrentContent.View = new WebView2();
        var webView2Environment = await CoreWebView2Environment.CreateAsync(null, cacheLoc);
        await CurrentContent.View.EnsureCoreWebView2Async(webView2Environment);
        CurrentContent.View.NavigationStarting += EnsureHttps;
        CurrentContent.View.SourceChanged += SourceChanged;
        CurrentContent.View.Source = new Uri(authUrl);
        
    }
    
    private async Task LoginWithWebCookies(Dictionary<string, Cookie> cookieContainer)
    {
        Log.Information("Attempting to login with Riot Account with Cloud");
        string curlPath = Path.Exists(Path.Combine(DependencyUtils.CurlDependencyFolder, "curl.exe")) ? Path.Combine(DependencyUtils.CurlDependencyFolder, "curl.exe") : "curl";
        RiotUser usr = new RiotUserBuilder().WithCustomCurl(curlPath).WithSettings(new RiotUserSettings()
        {
            AuthenticationMethod = AuthenticationMethod.CURL
        }).Build();

        try
        {
            usr.GetAuthClient().SetCookies(cookieContainer);
           var result =  await usr.Authentication.ReAuthWithCookies();
           
        }
        catch (Exception e)
        {
            Log.Error("Failed to Authenticate with Cookies");
            Log.Error("Message: " + e.Message);
            Log.Error("Source: " + e.Source);
            Log.Error("Stack: " + e.StackTrace);
            
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
            profile.Region = usr.GetRegion();
            profile.LastLoginTime = DateTime.UtcNow;
            profile.Personalization = new AccountProfile.AccountProfilePersonalization()
            {
                GameName = usr.UserData.acct.game_name,
                TagLine = usr.UserData.acct.tag_line
            };
            var inventory = await usr.Inventory.GetPlayerInventory();
            var pMmr = await usr.Player.GetPlayerMmr();
            profile.Personalization.PlayerCardId = inventory.PlayerData.PlayerCardID;
            profile.Personalization.PlayerLevel = inventory.PlayerData.AccountLevel;
            profile.Personalization.ValRankTier = pMmr.LatestCompetitiveUpdate.TierAfterUpdate;
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
            
            return;
        }

        AssistApplication.ActiveUser = usr;
        AssistApplication.ActiveAccountProfile = profile;

        AccountSettings.Default.DefaultAccount =
            string.IsNullOrEmpty(AccountSettings.Default.DefaultAccount) ? profile.Id : "";
        await AccountSettings.Default.UpdateAccount(profile);
        AccountSettings.Save();
        
        Log.Information("Finished Setting up Riot Account as the Main User & To the settings.");
        LoginCompletedCommand?.Execute("");
    }

    private async void SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
    {
        var redirectUrl = CurrentContent.View.Source.ToString();
        Log.Information(redirectUrl);
        CurrentContent.View.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
        CurrentContent.View.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        if (redirectUrl.Contains("https://playvalorant.com/opt_in#"))
        {
            var cookies = await GetCookies(this.CurrentContent.View);
            var valid = cookies.Find(_c => _c.Name == "ssid") != null;
            if (valid)
            {
                CurrentContent.IsVisible = false;

                var cc = new Dictionary<string, Cookie>();
                cookies.ForEach(x =>
                {
                    if (x.Domain.Contains("riotgames.com"))
                    {
                        cc.TryAdd(x.Name, new Cookie(x.Name, x.Value, x.Path, x.Domain));
                    }
                });
                

                await LoginWithWebCookies(cc);
            }
        }
    }
    
    
    
    private async Task<List<CoreWebView2Cookie>?> GetCookies(WebView2 webView)
    {           
        var result = await webView.CoreWebView2.CookieManager.GetCookiesAsync(null);

        result.ForEach(x => Log.Information(x.Name));
        var c = result.Find(_c => _c.Name == "ssid");

        if (c != null)
        {
            Log.Debug("Found SSID Cookie from Authentication");
            var cook = new Cookie
            {
                Name = c.Name,
                Value = c.Value,
                Path = c.Path,
                Secure = c.IsSecure,
                HttpOnly = c.IsHttpOnly,
                Domain = c.Domain,
            };
            if (c.Expires.ToString().Contains("1/1/0001"))
                cook.Expires = DateTime.Now.AddMonths(1);
            else
                cook.Expires = c.Expires;

            webView.CoreWebView2.CookieManager.DeleteAllCookies();

        }

        return result;
    }
    private void EnsureHttps(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (!e.Uri.StartsWith("https://"))
        {
            e.Cancel = true;
        }
    }
}