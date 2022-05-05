using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using Assist;
using Assist.Modules.Popup;
using Assist.MVVM.Model;
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
        private BgClientObj _bgClientObj;

        private string _bgcPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", "AssistBackgroundClient.exe");

        public async Task<bool> LaunchGame()
        {
            Directory.CreateDirectory(_modulesFolder);

            AssistLog.Normal("Finding Assist Background Client");
            _bgClientObj = await AssistApplication.AppInstance.AssistApiController.GetBgClientData();

                if (File.Exists(_bgcPath))
                {
                    AssistLog.Normal("Found Assist Background Client");
                    // Launch BG Client

                    AssistLog.Normal("Checking for BG Client Updates");
                    await CheckForBgClientUpdate();
                    ProcessStartInfo ASSBGINFO = new ProcessStartInfo(_bgcPath, $"--patchline:live --discord:{AssistSettings.Current.LaunchSettings.ValDscRpcEnabled}");
                    ASSBGINFO.UseShellExecute = true;
                    Process.Start(ASSBGINFO);

                    PopupSystem.ModifyCurrentPopup(new PopupSettings()
                    {
                        PopupDescription = $"Enjoy!",
                        PopupTitle = "Launching",
                        PopupType = PopupType.LOADING
                    });

                    Thread.Sleep(1000);
                }
                else
                {
                    AssistLog.Normal("Did not find Assist Background Client");
                    while (!File.Exists(_bgcPath))
                    {
                        AssistLog.Normal("Starting Download of Assist Client");
                        await DownloadBgClient();
                        AssistLog.Normal("Completed Download of Assist Client");
                    }

                    await LaunchGame();
                }

                return true;


        }

        public async Task DownloadBgClient()
        {
            AssistLog.Normal("Downloading Assist Background Client");
            var wBClient = new WebClient();
            // Download BG Client
            wBClient.DownloadProgressChanged += wc_DownloadProgressChanged;
            wBClient.DownloadFileCompleted += WBClientOnDownloadFileCompleted;
            await wBClient.DownloadFileTaskAsync(_bgClientObj.DownloadUrl, _bgcPath);
        }

        private void WBClientOnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            AssistLog.Normal("Completed Download of Assist Client");
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            PopupSystem.ModifyCurrentPopup(new PopupSettings()
            {
                PopupDescription = $"Downloading {e.ProgressPercentage}%",
                PopupTitle = "Downloading Background Client",
                PopupType = PopupType.LOADING
            });
        }

        public async Task CheckForBgClientUpdate()
        {
            if(_bgClientObj == null)
                return;
            
            var fileInfo = FileVersionInfo.GetVersionInfo(_bgcPath);

            AssistLog.Normal("Version of BgClient Detected: " + fileInfo.FileVersion);
            var newV = new Version(_bgClientObj.VersionNumber);
            var currV = new Version(fileInfo.FileVersion);

            if (newV > currV)
            {
                AssistLog.Normal("Newer Version of BgClient Detected, Downloading now. " + newV); 
                File.Delete(_bgcPath); // Delete the old file.

                while (!File.Exists(_bgcPath))
                {
                    AssistLog.Normal("Starting Update Download of Assist Client");
                    await DownloadBgClient();
                    AssistLog.Normal("Completed Update Download of Assist Client");
                }
            }
        }
    }

    
}
