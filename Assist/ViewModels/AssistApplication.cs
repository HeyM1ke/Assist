using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Assist.Game.Services;
using Assist.Game.Views;
using Assist.Game.Views.Authentication;
using Assist.Game.Views.Initial;
using Assist.Objects.Enums;
using Assist.Services;
using Assist.Services.Popup;
using Assist.Services.Riot;
using Assist.Services.Server;
using Assist.Settings;
using Assist.Views;
using Assist.Views.Authentication;
using Assist.Views.Settings;
using Assist.Views.Startup;
using Assist.Views.Windows;
using AssistUser.Lib;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Avalonia.Threading;
using Serilog;
using ValNet;
using ValNet.Enums;
using ValNet.Objects;
using ValNet.Objects.Exceptions;
using AssistUser.Lib;
using Avalonia.Controls.Notifications;
using NAudio.Wave;

namespace Assist.ViewModels
{
    internal class AssistApplication
    {
        #region Static Data

        public const string CurrentBattlepassId = "2b3a941d-4b85-a0df-5beb-8897224d290a";

        #endregion

        public AssistApplication()
        {
            AudioOutputDevice = new WaveOutEvent();
        }
        
        public static AssistApplication Current = new AssistApplication();
        public static AssistApiService ApiService = new AssistApiService();
        public RiotUser CurrentUser;
        public ProfileSettings CurrentProfile;
        public ClientSettings ClientLaunchSettings = new ClientSettings();
        public RuntimePlatformInfo Platform;
        public bool GameModeEnabled = false;

        public static ClassicDesktopStyleApplicationLifetime CurrentApplication = Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime;
        
        public void ChangeMainWindowResolution(EResolution res)
        {
            if (CurrentApplication.MainWindow is MainWindow)
            {
                var w = (MainWindow)CurrentApplication.MainWindow;

                if (w != null)
                {
                    w.ChangeResolution(res);
                }
            }
        }

        public void ChangeToGameModeResolution(EResolution res)
        {
            if (CurrentApplication.MainWindow is MainWindow)
            {
                var w = (MainWindow)CurrentApplication.MainWindow;

                if (w != null)
                {
                    w.ChangeGameResolution(res);
                }
            }

            GameModeEnabled = true;
            Dispatcher.UIThread.InvokeAsync(() => MainWindowContentController.Change(new GameInitialView()));
        }

        public void OpenMainWindow()
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Window mainRef = desktop.MainWindow;

                mainRef.Hide();
                
                // Initial Window Opened at launch.
                var main = new MainWindow();

                main.Show();

