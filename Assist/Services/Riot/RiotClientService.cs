using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Core.Settings.Options;
using Assist.Models.Riot;
using Assist.Shared.Services.Utils;
using Assist.Shared.Settings;
using Assist.Shared.Settings.Accounts;
using Assist.ViewModels;
using Assist.Views.Game;
using Avalonia.Threading;
using Serilog;
using YamlDotNet.Serialization.NamingConventions;

namespace Assist.Services.Riot
{
    internal class RiotClientService
    {
        public static event EventHandler ValorantGameLaunched;
        public static event EventHandler RiotClientLaunched;

        private string RiotClientLocation;
        public static bool ClientOpened = false;
        private BackgroundWorker _worker;
        public RiotApplicationData RiotApplicationData = new RiotApplicationData();
        public string AdditionalRiotClientArguments = string.Empty;

        private const string bgVidUrl = "https://cdn.rumblemike.com/Static/live/assistBackVideo_dev.mp4";
        
        /*public async Task<bool> LaunchClient()
        {
            Log.Information("Attempting to Start Client");

            Log.Information("Killing all riots");
            await CloseRiotRelatedPrograms();

            Log.Information("Trying to find client location");
            string clientLocation = await FindRiotClient();
            if (clientLocation == null){
                Log.Error("NOT FOUND");
                return false;
            }
            Log.Information("Creating Auth Files");
            await CreateAuthenticationFile();

            Log.Information("Attempting to Launch Client");

            string argumentString =
                $"--launch-product=valorant --launch-patchline={AssistApplication.Current.ClientLaunchSettings.Patchline.ToLower()} --insecure";

            if (!string.IsNullOrEmpty(AssistSettings.Current.AdditionalArgs))
            {
                argumentString += $" {AssistSettings.Current.AdditionalArgs}";
            }
            
            ProcessStartInfo riotClientStart = new ProcessStartInfo(clientLocation, argumentString)
            {
                UseShellExecute = true
            };

            Process.Start(riotClientStart);
            await Task.Delay(1000); // Delay for the Client to Start
            StartWorker();

            return true;
        }*/

        public async Task CreateAuthenticationFile()
        {
            string baseRiotClientFolder =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client");
            
            string pSettingsPath = Path.Combine(baseRiotClientFolder, "Data", "RiotGamesPrivateSettings.yaml");
            string pSettingsPathBackup = Path.Combine(baseRiotClientFolder, "Data", "RiotClientPrivateSettings.yaml");
            string pClientSettingsPath = Path.Combine(baseRiotClientFolder, "Config", "RiotClientSettings.yaml");

            string riotClient = await FindRiotClient();
            var fileInfo = FileVersionInfo.GetVersionInfo(riotClient);
            var cSettings = new ClientPrivate(AssistApplication.ActiveUser);

            if (fileInfo.FileMajorPart >= 46)
            {
                // Create File

                // Creates Settings File
                using (TextWriter writer = File.CreateText(pClientSettingsPath))
                {
                    cSettings.CreateSettingsModel().Save(writer, false);
                }

                // Create RiotClientPrivateSettings.yaml
                using (TextWriter writer = File.CreateText(pSettingsPath))
                {
                    cSettings.CreateGameModelWRegionMicro().Save(writer, false);
                }
                
                if (Path.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Beta")))
                {
                    var baseBetaPass =  Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Beta");
                    var pBetaSettingsPath = Path.Combine(baseBetaPass, "Data", "RiotGamesPrivateSettings.yaml");
                    var pBetaSettingsPathBackup = Path.Combine(baseBetaPass, "Data", "RiotClientPrivateSettings.yaml");
                    var pBetaClientSettingsPath = Path.Combine(baseBetaPass, "Config", "RiotClientSettings.yaml");
                    
                    
                    using (TextWriter writer = File.CreateText(pBetaClientSettingsPath))
                    {
                        cSettings.CreateSettingsModel().Save(writer, false);
                    }

                    // Create RiotClientPrivateSettings.yaml
                    using (TextWriter writer = File.CreateText(pBetaSettingsPath))
                    {
                        cSettings.CreateGameModelWRegionMicro().Save(writer, false);
                    }
                    
                }
                
            }
            else
            {
                // Create File
                // Create RiotClientPrivateSettings.yaml
                using (TextWriter writer = File.CreateText(pSettingsPath))
                {
                    cSettings.CreateGameModel().Save(writer, false);
                }

                using (TextWriter writer = File.CreateText(pSettingsPathBackup))
                {
                    cSettings.CreateClientPrivateModel().Save(writer, false);
                }
            }
        }
        
        public void StartWorker()
        {
            Log.Information("Background Worker Started");
            _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;

            _worker.RunWorkerAsync();
        }

        private async void _worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            while (!ClientOpened)
            {
                var rProcesses = await GetCurrentRiotProcesses();
                var rC = rProcesses.Where(_p => _p.ProcessName.Contains("RiotClientServices")).FirstOrDefault();
                var val = rProcesses.Where(_p => _p.ProcessName.Contains("VALORANT-Win64-Shipping")).FirstOrDefault();

                if (rC != null && RiotApplicationData.RiotClientProcess == null)
                {
                    RiotApplicationData.RiotClientProcess = rC;
                    RiotApplicationData.RiotClientProcess.EnableRaisingEvents = true;

                    Log.Information("Found Riot Client Process");
                    Log.Information($"Process ID: {rC.Id}");
                    Log.Information($"Process Name: {rC.ProcessName}");
                    RiotClientLaunched?.Invoke(this, e);
                }

                if (val != null && RiotApplicationData.ValorantGameProcess == null)
                {
                    RiotApplicationData.ValorantGameProcess = val;
                    RiotApplicationData.ValorantGameProcess.EnableRaisingEvents = true;
                    RiotApplicationData.ValorantGameProcess.Exited += async (o, args) =>
                    {
                        ClientOpened = false;
                        await CloseRiotRelatedPrograms();
                    };
                    ClientOpened = true;
                    Log.Information("Found Valorant Process");
                    Log.Information($"Process ID: {val.Id}");
                    Log.Information($"Process Name: {val.ProcessName}");
                    OnValorantGameLaunched();
                }
                    

                await Task.Delay(5000);
            }
        }

