using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Assist.Game.Views.Initial;
using Assist.Objects.RiotClient;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia.Threading;
using Serilog;

namespace Assist.Services.Riot
{
    internal class RiotClientService
    {
        public static event EventHandler ValorantGameLaunched;
        public static event EventHandler RiotClientLaunched;

        private string RiotClientLocation;
        public static bool ClientOpened = false;
        private BackgroundWorker _worker;
        public static RiotApplicationData RiotApplicationData = new RiotApplicationData();

        private const string bgVidUrl = "https://cdn.rumblemike.com/Static/live/assistBackVideo_dev.mp4";
        public RiotClientService()
        {
            
        }

        public async Task<bool> LaunchClient()
        {
            Log.Information("Attempting to Start Client");

            Log.Information("Killing all riots");
            await CloseRiotRelatedPrograms();

            Log.Information("Trying to find client location");
            string clientLocation = await AssistSettings.Current.FindRiotClient();
            if (clientLocation == null){
                Log.Error("NOT FOUND");
                return false;
            }
            Log.Information("Creating Auth Files");
            await CreateAuthenticationFile();

            Log.Information("Attempting to Launch Client");
            ProcessStartInfo riotClientStart = new ProcessStartInfo(clientLocation, $"--launch-product=valorant --launch-patchline={AssistApplication.Current.ClientLaunchSettings.Patchline.ToLower()} --insecure")
            {
                UseShellExecute = true
            };

            Process.Start(riotClientStart);
            await Task.Delay(1000); // Delay for the Client to Start
            StartWorker();

            return true;
        }

        public async Task CreateAuthenticationFile()
        {
            string pSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data", "RiotGamesPrivateSettings.yaml");
            string pSettingsPathBackup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data", "RiotClientPrivateSettings.yaml");
            string pClientSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Config", "RiotClientSettings.yaml");

            string riotClient = await AssistSettings.Current.FindRiotClient();
            var fileInfo = FileVersionInfo.GetVersionInfo(riotClient);
            var cSettings = new ClientPrivate(AssistApplication.Current.CurrentUser);

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
                    cSettings.CreateGameModelWRegion().Save(writer, false);
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

        private void StartWorker()
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
                    RiotApplicationData.ValorantGameProcess.Exited += (o, args) => ClientOpened = false;
                    ClientOpened = true;
                    Log.Information("Found Valorant Process");
                    Log.Information($"Process ID: {val.Id}");
                    Log.Information($"Process Name: {val.ProcessName}");
                    OnValorantGameLaunched();
                }
                    

                await Task.Delay(5000);
            }
        }

        private async void OnValorantGameLaunched()
        {
            Log.Information("Valorant Launched Taking Backup");
            BackupsSettings.SaveBackup(new BackupModel()
            {
                PlayerUuid = AssistApplication.Current.CurrentUser.UserData.sub,
                DataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data"),
                ConfigFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Config"),
            });


            //EXPERIMENTAL EXPERIMENTAL EXPERIMENTAL DO NOT TOUCH
            /*var t = await DownloadAssistBackgroundVideo();
            if(t)
                OnValorantGameLaunched();*/
            //await ReplaceValorantBackground();


            

            if (AssistSettings.Current.GameModeEnabled)
            {
                Dispatcher.UIThread.InvokeAsync(() => { MainWindowContentController.Change(new GameInitialView()); });
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

        private static async Task<bool> DownloadAssistBackgroundVideo()
        {
            var filePath = Path.Combine(AssistSettings.ResourcesFolderPath, "assistBg.mp4");
            if (File.Exists(filePath))
                return false;

            
            var wBClient = new WebClient();
            wBClient.DownloadFileCompleted += WBClientOnDownloadFileCompleted;
            await wBClient.DownloadFileTaskAsync(bgVidUrl, filePath);
            return true;
        }

        private static void WBClientOnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            Log.Information("Completed download of BG Video");
        }

        private static async Task ReplaceValorantBackground()
        {
            var filePath = Path.Combine(AssistSettings.ResourcesFolderPath, "assistBg.mp4");
            if (!File.Exists(filePath))
                return;

            var TEMPPATHLOCATION = @"C:\Riot Games\VALORANT\live\ShooterGame\Content\Movies\Menu";
            var vid = Path.Combine(TEMPPATHLOCATION, "HomeScreen.mp4");
            var vidClone = Path.Combine(TEMPPATHLOCATION, "HomeScreen_O.mp4");

            await Task.Delay(5000);
            try
            {
                System.IO.File.Move(vid, vidClone);
                File.Copy(filePath, vid, true);
            }
            catch (Exception e)
            {
                    Log.Error("Error! " + e.Message);
            }
            

        }

        private async Task CloseRiotRelatedPrograms()
        {
            var rProcesses = await RiotClientService.GetCurrentRiotProcesses();
            var rC = rProcesses.Where(_p => _p.ProcessName.Contains("RiotClientServices")).FirstOrDefault();
            var val = rProcesses.Where(_p => _p.ProcessName.Contains("VALORANT-Win64-Shipping")).FirstOrDefault();

            if (val != null) { val.Kill(); }
            if (rC != null) { rC.Kill(); }
        }
    }

    struct RiotApplicationData
    {
        public Process RiotClientProcess;
        public Process ValorantGameProcess;
    }
    
}
