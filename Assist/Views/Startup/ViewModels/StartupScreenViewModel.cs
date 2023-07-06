using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Controls.Global.Popup;
using Assist.Game.Views;
using Assist.Game.Views.Initial;
using Assist.Objects.RiotClient;
using Assist.Services;
using Assist.Services.Popup;
using Assist.Settings;
using Assist.ViewModels;
using Assist.Views.Authentication;
using Assist.Views.Authentication.Sections.ViewModels;
using Avalonia;
using Avalonia.Platform;
using ReactiveUI;
using Serilog;
using Serilog.Core;
using Squirrel;
using ValNet;
using ValNet.Enums;
using ValNet.Objects;
using ValNet.Objects.Authentication;
using ValNet.Objects.Exceptions;
using YamlDotNet.Serialization.NamingConventions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Assist.Views.Startup.ViewModels
{
    internal class StartupScreenViewModel : ViewModelBase
    {
        private string _message;

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }
        public async Task StartupSetup()
        {
            Log.Information("Checking for Update");
            Message = "Checking for Updates...";
            var cont = await CheckForUpdates();
            if (!cont)
            {
                // Means update is found and needs to stop.
            }


            Log.Information("Running Setup");

            if (!AssistSettings.Current.LanguageSelected)
            {
                Log.Information("Lang Setup");
                MainWindowContentController.Change(new SelectLanguage());
                return;
            }
            Log.Information("Connecting to GENERAL SERVER");
            await AssistApplication.Current.ConnectToServerHub();
            // Check Args
            if (AssistApplication.CurrentApplication.Args.Contains("--forcegame"))
            {
                MainWindowContentController.Change(new GameInitialView());
                return;
            }

            // Check if Game the game is running, if so launch gamemode.

            if (IsValorantRunning())
            {
                //TODO THIS IS BROKEN
                /*var menuPopup = new GamemodeWarningPopup();
                menuPopup.WarningClosing += GamemodePopupClose;
                PopupSystem.SpawnCustomPopup(menuPopup);
                return;*/
                
                MainWindowContentController.Change(new GameInitialView());
                return;
            }


            
            await StartStartupAuthentication();
        }

        private async void GamemodePopupClose()
        {
            PopupSystem.KillPopups();
            await StartStartupAuthentication();
        }

        private async Task StartStartupAuthentication()
        {
            if (!string.IsNullOrEmpty(AssistSettings.Current.AssistUserCode))
            {
                Log.Information("Logging into Assist Account");
                try
                {
                    var authResp = await AssistApplication.Current.AssistUser.Authentication.AuthenticateWithRefreshToken(AssistSettings.Current
                        .AssistUserCode);
                    AssistSettings.Current.AssistUserCode = authResp.RefreshToken;
                    AssistSettings.Save();
                    await AssistApplication.Current.AssistUser.Account.GetUserInfo();
                }
                catch (Exception e)
                {
                    Log.Fatal("Account Token is not Valid");
                }
            }
            
            if (AssistSettings.Current.Profiles.Count == 0)
            {
                Log.Information("No Profiles Found, Going to Auth View");
                MainWindowContentController.Change(new AuthenticationView());
                return;
            }

            await ExperimentalAuth();
        }

        public async Task AuthProfile(ProfileSettings p)
        {
            var user = new RiotUserBuilder().WithSettings(new RiotUserSettings() { AuthenticationMethod = AuthenticationMethod.CURL }).Build();
            
            AuthenticationResult r = await AuthenticationController.CookieLogin(p, user);
            

            if (r.error != null)
            {
                // Handle Error
                Log.Error(r.error + " Auth Error Hit");
                throw new Exception(r.error);
            }

            if (string.IsNullOrEmpty(AssistSettings.Current.DefaultAccount))
                AssistSettings.Current.DefaultAccount = p.ProfileUuid;

            await FinishAuthentication(user);
        }

        private async Task FinishAuthentication(RiotUser u)
        {
            ProfileSettings pS = new ProfileSettings();

            await pS.SetupProfile(u);
            await AssistSettings.Current.SaveProfile(pS);

            pS.ConvertCookiesTo64(u.GetAuthClient().ClientCookies);

            AssistApplication.Current.CurrentUser = u;
            AssistApplication.Current.CurrentProfile = pS;
            AssistApplication.Current.OpenMainView();
        }

        private async Task<bool> BackupAuthentication(ProfileSettings p)
        {
            string[] folders = System.IO.Directory.GetDirectories(BackupsSettings.BackupsFolderPath, "*", System.IO.SearchOption.AllDirectories);

            string backupFolderPath = string.Empty;
            foreach (string folder in folders)
            {
                if (folder.Contains(p.ProfileUuid))
                {
                    Log.Information("Found Backup of Profile");
                    backupFolderPath = folder;
                    break;
                }
            }

            if (backupFolderPath == string.Empty)
            {
                return false;
            }

            var files = Directory.GetFiles(backupFolderPath);
            var filePath = files.Single(file => file.Contains("RiotGamesPrivateSettings"));
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var settings = deserializer.Deserialize<RiotGamesPrivateModel>(File.ReadAllText(filePath));

            if (!await CheckSettings(settings))
            {
                Log.Information("Bad backup");

                return false;
            }

            var dic = await RCAuthViewModel.CreateCookieDic(settings);

            Log.Information("Backup is good to go");
            p.ConvertCookiesTo64(dic);
            return true;
        }

        // Stolen from Riot Client Auth Code for now
        private async Task<bool> CheckSettings(RiotGamesPrivateModel config)
        {
            try
            {
                if (config.RiotLogin?.Persist?.Session?.Cookies != null)
                {
                    var ssid = config.RiotLogin.Persist.Session.Cookies.Find(c => c.name == "ssid");

                    if (ssid != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            return false;
        }

        private bool IsValorantRunning()
        {
            var processlist = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Where(process => process.Id != Process.GetCurrentProcess().Id).ToList();
            processlist.AddRange(Process.GetProcessesByName("VALORANT-Win64-Shipping"));

            return processlist.Any();
        }

        public async Task<bool> CheckForUpdates()
        {
#if (!DEBUG)

            if (AssistApplication.Current.Platform.OperatingSystem == OperatingSystemType.WinNT)
            {
                try
                {
                    using var mgr = new UpdateManager("https://content.assistapp.dev/releases/3a4f7214-2ab5-4bf2-b2cd-7db4d560a82d/windows/");
                    var updateInfo = await mgr.CheckForUpdate();
                    if (updateInfo.ReleasesToApply.Any())
                    {
                        var newVersion = await mgr.UpdateApp();
                        // You must restart to complete the update. 
                        // This can be done later / at any time.
                        return false;
                        if (newVersion != null)
                            UpdateManager.RestartApp();
                    }

                   return true;
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }

            if (AssistApplication.Current.Platform.OperatingSystem == OperatingSystemType.OSX)
            {
                try
                {
                    using var mgr = new UpdateManager("https://cdn.assistapp.dev/Releases/live/mac/");
                    var newVersion = await mgr.UpdateApp();

                    // You must restart to complete the update. 
                    // This can be done later / at any time.
return false;
                    if (newVersion != null)
                        UpdateManager.RestartApp();

                    return true;

                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }

            if (AssistApplication.Current.Platform.OperatingSystem == OperatingSystemType.Linux)
            {
                try
                {
                    using var mgr = new UpdateManager("https://cdn.assistapp.dev/Releases/live/linux/");
                    var newVersion = await mgr.UpdateApp();

                    // You must restart to complete the update. 
                    // This can be done later / at any time.
return false;
                    if (newVersion != null)
                        UpdateManager.RestartApp();

                    return true;

                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }
#endif

            return true;

        }

        public async Task ExperimentalAuth()
        {
            // Checks there is a Default Profile Profile
            if (!string.IsNullOrEmpty(AssistSettings.Current.DefaultAccount))
            {
                Log.Information("Default Profile Found, using attempting Default.");

                var p = AssistSettings.Current.Profiles.Find(
                    x => x.ProfileUuid == AssistSettings.Current.DefaultAccount);

                if (p != null)
                {
                    Message = $"Logging into Default: {p.Gamename}";

                    var backupExcists = await BackupsSettings.CheckIfBackupExistsForId(p.ProfileUuid);

                    // Backup Auth
                    if (backupExcists)
                    {
                        // This is ran if the folder is found.
                        ProfileSettings backupSettings = null;
                        try
                        {
                            // Data is read
                            var data = await BackupsSettings.ReadBackupFromId(p.ProfileUuid);

                            if (data != null)
                            {
                                backupSettings = data;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }

                        // Attempt to Authenticate with BackupSettings
                        if (backupSettings != null)
                        {
                            Log.Information("Logging in with BackupData");
                            try
                            {
                                await AuthProfile(backupSettings);
                                return;
                            }
                            catch (ValNetException ex)
                            {
                                Message = $"Backup is expired.";
                            }
                        }
                    }

                    // Cookie Auth
                    try
                    {
                        Log.Information("Logging in with Cookie AUth");
                        await AuthProfile(p);
                        return;
                    }
                    catch (Exception ex2)
                    {
                        p.isExpired = true; // Set the Profile to expired.

                        if (p.isExpired)
                            Log.Fatal("Profile is Expired:" + p.Gamename);

                        Message = $"{p.Gamename} is expired.";
                    }
                }
                else
                {
                    AssistSettings.Current.DefaultAccount = "";
                }
            }

            // Go through Each Profile
            for (int i = 0; i < AssistSettings.Current.Profiles.Count; i++)
            {
                var p = AssistSettings.Current.Profiles[i];
                Message = $"Logging into: {p.Gamename}";
                if (!p.isExpired)
                {

                    // Backup Auth
                    if (await BackupsSettings.CheckIfBackupExistsForId(p.ProfileUuid))
                    {
                        // This is ran if the folder is found.
                        ProfileSettings backupSettings = null;
                        try
                        {
                            // Data is read
                            var data = await BackupsSettings.ReadBackupFromId(p.ProfileUuid);

                            if (data != null)
                            {
                                backupSettings = data;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }

                        // Attempt to Authenticate with BackupSettings
                        if (backupSettings != null)
                        {
                            Log.Information("Logging in with BackupData");
                            try
                            {
                                await AuthProfile(backupSettings);
                                return;
                            }
                            catch (ValNetException ex)
                            {
                                Message = $"Backup is expired.";
                            }
                        }
                    }

                    // Cookie Auth
                    try
                    {
                        Log.Information("Logging in with Cookie data");
                        await AuthProfile(p);
                        return;
                    }
                    catch (Exception ex2)
                    {
                        p.isExpired = true; // Set the Profile to expired.

                        if (p.isExpired)
                            Log.Fatal("Profile is Expired:" + p.Gamename);

                        Message = $"{p.Gamename} is expired.";

                        if (ex2 is RequestException e)
                        {
                            Log.Error("Status Code: " + e.StatusCode);
                            // Add Auth Retry on Status Code 0
                            Log.Error("Content: " + e.Content);
                        }
                    }
                }
            }

            // After Everything, if no profiles or users are logged into. Send to Auth Page.
            Log.Information("No Profiles were able to be logged into, Going to Auth View");
            MainWindowContentController.Change(new AuthenticationView());
        }
    }
}
