using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Assist.Settings;

namespace Assist.MVVM.ViewModel
{
    internal class LaunchControlViewModel : INotifyPropertyChanged
    {
        

        public LaunchControlViewModel()
        {
            
        }

        public async Task LaunchGame()
        {

            // Create RiotPrivateSettings.yaml for auth
            await AssistApplication.AppInstance.CreateAuthenticationFile();

            // Run RiotCServices with correct parameters, check if custom parameters are entered.

            ProcessStartInfo riotClient = new ProcessStartInfo(UserSettings.Instance.RiotClientInstallPath, " --launch-product=valorant --launch-patchline=live" + UserSettings.Instance.LaunchSettings.CustomValParams);

            var processEntry = Process.Start(riotClient);

            // Concider Settings Panel, Which LNModules are enabled
            if (UserSettings.Instance.LaunchSettings.ValDscRpcEnabled)
            {
                // Run Discord RPC
                await RunAssistBgClient();
            }

            Application.Current.Shutdown();
        }

        public void UpdateDiscordSetting(bool value)
        {
            UserSettings.Instance.LaunchSettings.ValDscRpcEnabled = value;
        }

        public void UpdateParamSetting(string value)
        {
            UserSettings.Instance.LaunchSettings.CustomValParams = value;
        }

        private async Task RunAssistBgClient()
        {
            AssistApplication.AppInstance.Log.Debug("Running BG Client");
            var path = Path.Combine(
               Environment.CurrentDirectory, "Modules", "ABGC",
                "AssistBackgroundClient.exe");
            if (File.Exists(path))
            {
                AssistApplication.AppInstance.Log.Debug("BG Client Found");
                ProcessStartInfo backgroundClient = new ProcessStartInfo(path);
                var processEntry = Process.Start(backgroundClient);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

}