        public static async void FocusValorant()
        {
            var rProcesses = await RiotClientService.GetCurrentRiotProcesses();
            var vProcess = rProcesses.Where(_p => _p.ProcessName.Contains("VALORANT-Win64-Shipping")).FirstOrDefault();
            WindowsUtils.SetForegroundWindow(vProcess.MainWindowHandle);
            WindowsUtils.ShowWindow(vProcess.MainWindowHandle, 5); // 5 for Windowed Full Screen & Windowed, 3 for Full Screen.
        }
        
        private async void OnValorantGameLaunched()
        {
            Log.Information("Valorant Launched Taking Backup");

            await CreateBackupFile();
            
            Log.Information("Checking if Gamemode is Enabled");
            if (AssistSettings.Default.AppType != AssistApplicationType.LAUNCHER_ONLY)
            {
                Log.Information("Swapping over to Game View as Mode is allowed");
                Dispatcher.UIThread.Invoke(() => {
                    AssistApplication.ChangeMainWindowPopupView(null);
                    AssistApplication.ChangeMainWindowView(new GameInitialStartupView());
                });
                return;
            }

            // Automatically Closes Valorant After Launch
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                AssistApplication.CurrentApplication.Shutdown();
            });
            
        }

        static internal async Task<IEnumerable<Process>> GetCurrentRiotProcesses()
        {
            var processlist = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Where(process => process.Id != Process.GetCurrentProcess().Id).ToList();
            processlist.AddRange(Process.GetProcessesByName("VALORANT-Win64-Shipping"));
            processlist.AddRange(Process.GetProcessesByName("RiotClientServices"));
            return processlist;
        }

        /*private static async Task<bool> DownloadAssistBackgroundVideo()
        {
            var filePath = Path.Combine(AssistSettings.ResourcesFolderPath, "assistBg.mp4");
            if (File.Exists(filePath))
                return false;

            
            var wBClient = new WebClient();
            wBClient.DownloadFileCompleted += WBClientOnDownloadFileCompleted;
            await wBClient.DownloadFileTaskAsync(bgVidUrl, filePath);
            return true;
        }*/

        private static void WBClientOnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            Log.Information("Completed download of BG Video");
        }

        internal static async Task<string> FindRiotClient()
        {
            if (!OperatingSystem.IsWindows())
                return null;

            List<string> clients = new List<string>();

            string riotInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Riot Games/RiotClientInstalls.json"); ;

            if (!File.Exists(riotInstallPath)) return null;

            JsonDocument config;
            try
            {
                config = JsonDocument.Parse(File.ReadAllText(riotInstallPath));
            }
            catch (Exception e)
            {
                Log.Error("Riot Client Check: Could not properly parse json file");
                return null;
            }
            
            if (config.RootElement.TryGetProperty("rc_beta", out JsonElement rcBeta)) { clients.Add(rcBeta.GetString()); }
            if (config.RootElement.TryGetProperty("rc_live", out JsonElement rcLive)) { clients.Add(rcLive.GetString()); }
            if (config.RootElement.TryGetProperty("rc_esports", out JsonElement rcEsports)) { clients.Add(rcEsports.GetString()); }
            if (config.RootElement.TryGetProperty("rc_default", out JsonElement rcDefault)) { clients.Add(rcDefault.GetString()); }

            foreach (var clientPath in clients)
            {
                if (File.Exists(clientPath))
                    return clientPath;
            }

            return null;
        }

        

        public static async Task CloseRiotRelatedPrograms()
        {
            var rProcesses = await RiotClientService.GetCurrentRiotProcesses();
            var rC = rProcesses.Where(_p => _p.ProcessName.Contains("RiotClientServices")).FirstOrDefault();
            var val = rProcesses.Where(_p => _p.ProcessName.Contains("VALORANT-Win64-Shipping")).FirstOrDefault();

            if (val != null) { val.Kill(); }
            if (rC != null) { rC.Kill(); }
        }

        private static readonly string defaultConfigLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data");
        private static readonly string defaultBetaConfigLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Beta", "Data");
        
        public static async Task CreateBackupFile()
        {
            var _currentDataFolderPath = Path.Exists(defaultBetaConfigLocation) ? defaultBetaConfigLocation : defaultConfigLocation;

            var settings = await ReadPrivateSettings(_currentDataFolderPath);

            if (settings is null)
            {
                Log.Error("Failed to create backup, settings were non-existent.");
            }
            
            var subOfClient = await GetSub(settings);
            var ssidOfClient = await GetSSID(settings);
            var accountProfile = AccountSettings.Default.Accounts.Find(x => x.Id == subOfClient);
            
            Log.Information("Attempting to Zip up Data Folder");

                Directory.CreateDirectory(Path.Combine(AccountSettings.BaseFolderPath, "Backups", subOfClient));

                if (File.Exists(Path.Combine(AccountSettings.BaseFolderPath, "Backups", subOfClient,  $"{subOfClient}_data.zip"))) // Deletes Zip File if it exists.
                    File.Delete(Path.Combine(AccountSettings.BaseFolderPath, "Backups", subOfClient,  $"{subOfClient}_data.zip"));

                Log.Information("Delaying... Rito Client slow");
                await Task.Delay(3000);
                
                try
                {
                    ZipFile.CreateFromDirectory(_currentDataFolderPath, Path.Combine(AccountSettings.BaseFolderPath, "Backups", subOfClient,  $"{subOfClient}_data.zip"), CompressionLevel.Fastest, false);
                    accountProfile.BackupZipPath = Path.Combine(AccountSettings.BaseFolderPath, "Backups", subOfClient,
                        $"{subOfClient}_data.zip");
                    accountProfile.UsesBackupZip = true;
                }
                catch (Exception e)
                {
                    Log.Error("Failed to Create Zip Backup");
                    Log.Error("Attempting to Do Code LauncherBackup");

                    if (string.IsNullOrEmpty(ssidOfClient))
                    {
                        Log.Error("Failed to get SSID of User.");
                        Log.Error("DEATHPOINT: Yea i dont know how the fuck we got here tbh.");
                        return;
                    }
                    
                    accountProfile.SaveAccountCAuthCode(ssidOfClient);
                    accountProfile.UsesLauncherCode = true;
                }
                
                Log.Information("Account Modified");
                Log.Information("Updating/Modifying Settings");
                accountProfile.CanLauncherBoot = true;
                await AccountSettings.Default.UpdateAccount(accountProfile);
        }

        public static async Task<ClientPrivateModel?> ReadPrivateSettings(string path)
        {
            ClientPrivateModel settings = new ClientPrivateModel();
            try
            {
                var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                return settings = deserializer.Deserialize<ClientPrivateModel>(File.ReadAllText(Path.Combine(path, "RiotGamesPrivateSettings.yaml")));
            }
            catch (Exception e)
            {
                Log.Error("Failed to Parse Settings after Change");
                Log.Error("Failed to Parse");
                return null;
            }
        }
        
        private static async Task<string> GetSSID(ClientPrivateModel config)
        {
            try
            {
                if (config.RiotLogin?.Persist?.Session?.Cookies != null)
                {
                    var ssid = config.RiotLogin.Persist.Session.Cookies.Find(c => c.name == "ssid");

                    if (ssid != null)
                    {
                        return ssid.value;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            return string.Empty;
        }
        
        private static async Task<string> GetSub(ClientPrivateModel config)
        {
            try
            {
                if (config.RiotLogin?.Persist?.Session?.Cookies != null)
                {
                    var cookie = config.RiotLogin.Persist.Session.Cookies.Find(c => c.name == "sub");

                    if (cookie != null)
                    {
                        return cookie.value;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            return string.Empty;
        }
    }

    struct RiotApplicationData
    {
        public Process RiotClientProcess;
        public Process ValorantGameProcess;
    }
    
}