                mainRef.Close();
                desktop.MainWindow = main;
                MainWindowContentController.Change(new InitialScreen());
            }
        }

        public void OpenMainWindowToSettings()
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Window mainRef = desktop.MainWindow;

                mainRef.Hide();

                // Initial Window Opened at launch.
                var main = new MainWindow();

                main.Show();

                mainRef.Close();
                desktop.MainWindow = main;

                if (AssistApplication.Current.GameModeEnabled)
                {
                    MainWindowContentController.Change(new GameView());
                }
                else
                {
                    MainWindowContentController.Change(new MainView(new SettingsPopup()));
                }
                
                
            }
        }

        public void OpenMainWindowToGameMode()
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Window mainRef = desktop.MainWindow;

                mainRef.Hide();

                // Initial Window Opened at launch.
                var main = new MainWindow();

                main.Show();

                mainRef.Close();
                desktop.MainWindow = main;
                MainWindowContentController.Change(new GameView());
            }
        }

        public void OpenMainView()
        {
            
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    MainWindowContentController.Change(new MainView());
                });
                
            }
            PopupSystem.KillPopups();
        }

        public void OpenGameView()
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    MainWindowContentController.Change(new GameView());
                });

            }
            PopupSystem.KillPopups();
        }

        public void OpenAssistAuthenticationView()
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    MainWindowContentController.Change(new AssistAuthenticationView());
                });

            }
            PopupSystem.KillPopups();
        }

        /// <summary>
        /// Swaps currently logged in profile using profile settings.
        /// </summary>
        /// <param name="nProfile">Profile to switch to.</param>
        public async Task SwapCurrentProfile(ProfileSettings nProfile, bool isRemoval = false)
        {
            Log.Information("Resaving current profile");
            if(!isRemoval)
                await AssistSettings.Current.SaveProfile(Current.CurrentProfile);

            PopupSystem.SpawnPopup(new PopupSettings());
            if (nProfile.isExpired) // If the profile is expired. Open Authentication Depending on their method
            {
                MainWindowContentController.Change(new AuthenticationView());
                PopupSystem.KillPopups();
                return;
            }
            
            
            RiotUser u = new RiotUserBuilder().WithSettings(new RiotUserSettings()
            {
                AuthenticationMethod = AuthenticationMethod.CURL,
            }).Build();
            try
            {
                await AuthenticationController.CookieLogin(nProfile, u);
            }
            catch (ValNetException ex)
            {
                Log.Error("Error Message: " + ex.Message);
                if (ex is RequestException e)
                {
                    Log.Error("Status Code: " + e.StatusCode);
                    Log.Error("Content: " + e.Content);
                }

                nProfile.isExpired = true;
                PopupSystem.KillPopups();
                return;
            }

            await FinishAuthentication(u);


        }
        
        private async Task FinishAuthentication(RiotUser u)
        {
            ProfileSettings pS = new ProfileSettings();

            pS.SetupProfile(u);
            await AssistSettings.Current.SaveProfile(pS);

            pS.ConvertCookiesTo64(u.GetAuthClient().ClientCookies);

            AssistApplication.Current.CurrentUser  = u;
            AssistApplication.Current.CurrentProfile = pS;
            AssistApplication.Current.OpenMainView();
        }

        #region Experimental
        
        public WaveOutEvent AudioOutputDevice;
        public ServerHub ServerHub;
        public RiotUserTokenRefreshService RefreshService;
        public async Task ConnectToServerHub()
        {
            ServerHub = new ServerHub();

            Log.Information("Attempting To Connect to Server");
            ServerHub.Connect();
        }

        public async Task PlaySound(string url)
        {
            using(var mf = new MediaFoundationReader(url))
            using(var wo = new WasapiOut())
            {
                wo.Init(mf);
                wo.Play();
                while (wo.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
            
        }
        
        public async Task Pause()
        {
            AudioOutputDevice.Pause();
        }
        
        public async Task SetVolume(int num)
        {
            AudioOutputDevice.Volume = num;
        }

        public async Task ShowNotification(INotification noti)
        {
            if (AssistApplication.CurrentApplication.MainWindow is MainWindow)
            {
                var w = (MainWindow)AssistApplication.CurrentApplication.MainWindow;

                if (w != null)
                {
                    w.notificationManager.Show(noti);
                }
            }
        }
        
        public async Task ShowNotification(string title, string description, NotificationType type, TimeSpan? duration = null)
        {
            if (AssistApplication.CurrentApplication.MainWindow is MainWindow)
            {
                var w = (MainWindow)AssistApplication.CurrentApplication.MainWindow;

                if (w != null)
                {
                    w.notificationManager.Show(new Notification(title,description, type, duration));
                }
            }
        }
        
        public async Task ShowNotification(string title, string description)
        {
            if (AssistApplication.CurrentApplication.MainWindow is MainWindow)
            {
                var w = (MainWindow)AssistApplication.CurrentApplication.MainWindow;

                if (w != null)
                {
                    w.notificationManager.Show(new Notification(title,description));
                }
            }
        }

        #endregion

        #region Gamemode

        public AssistUser.Lib.AssistUser AssistUser = new AssistUserBuilder().Build();
        public RiotWebsocketService RiotWebsocketService = new RiotWebsocketService();
        public AssistGameServerConnection GameServerConnection = new AssistGameServerConnection();
        
        #endregion


    }
}
