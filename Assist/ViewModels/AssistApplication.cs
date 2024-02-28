using System;
using System.Threading;
using System.Threading.Tasks;
using Assist.Controls.Infobars;
using Assist.Controls.Navigation;
using Assist.Models.Enums;
using Assist.Services;
using Assist.Services.Assist;
using Assist.Services.Navigation;
using Assist.Services.Riot;
using Assist.Shared.Services.Utils;
using Assist.Shared.Settings.Accounts;
using Assist.Views;
using Assist.Views.Dashboard;
using Assist.Views.Extras;
using Assist.Views.Game.Live;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Serilog;
using ValNet;
using AssistUser.Lib;
using AssistUser.Lib.V2;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using Velopack;

namespace Assist.ViewModels;

public static class AssistApplication
{
    public static EAssistMode CurrentMode = EAssistMode.NONE;
    public static ClassicDesktopStyleApplicationLifetime CurrentApplication = Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime;
    private static Mutex _mutex = null;

    public static RiotUser? ActiveUser;
    public static AssistApiService AssistApiService = new AssistApiService();
    public static AccountProfile? ActiveAccountProfile;
    public static AUser? AssistUser = new AUserBuilder().Build();
    public static AssistGameServerConnection? Server = new AssistGameServerConnection();
    public static ValorantWebsocketClient? WebsocketClient;
    
    //TODO: Move to proper class
    public static bool CurrentlyInAssistLeagueMatch = false;
    public static bool RequestedClose = false;
    public static ValorantWebsocketClient RiotWebsocketService { get; set; } = new ValorantWebsocketClient();
    public static RiotUserTokenRefreshService RefreshService { get; set; } = new RiotUserTokenRefreshService();

    public static void ChangeMainWindowResolution(EResolution res)
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
    
    public static void ChangeMainWindowView(UserControl NewView)
    {
        CurrentApplication = Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime;
        if (CurrentApplication.MainWindow is MainWindow)
        {
            var w = (MainWindow)CurrentApplication.MainWindow;
            if (w != null) { w.ChangeView(NewView); }
        }
    }
    
    public static void ChangeMainWindowPopupView(UserControl NewView)
    {
        if (CurrentApplication.MainWindow is MainWindow)
        {
            var w = (MainWindow)CurrentApplication.MainWindow;
            if (w != null) { w.ChangePopupView(NewView); }
        }
    }

    public static void CreateInstance()
    {
        const string appName = "Assist";
        bool createdNew;

        _mutex = new Mutex(true, appName, out createdNew);

        if (!createdNew)
        {
            //app is already running! Exiting the application
                
            WindowsUtils.ShowToFront(appName);
                
            AssistApplication.CurrentApplication.Shutdown();
        }
        GC.KeepAlive(_mutex);
    }
    
    public static async Task ShowNotification(INotification noti)
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
        
    public static async Task ShowNotification(string title, string description, NotificationType type, TimeSpan? duration = null)
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
        
    public static async Task ShowNotification(string title, string description)
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

    public static async Task SwapAssistMode(EAssistMode newMode)
    {

        if (newMode == CurrentMode)
            return;

        Dispatcher.UIThread.Invoke(() =>
        {
            NavigationContainer.ViewModel.HideAllButtons();
        });
        

        CurrentMode = newMode;
        
        switch (newMode)
        {
          case  EAssistMode.GAME:
              Titlebar.ViewModel.AccountSwapVisible = false;
              break;
          case EAssistMode.LAUNCHER:
              break;
        }
        
    }
    
    /// <summary>
    /// Method is ran after any successful profile swap or authentication.
    /// </summary>
    public static async Task SetupComplete_Launcher()
    {
        
        await Titlebar.ViewModel.ShowcaseProfile(AssistApplication.ActiveAccountProfile);
        await SwapAssistMode(EAssistMode.LAUNCHER);
        
        Dispatcher.UIThread.Invoke(() =>
        {
            Titlebar.ViewModel.AccountSwapVisible = true;
            Titlebar.ViewModel.AccountSwapEnabled = true;
            Titlebar.ViewModel.SettingsEnabled = true;
            EnableDefaultLauncherButtons();
            NavigationContainer.ViewModel.ChangePage(AssistPage.DASHBOARD);
        });
        
        
    }
    
    /// <summary>
    /// Method is ran after any successful profile swap or authentication.
    /// </summary>
    public static async Task SetupComplete_Game()
    {
        
        await SwapAssistMode(EAssistMode.GAME); // Just to confirm
        Dispatcher.UIThread.Invoke(() =>
        {
            Titlebar.ViewModel.AccountSwapVisible = false;
            Titlebar.ViewModel.SettingsEnabled = true;
            NavigationContainer.ViewModel.ChangePage(AssistPage.LIVE);
            EnableDefaultGameButtons();
            
        });
    }

    public static void EnableDefaultLauncherButtons()
    {
        NavigationContainer.ViewModel.EnableButton(AssistPage.DASHBOARD);
        NavigationContainer.ViewModel.EnableButton(AssistPage.STORE);
#if DEBUG
        NavigationContainer.ViewModel.EnableButton(AssistPage.MODULES);
#endif
        NavigationContainer.ViewModel.EnableAllButtons();
    }
    
    public static void EnableDefaultGameButtons()
    {
        NavigationContainer.ViewModel.EnableButton(AssistPage.LIVE);
#if DEBUG
        NavigationContainer.ViewModel.EnableButton(AssistPage.MODULES);
#endif
        //NavigationContainer.ViewModel.EnableButton(AssistPage.MODULES); not yet lmao
        NavigationContainer.ViewModel.EnableAllButtons();
    }
    
    public static void OpenUpdateWindow()
    {
        if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Window mainRef = desktop.MainWindow;

            mainRef.Hide();
                
            // Initial Window Opened at launch.
            var main = new UpdateWindow();

            main.Show();

            mainRef.Close();
            desktop.MainWindow = main;
            
        }
    }
    
    public static async Task<bool> CheckForUpdates()
    {
#if (!DEBUG)

            if (OperatingSystem.IsWindows())
            {
                try
                {
                    var mgr = new UpdateManager("https://cdn.assistval.com/releases/live/win/");
                    var newVer = await mgr.CheckForUpdatesAsync();
                    if (newVer == null)
                        return false;
                    
                   return true;
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }

            if (OperatingSystem.IsMacOS())
            {
                try
                {
                    var mgr = new UpdateManager("https://cdn.assistval.com/releases/live/mac/");
                    var newVer = await mgr.CheckForUpdatesAsync();
                    if (newVer == null)
                        return false;
                    
                    return true;

                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }
            
#endif

        return false;

    }

    public static void OpenMainWindowToSettings()
    {
        
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Window mainRef = desktop.MainWindow;

                mainRef.Hide();

                // Initial Window Opened at launch.

                Dispatcher.UIThread.Invoke(() =>
                {
                    var main = new MainWindow();

                    main.Show();

                    mainRef.Close();
                    desktop.MainWindow = main;
                });
                //NavigationContainer.ViewModel.ChangePage(AssistPage.SETTINGS);


            }
    }
}