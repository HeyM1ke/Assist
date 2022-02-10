using Assist.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ValNet.Objects.Authentication;
namespace Assist.MVVM.ViewModel
{
    internal class AssistLaunchControlViewModel : INotifyPropertyChanged
    {
        // live , pbe
        string currentPatchlineAddress;
        public int currentSelectedPatchline;
        public List<PatchlineObj> entitledPatchlines = new List<PatchlineObj>();

        // Populate the patchlines combobox with current users entitlements
        public async Task GetUserPatchlines() {
            entitledPatchlines.Clear();
            entitledPatchlines.Add(new()
            {
                PatchlineName = "Live",
                PatchlinePath = "live"
            });

            var entitlements = await AssistApplication.AppInstance.currentUser.Authentication.GetPlayerGameEntitlements();

            foreach (var entitlement in entitlements)
            {
                entitledPatchlines.Add(entitlement);
            }
        }

        // Create a method to open launch settings menu

        // TO:DO

        // Launch the game with concideration with settings by user and patchline <----------------



        public async void LaunchClient()
        {
            currentPatchlineAddress = entitledPatchlines[currentSelectedPatchline].PatchlinePath;

            // Create RiotPrivateSettings.yaml for auth
            await AssistApplication.AppInstance.CreateAuthenticationFile();

            // Run RiotCServices with correct parameters, check if custom parameters are entered.
            ProcessStartInfo riotClient;

            /*if (UserSettings.Instance.LaunchSettings.EnableCustomParams)
                riotClient = new ProcessStartInfo(UserSettings.Instance.RiotClientInstallPath, $" --launch-product=valorant --launch-patchline={currentPatchlineAddress}" + UserSettings.Instance.LaunchSettings.CustomValParams);
            else*/

            

            riotClient = new ProcessStartInfo(UserSettings.Instance.RiotClientInstallPath, $" --launch-product=valorant --launch-patchline={currentPatchlineAddress}");

            // This line is a fallback to make sure if a user has RiotClient running in admin, the program does not fail to launch the client.
            riotClient.UseShellExecute = true;


            var processEntry = Process.Start(riotClient);

            // Concider Settings Panel, Which LNModules are enabled
            if (true)
            {
                // Run Discord RPC
                await RunAssistBgClient();
            }

            Application.Current.Shutdown();
        } 

        private async Task RunAssistBgClient()
        {
            AssistApplication.AppInstance.Log.Debug("Running BG Client");
            var path = Path.Combine(
               Environment.CurrentDirectory,"ABGC",
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
