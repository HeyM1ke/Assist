using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Assist.Models.Riot;
using Assist.Services.Riot;
using Assist.Shared.Settings;
using Assist.Shared.Settings.Accounts;
using AssistUser.Lib.V2;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using YamlDotNet.Serialization.NamingConventions;

namespace Assist.ViewModels.RAccount;

public partial class RAccountSecondaryClientLoginViewModel : ViewModelBase
{
    [ObservableProperty] private ICommand? _loginCompletedCommand;
    [ObservableProperty] private bool _isProcessing = false;
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private bool _errorMessageVisible = false;

    
    private static readonly string defaultConfigLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data");
    private static readonly string defaultBetaConfigLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Beta", "Data");
    private Process _riotClientProcess;
    private FileSystemWatcher _authFileWatcher;
    private string _currentDataFolderPath;
    
    [RelayCommand]
    public async Task ClientLaunchButtonPressed()
    {
        Log.Information("Client Launch Button Launched");
        ErrorMessageVisible = false;
        IsProcessing = true;
        
        await StartRiotClient();
        await StartWatcher();
        
    }
    
     public async Task StartRiotClient()
        {
            await RiotClientService.CloseRiotRelatedPrograms();
            
            ClearPreviousConfig();

            string clientLocation = await RiotClientService.FindRiotClient();
            
            if (clientLocation == null)
                Log.Error("DID NOT FIND CLIENT");

            
            
            ProcessStartInfo riotClientStart = new ProcessStartInfo(clientLocation, $"--launch-product=valorant --allow-multiple-clients") { UseShellExecute = true };

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
                    try
                    {
                        _riotClientProcess.EnableRaisingEvents = true;
                    }
                    catch (Exception e)
                    {
                        
                    }
                    return;
                }

                await Task.Delay(1500);
            }

        }

        private void ClearPreviousConfig()
        {
            // Check if there is a Riot Games file already.

            if (Directory.Exists(defaultConfigLocation))
            {
                DirectoryInfo di = new DirectoryInfo(defaultConfigLocation);
                foreach (var filePath in di.GetFiles())
                {
                    filePath.Delete();
                }
                // removed any currently logged in client
            }

            if (Directory.Exists(defaultBetaConfigLocation))
            {
                DirectoryInfo di = new DirectoryInfo(defaultBetaConfigLocation);
                foreach (var filePath in di.GetFiles())
                {
                    filePath.Delete();
                }
                // removed any currently logged in client
            }
        }

        [RelayCommand]
        public void CancelRiotClient()
        {
            _authFileWatcher.EnableRaisingEvents = false;
            _authFileWatcher.Dispose();

            IsProcessing = false;
        }


        public async Task StartWatcher()
        {
            _currentDataFolderPath = Path.Exists(defaultBetaConfigLocation) ? defaultBetaConfigLocation : defaultConfigLocation;
            
            _authFileWatcher = new FileSystemWatcher(_currentDataFolderPath, "RiotGamesPrivateSettings.yaml");

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
            await CheckForLogin();
        }

        private async Task CheckForLogin()
        {
            ClientPrivateModel settings = new ClientPrivateModel();
            try
            {
                var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                settings = deserializer.Deserialize<ClientPrivateModel>(File.ReadAllText(Path.Combine(_currentDataFolderPath, "RiotGamesPrivateSettings.yaml")));
            }
            catch (Exception e)
            {
                Log.Error("Failed to Parse Settings after Change");
                Log.Error("Failed to Parse");
                return;
            }

            var check = await CheckSettings(settings);

            if (check)
            {
                Log.Information("SSID of ClientPrivate Settings has been found");
                Log.Information("Killing Riot Client");

                _authFileWatcher.Changed -= AuthFileWatcherOnChanged;
                _authFileWatcher.Dispose();
                
                await RiotClientService.CloseRiotRelatedPrograms();
                
                Log.Information("Locating Account Settings");
                var subOfClient = await GetSub(settings);
                var ssidOfClient = await GetSSID(settings);
                var accountProfile = AccountSettings.Default.Accounts.Find(x => x.Id == subOfClient);

                if (accountProfile is null)
                {
                    Log.Error("Player Logged into an account which is NOT apart of the assist account line.");
                    
                    ErrorMessage = "Account you logged into, is not the valid account. Please try again.";
                    ErrorMessageVisible = true;
                    return;
                }
                
                Log.Information("Profile exists, continuing process.");
                
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
                        ErrorMessage = "Something went extremely wrong. DEATHPOINT:HTFDTR";
                        ErrorMessageVisible = true;
                        return;
                    }
                    
                    accountProfile.SaveAccountCAuthCode(ssidOfClient);
                    accountProfile.UsesLauncherCode = true;
                }
                
                Log.Information("Account Modified");
                Log.Information("Updating/Modifying Settings");
                accountProfile.CanLauncherBoot = true;
                await AccountSettings.Default.UpdateAccount(accountProfile);
                AssistApplication.ActiveAccountProfile = accountProfile; // Sets this as the updated one.
                ClearPreviousConfig();
                LoginCompletedCommand?.Execute("");
            }
            else
            {
                var badLogin = await CheckForBadLogin(settings);

                if (badLogin)
                {
                    // If the login is bad (AKA: User forgot to put Remember me but logged in.
                    // Close Riot Client, Clear the Settings, and Showcase a message that states to click Remember Me.
                    await RiotClientService.CloseRiotRelatedPrograms();
                    _authFileWatcher.Changed -= AuthFileWatcherOnChanged;
                    _authFileWatcher.Dispose();
                    
                    ClearPreviousConfig();
                    
                    // Showcase Message/Window stating issue.
                    ErrorMessage = "Login Failed! Please Remember to Select Remember Me Options!";
                    ErrorMessageVisible = true;
                    IsProcessing = false; // Unhides UI to progress
                }
                
                Log.Error("Parse Passed but failed Check");
                Log.Error("APOINT: PPBFC");
                return;
            }
        }

        private async Task<bool> CheckForBadLogin(ClientPrivateModel config)
        {
            try
            {
                if (config.RiotLogin?.Persist?.Session?.Cookies != null)
                {
                    var tdid = config.RiotLogin.Persist.Session.Cookies.Find(c => c.name == "tdid");

                    return tdid != null;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            return false;
        }

        private async Task<bool> CheckSettings(ClientPrivateModel config)
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
        
        private async Task<string> GetSSID(ClientPrivateModel config)
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
        
        private async Task<string> GetSub(ClientPrivateModel config)
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