using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Game.Views;
using Assist.Game.Views.Initial;
using Assist.Objects.RiotClient;
using Assist.Services;
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
            await CheckForUpdates();


            Log.Information("Running Setup");

            if (!AssistSettings.Current.LanguageSelected)
            {
                Log.Information("Lang Setup");
                MainWindowContentController.Change(new SelectLanguage());
                return;
            }

            // Check Args
            if (AssistApplication.CurrentApplication.Args.Contains("--forcegame"))
            {
                MainWindowContentController.Change(new GameInitialView());
                return;
            }

            // Check if Game the game is running, if so launch gamemode.

            if (IsValorantRunning())
            {
                MainWindowContentController.Change(new GameInitialView());
                return;
            }



            await StartStartupAuthentication();
        }

        private async Task StartStartupAuthentication()
        {
            if (AssistSettings.Current.Profiles.Count == 0)
            {
                Log.Information("No Profiles Found, Going to Auth View");
                MainWindowContentController.Change(new AuthenticationView());
                return;
            }


            if (!string.IsNullOrEmpty(AssistSettings.Current.DefaultAccount))
            {
                Log.Information("Default Profile Found, using attempting Default.");

                var p = AssistSettings.Current.Profiles.Find(
                    x => x.ProfileUuid == AssistSettings.Current.DefaultAccount);

                if (p == null)
                    AssistSettings.Current.DefaultAccount = "";
                else
                {
                    Message = $"Logging into Default: {p.Gamename}";
                    try
                    {
                        await AuthProfile(p);
                    }
                    catch (ValNetException ex)
                    {
                        if (await BackupAuthentication(p))
                        {
                            try
                            {
                                await AuthProfile(p);
                            }
                            catch (ValNetException ex2)
                            {
                                p.isExpired = true; // Set the Profile to expired.

                                if (p.isExpired)
                                    Log.Fatal("Profile is Expired:" + p.Gamename);

                                Message = $"{p.Gamename} is expired.";
                            }
                        }
                    }
                }
            }

            if (AssistApplication.Current.CurrentUser != null || AssistApplication.Current.CurrentProfile != null)
                return;

            for (int i = 0; i < AssistSettings.Current.Profiles.Count; i++)
            {
                var profile = AssistSettings.Current.Profiles[i];
                try
                {
                    Message = $"Logging into: {profile.Gamename}";
                    if (!profile.isExpired)
                        await AuthProfile(profile);

                    if (AssistApplication.Current.CurrentUser != null || AssistApplication.Current.CurrentProfile != null)
                        return;
                }
                catch (ValNetException ex)
                {
                    Log.Error("Error Message: " + ex.Message);
                    if (ex is RequestException e)
                    {
                        Log.Error("Status Code: " + e.StatusCode);
                        // Add Auth Retry on Status Code 0
                        Log.Error("Content: " + e.Content);
                    }


                    if (await BackupAuthentication(profile))
                    {
                        try
                        {
                            await AuthProfile(profile);
                        }
                        catch (ValNetException ex2)
                        {
                            profile.isExpired = true; // Set the Profile to expired.

                            if (profile.isExpired)
                                Log.Fatal("Profile is Expired:" + profile.Gamename);

                            Message = $"{profile.Gamename} is expired.";
                        }
                    }
                }
            }

            // After Everyhing, if no profiles or users are logged into. Send to Auth Page.
            Log.Information("No Profiles were able to be logged into, Going to Auth View");
            MainWindowContentController.Change(new AuthenticationView());
        }

        public async Task AuthProfile(ProfileSettings p)
        {
            var user = new RiotUserBuilder().WithSettings(new RiotUserSettings() { AuthenticationMethod = AuthenticationMethod.CURL }).Build();
            
            AuthenticationResult r = await AuthenticationController.CookieLogin(p, user);
            

            if (r.error != null)
            {
                // Handle Error
                Log.Error(r.error + " Auth Error Hit");
            }

            if (string.IsNullOrEmpty(AssistSettings.Current.DefaultAccount))
                AssistSettings.Current.DefaultAccount = p.ProfileUuid;

            await FinishAuthentication(user);
        }

        private async Task FinishAuthentication(RiotUser u)
        {
            ProfileSettings pS = new ProfileSettings();

            pS.SetupProfile(u);
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

        public async Task CheckForUpdates()
        {
#if (!DEBUG)

            if (AssistApplication.Current.Platform.OperatingSystem == OperatingSystemType.WinNT)
            {
                try
                {
                    using var mgr = new UpdateManager("https://content.assistapp.dev/releases/beta/windows/");
                    var newVersion = await mgr.UpdateApp();

                    // You must restart to complete the update. 
                    // This can be done later / at any time.
                    if (newVersion != null)
                        UpdateManager.RestartApp();

                    return;
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
                    if (newVersion != null)
                        UpdateManager.RestartApp();

                    return;
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
                    if (newVersion != null)
                        UpdateManager.RestartApp();

                    return;
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }
#endif



        }
    }
}
