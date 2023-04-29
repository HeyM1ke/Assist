using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Assist.Objects.RiotClient;
using Assist.Services;
using Assist.Services.Riot;
using Assist.Settings;
using Assist.ViewModels;
using Serilog;
using ValNet;
using ValNet.Enums;
using ValNet.Objects;
using ValNet.Objects.Exceptions;
using YamlDotNet.Serialization.NamingConventions;

namespace Assist.Views.Authentication.Sections.ViewModels
{
    internal class RCAuthViewModel : ViewModelBase
    {
        // Check first for Login if there is one stored locally.
            // if not, show screen

        // Once a User Logs in, return to Assist.
        // Log into account to verify if it works, Show Username to Confirm if they want to Add the Account
        // Delete Data in the Config
        private static readonly string defaultConfigLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data");
        private static readonly string defaultConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data", "RiotGamesPrivateSettings.yaml");
        private Process _riotClientProcess;
        private FileSystemWatcher _authFileWatcher;
        public RCAuthViewModel()
        {
            
        }


        public async Task StartLogin()
        {
            await StartRiotClient();
            await StartWatcher();
        }

        public async Task StartRiotClient()
        {
            // Check if there is a Riot Games file already.

            if (File.Exists(defaultConfigPath))
            {
                File.Delete(defaultConfigPath); // removed any currently logged in client

                // Close any Riot client instance
                await CloseRiotRelatedPrograms();

            }

            string clientLocation = await AssistSettings.Current.FindRiotClient();
            if (clientLocation == null)
            {
                Log.Error("DID NOT FIND CLIENT");
            }
            ProcessStartInfo riotClientStart = new ProcessStartInfo(clientLocation, $"--launch-product=valorant --launch-patchline={AssistApplication.Current.ClientLaunchSettings.Patchline}")
            {
                UseShellExecute = true
            };

            Process.Start(riotClientStart);
            await Task.Delay(1000);
            while (_riotClientProcess == null)
            {
                Log.Information("Looking for Riot Client");
                var rProcesses = await RiotClientService.GetCurrentRiotProcesses();
                _riotClientProcess = rProcesses.Where(_p => _p.ProcessName.Contains("RiotClientServices")).FirstOrDefault();

                if (_riotClientProcess != null)
                {
                    Log.Information("Riot Client Found");
                    _riotClientProcess.EnableRaisingEvents = true;
                    return;
                }

                await Task.Delay(1500);
            }

        }


        public async Task StartWatcher()
        {
            _authFileWatcher = new FileSystemWatcher(defaultConfigLocation, "RiotGamesPrivateSettings.yaml");

            _authFileWatcher.NotifyFilter = NotifyFilters.Attributes
                                            | NotifyFilters.CreationTime
                                            | NotifyFilters.DirectoryName
                                            | NotifyFilters.FileName
                                            | NotifyFilters.LastAccess
                                            | NotifyFilters.LastWrite
                                            | NotifyFilters.Security
                                            | NotifyFilters.Size;

            _authFileWatcher.Changed += AuthFileWatcherOnChanged;

            _authFileWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Signals whenever there is a change on the Private Settings YAML File.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AuthFileWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            Log.Information("Config File has Changed");
            Log.Information("Attempting to Parse File");
            await ParseSettingsFile();
        }

        private async Task ParseSettingsFile()
        {
            RiotGamesPrivateModel settings = new RiotGamesPrivateModel();
            try
            {
                var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                string rgpm = File.ReadAllText(defaultConfigPath);
                settings = deserializer.Deserialize<RiotGamesPrivateModel>(rgpm);
            }
            catch (Exception e)
            {
                Log.Error("Failed to Parse Settings after Change");
                Log.Error("Failed to Parse");
            }

            var check = await CheckSettings(settings);

            if (check)
            {
                Log.Information("SSID has been found");
                Log.Information("Saving Profile.");

                _authFileWatcher.Dispose();
                // Create Riot User
                try
                {
                    Log.Information("Attempting to Create USER");
                    await CreateUser(settings);
                }
                catch (Exception e)
                {
                    Log.Information("Starting Watcher Again");

                    await StartWatcher();
                }
                
            }


        }

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

        public static async Task<Dictionary<string, Cookie>> CreateCookieDic(RiotGamesPrivateModel settings)
        {
            Dictionary<string, Cookie> cookies = new Dictionary<string, Cookie>();

            var cook = settings.RiotLogin.Persist.Session.Cookies;
            foreach (var cookie in cook)
            {
                cookies.Add(cookie.name, new Cookie()
                {
                    Name = cookie.name,
                    Domain = cookie.domain,
                    Path = cookie.path,
                    Value = cookie.value
                });
            }

            return cookies;
        }
        private async Task CreateUser(RiotGamesPrivateModel config)
        {
            var u = new RiotUserBuilder().WithSettings(new RiotUserSettings()
            {
                AuthenticationMethod = AuthenticationMethod.CURL,
            }).Build();

            var cookies = await CreateCookieDic(config);

            u.GetAuthClient().SetCookies(cookies);

            try
            {
                await AuthenticationController.CookieLogin(u);
            }
            catch (ValNetException ex)
            {
                Log.Error("Error Message: " + ex.Message);
                if (ex is RequestException e)
                {
                    Log.Error("Status Code: " + e.StatusCode);
                    Log.Error("Content: " + e.Content);
                }

                throw ex; // Show Error
            }

            // Kill Riot Client
            await CloseRiotRelatedPrograms();
            await FinishAuthentication(u);


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

        private async Task CloseRiotRelatedPrograms()
        {
            var rProcesses = await RiotClientService.GetCurrentRiotProcesses();
            var rC = rProcesses.Where(_p => _p.ProcessName.Contains("RiotClientServices")).FirstOrDefault();
            var val = rProcesses.Where(_p => _p.ProcessName.Contains("VALORANT-Win64-Shipping")).FirstOrDefault();

            if (val != null) { val.Kill(); }
            if(rC != null) { rC.Kill(); }
        }
    }

        
    }
