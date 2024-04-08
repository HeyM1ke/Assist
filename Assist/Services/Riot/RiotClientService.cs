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
using Assist.Controls.General;
using Assist.Controls.Navigation;
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
        public static RiotClientService Instance;
        public static event EventHandler ValorantGameLaunched;
        public static event EventHandler RiotClientLaunched;

        private Process? _riotClient;
        private string RiotClientLocation;
        public static bool ClientOpened = false;
        private BackgroundWorker _worker;
        public RiotApplicationData RiotApplicationData = new RiotApplicationData();
        public string AdditionalRiotClientArguments = string.Empty;

        private const string bgVidUrl = "https://cdn.rumblemike.com/Static/live/assistBackVideo_dev.mp4";

        public RiotClientService()
        {
            Instance = this;
            Log.Information($"Created RiotClientService Instance");
        }
        
        public async Task ApplyLauncherFiles()
        {
            if (string.IsNullOrEmpty(AssistApplication.ActiveAccountProfile.BackupZipPath))
            {
                var possiblePath = Path.Combine(AccountSettings.BaseFolderPath, "Backups", AssistApplication.ActiveAccountProfile.Id, $"{AssistApplication.ActiveAccountProfile.Id}_data.zip");
                if (File.Exists(possiblePath))
                    AssistApplication.ActiveAccountProfile.BackupZipPath = possiblePath;
            }
            
            
            if (!File.Exists(AssistApplication.ActiveAccountProfile.BackupZipPath))
            {
                Log.Error("Launcher files dont exist");

                AssistApplication.ShowcaseErrorMessage("Required Files do not exist, please repair the profile.\nWithin Profile Management");
                NavigationContainer.ViewModel.EnableAllButtons();
                return;
            }
        
            Log.Information("Backup File Exists.");
            try
            {
                await RiotClientService.CloseRiotRelatedPrograms();
                Log.Information("Deleting Files");
                RemoveAnyExistingClientDataFiles();
                
                Log.Information("Deciding Current Data Path");
                var _currentDataFolderPath = Path.Exists(defaultBetaConfigLocation) ? defaultBetaConfigLocation : defaultConfigLocation;
                Log.Information($"Current Data Path: {_currentDataFolderPath}" );
                Log.Information("Extracting to Directory");
                ZipFile.ExtractToDirectory(AssistApplication.ActiveAccountProfile.BackupZipPath, _currentDataFolderPath, true);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                Log.Error(e.StackTrace);

                AssistApplication.ShowcaseErrorMessage("Error Occured while Launching", $"{e.Message} \n \n Please open a ticket on the discord, if this issue persists.");
                return;
            }
            
            await StartClient();
        }

        public async Task StartClient(string profileCurrentId = "")
        {
            
            string clientLocation = await RiotClientService.FindRiotClient();

            if (clientLocation == null)
            {
                Log.Error("DID NOT FIND CLIENT");
                AssistApplication.ShowcaseErrorMessage("No Riot Client Found", $"We could not find any Riot Client install, please install or repair your Riot Client");
                return;
            }
                

            Log.Information("Attempting to Launch VALORANT.");
            
            ProcessStartInfo riotClientStart = new ProcessStartInfo(clientLocation, $"--launch-product=valorant --launch-patchline=live")
            {
                UseShellExecute = true
            };
            
            Log.Information($"Launch Path: {clientLocation}");
            Log.Information($"Launch Args: {riotClientStart.Arguments}");
            
            Log.Information($"Starting Process: {riotClientStart.FileName}");
            _riotClient = Process.Start(riotClientStart);
            await Task.Delay(1000);
            var serv = new RiotClientService();
            serv.StartWorker();
        }

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
            Log.Information("RCS: Background Worker Started");
            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += _worker_DoWork;
            
            _worker.RunWorkerAsync();
        }
        
        public void StopWorker()
        {
            Log.Information("Background Worker Attempting to Stop");
            _worker.CancelAsync();
        }

        private async void _worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            while (!ClientOpened && !_worker.CancellationPending)
            {
                Log.Information("RCS: Worker Background Work Start");
                var rProcesses = await GetCurrentRiotProcesses();
                var rC = rProcesses.Where(_p => _p.ProcessName.Contains("RiotClientServices")).FirstOrDefault();
                var val = rProcesses.Where(_p => _p.ProcessName.Contains("VALORANT-Win64-Shipping")).FirstOrDefault();

                if (rC != null && RiotApplicationData.RiotClientProcess == null)
                {
                    Log.Information("RCS: Found Riot Client Process");
                    RiotApplicationData.RiotClientProcess = rC;

                    try
                    {
                        RiotApplicationData.RiotClientProcess.EnableRaisingEvents = true;
                    }
                    catch (Exception exception)
                    {
                        Log.Error("RCS: Failed to Enable Raise Events for RC");
                        Log.Error($"RCS: RC ERR MESSAGE; {exception.Message}");
                        Log.Error($"RCS: RC ERR STACK; {exception.StackTrace}");
                    }
                    
                    Log.Information("Found Riot Client Process");
                    Log.Information($"Process ID: {rC.Id}");
                    Log.Information($"Process Name: {rC.ProcessName}");
                    RiotClientLaunched?.Invoke(this, e);
                }

                if (val != null && RiotApplicationData.ValorantGameProcess == null)
                {
                    RiotApplicationData.ValorantGameProcess = val;
                    
                    try
                    {
                        RiotApplicationData.ValorantGameProcess.EnableRaisingEvents = true;
                    }
                    catch (Exception exception)
                    {
                        Log.Error("RCS: Failed to Enable Raise Events for VAL");
                        Log.Error($"RCS: VAL ERR MESSAGE; {exception.Message}");
                        Log.Error($"RCS: VAL ERR STACK; {exception.StackTrace}");
                    }
                    
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
            Log.Information("RCS: Valorant has Launched");
            Log.Information("RCS: Creating BackupFile");

            await CreateBackupFile();
            
            Log.Information("RCS: Checking if Gamemode is Enabled");
            if (AssistSettings.Default.AppType != AssistApplicationType.LAUNCHER_ONLY)
            {
                Log.Information("RCS: Swapping over to Game View as Mode is allowed");
                Dispatcher.UIThread.Invoke(() => {
                    AssistApplication.ChangeMainWindowPopupView(null);
                    AssistApplication.ChangeMainWindowView(new GameInitialStartupView());
                });
                return;
            }

            // Automatically Closes Valorant After Launch
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Log.Information("RCS: Gamemode is not enabled, Shutting Assist Down.");
                AssistApplication.CurrentApplication.Shutdown();
            });
            
        }

        static internal async Task<IEnumerable<Process>> GetCurrentRiotProcesses()
        {
            Log.Information("RCS: Checking for Riot Processes");
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

        public void RemoveAnyExistingClientDataFiles()
        {
            if (Directory.Exists(defaultConfigLocation))
            {
                Log.Information("Found Directory of Default Config Client, Removing Files");
                DirectoryInfo di = new DirectoryInfo(defaultConfigLocation);
                foreach (var filePath in di.GetFiles())
                {
                    filePath.Delete();
                }
                // removed any currently logged in client
                Log.Information("Found Directory of Default Config Client, Removed Files");
            }

            if (Directory.Exists(defaultBetaConfigLocation))
            {
                Log.Information("Found Directory of Beta Config Client, Removing Files");
                DirectoryInfo di = new DirectoryInfo(defaultBetaConfigLocation);
                foreach (var filePath in di.GetFiles())
                {
                    filePath.Delete();
                }
                // removed any currently logged in client
                Log.Information("Found Directory of Beta Config Client, Removed Files");
            }
        }

        internal static async Task<string> FindRiotClient()
        {
            Log.Information("Finding Riot Client");
            if (!OperatingSystem.IsWindows())
                return null;

            List<string> clients = new List<string>();

            Log.Information("Finding Riot Client Installs Data");
            string riotInstallPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Riot Games/RiotClientInstalls.json");
            ;

            if (!File.Exists(riotInstallPath)) return null;

            JsonDocument config;
            try
            {
                Log.Information("Parsing Riot Client Config Data");
                config = JsonDocument.Parse(File.ReadAllText(riotInstallPath));
            }
            catch (Exception e)
            {
                Log.Error("Riot Client Check: Could not properly parse json file");
                Log.Error("RCC MESSAGE: " + e.Message);
                Log.Error("RCC STACK: " + e.StackTrace);
                return null;
            }


            if (config.RootElement.TryGetProperty("rc_live", out JsonElement rcLive))
            {
                Log.Information("Found rc_live");
                clients.Add(rcLive.GetString());
            }

            // Beta has second priority due to issues...
            if (config.RootElement.TryGetProperty("rc_beta", out JsonElement rcBeta))
            {
                Log.Information("Found rc_beta");
                clients.Add(rcBeta.GetString());
            }

            if (config.RootElement.TryGetProperty("rc_esports", out JsonElement rcEsports))
            {
                Log.Information("Found rc_esports");
                clients.Add(rcEsports.GetString());
            }

            if (config.RootElement.TryGetProperty("rc_default", out JsonElement rcDefault))
            {
                Log.Information("Found rc_default");
                clients.Add(rcDefault.GetString());
            }

            // Expected behavior is to find Live & Default.
            foreach (var clientPath in clients)
            {
                Log.Information("Checking for Client Path: " + clientPath);
                if (File.Exists(clientPath))
                {
                    Log.Information("Client Path found: " + clientPath);
                    return clientPath;
                }
            }
            
            
            Log.Error("No Client Paths were found, returning Null");
            return null;
            
        }



        public static async Task CloseRiotRelatedPrograms()
        {
            Log.Information("Closing any Riot Related Programs");
            var rProcesses = await RiotClientService.GetCurrentRiotProcesses();
            var rC = rProcesses.Where(_p => _p.ProcessName.Contains("RiotClientServices")).FirstOrDefault();
            var val = rProcesses.Where(_p => _p.ProcessName.Contains("VALORANT-Win64-Shipping")).FirstOrDefault();
            Log.Information("Finished Searching for Riot Programs");
            if (val != null)
            {
                Log.Information("Found VALORANT-Win64-Shipping, Killing VALORANT-Win64-Shipping");
                val.Kill();
            }

            if (rC != null)
            {
                Log.Information("Found RiotClientServices, Killing RiotClientServices");
                rC.Kill();
            }
            
            Log.Information("Completed Closing Riot Programs");
        }

        private static readonly string defaultConfigLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data");
        private static readonly string defaultBetaConfigLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Beta", "Data");
        
        
        public static async Task CreateBackupFile()
        {
            Log.Information("RCS: Checking for DataPath");
            var _currentDataFolderPath = Path.Exists(defaultBetaConfigLocation) ? defaultBetaConfigLocation : defaultConfigLocation;
            Log.Information($"RCS: Data Path being used: {_currentDataFolderPath}");
            Log.Information($"RCS: Reading Private Settings");
            var settings = await ReadPrivateSettings(_currentDataFolderPath);

            if (settings is null)
            {
                Log.Error("Failed to create backup, settings were non-existent.");
                return;
            }
            
            var subOfClient = await GetSub(settings);
            var ssidOfClient = await GetSSID(settings);
            Log.Information($"RCS: Finding Profile that is equal to current launch of profile.");
            var accountProfile = AccountSettings.Default.Accounts.Find(x => x.Id == subOfClient);
            
            Log.Information("RCS: Attempting to Zip up Data Folder");

            Log.Information($"RCS: Creating DIR for Backup for User");
            Directory.CreateDirectory(Path.Combine(AccountSettings.BaseFolderPath, "Backups", subOfClient));

            if (File.Exists(Path.Combine(AccountSettings.BaseFolderPath, "Backups", subOfClient,  $"{subOfClient}_data.zip"))) // Deletes Zip File if it exists.
                File.Delete(Path.Combine(AccountSettings.BaseFolderPath, "Backups", subOfClient,  $"{subOfClient}_data.zip"));

            Log.Information("RCS: Delaying... Rito Client slow");
            await Task.Delay(3000);
                
                try
                {
                    Log.Information($"RCS: Creating Zip file Backup");
                    ZipFile.CreateFromDirectory(_currentDataFolderPath, Path.Combine(AccountSettings.BaseFolderPath, "Backups", subOfClient,  $"{subOfClient}_data.zip"), CompressionLevel.Fastest, false);
                    if (accountProfile != null)
                    {
                        accountProfile.BackupZipPath = Path.Combine(AccountSettings.BaseFolderPath, "Backups",
                            subOfClient, $"{subOfClient}_data.zip");
                        accountProfile.UsesBackupZip = true;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("RCS: Failed to Create Zip Backup");
                    Log.Error("RCS: Attempting to Do Code LauncherBackup");

                    if (string.IsNullOrEmpty(ssidOfClient))
                    {
                        Log.Error("RCS:Failed to get SSID of User.");
                        Log.Error("RCS: DEATHPOINT: Yea i dont know how the fuck we got here tbh.");
                        return;
                    }

                    if (accountProfile != null)
                    {
                        accountProfile.SaveAccountCAuthCode(ssidOfClient);
                        accountProfile.UsesLauncherCode = true;
                    }
                }
                
                if (accountProfile != null)
                {
                    Log.Information("RCS: Account Modified");
                    Log.Information("RCS: Updating/Modifying Settings");
                    accountProfile.CanLauncherBoot = true;
                    await AccountSettings.Default.UpdateAccount(accountProfile);
                }
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
                Log.Error("RCS: Failed to Parse Settings after Change");
                Log.Error("RCS: Failed to Parse");
                return null;
            }
        }
        
        private static async Task<string> GetSSID(ClientPrivateModel config)
        {
            try
            {
                Log.Information($"RCS: Getting SSID Private Settings Value");
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
                Log.Information($"RCS: Getting SUB Private Settings Value");
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
