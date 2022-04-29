using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using Assist;
using Assist.Modules.Popup;
using Assist.MVVM.ViewModel;
using Assist.Settings;
using ValNet.Objects;


namespace Assist.Controls.Home.ViewModels
{
    internal class ProfileLaunchViewModel : ViewModelBase
    {
        private ProfileSetting _profile = new ProfileSetting()
        {
            Gamename = "Username",
            Tagline = "00000",
            PCID = "612cd02d-4294-ee2a-644c-a3ba3ddf8805",
            Region = 0
        };
        public ProfileSetting Profile
        {
            get => _profile;
            set => SetProperty(ref _profile, value);
        }
        private string _profileUsername = "DefaultAccount";
        public string ProfileUsername {
            get => _profile.RiotId;
            set => SetProperty(ref _profileUsername, value);
        }
        private string _profileRegion = "NA";
        public string ProfileRegion
        {
            get => Enum.GetName(typeof(RiotRegion), _profile.Region);
            set => SetProperty(ref _profileRegion, value);
        }
        private int _profileAccountLevel = 420;
        public int ProfileAccountLevel
        {
            get => _profile.playerLevel;
            set => SetProperty(ref _profileAccountLevel, value);
        }
        private BitmapImage _profilePlayercard;
        public BitmapImage ProfilePlayercard
        {
            get => _profilePlayercard;
            set => SetProperty(ref _profilePlayercard, value);
        }
        private BitmapImage _backingImage;
        public BitmapImage BackingImage
        {
            get => _backingImage;
            set => SetProperty(ref _backingImage, value);
        }

        private string _modulesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules");

        private string _bgcPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", "AssistBackground.exe");
        public async Task LaunchGame()
        {
            Directory.CreateDirectory(_modulesFolder);

            if (File.Exists(_bgcPath))
            {
                // Launch BG Client
                ProcessStartInfo ASSBGINFO =
                    new ProcessStartInfo(_bgcPath, $"--patchline:live --discord:{AssistSettings.Current.LaunchSettings.ValDscRpcEnabled}");

                ASSBGINFO.UseShellExecute = true;
                Process.Start(ASSBGINFO);

                Thread.Sleep(1000);
            }
            else
            {
                while (!File.Exists(_bgcPath))
                {
                    await DownloadBgClient();
                }
            }
        }

        public async Task DownloadBgClient()
        {
            
            // Download BG Client
            using (WebClient wc = new WebClient())
            {
                //wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFile(new Uri("https://cdn.assistapp.dev/Static/Development/AssistBackground.exe"), _bgcPath);
            }   
        }
        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            PopupSystem.ModifyCurrentPopup(new PopupSettings()
            {
                PopupTitle = "Downloading Assist BGC",
                PopupDescription = $"Download Progress: {e.ProgressPercentage}%",
                PopupType = PopupType.LOADING
            });
        }
    }

    
}
