
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.Json;
using System.Threading;
using Assist.Models.Enums;
using Assist.Services.Navigation;
using Assist.Shared.Services.Utils;
using Assist.Shared.Settings;
using Assist.Shared.Settings.Accounts;
using Assist.Shared.Settings.Modules;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Assist.ViewModels;
using Assist.Views;
using Assist.Views.Extras;
using Assist.Views.Startup;
using AsyncImageLoader;
using AsyncImageLoader.Loaders;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Assist;

public partial class App : Application
{
#if DEBUG
    public const string APPPROTOCOL = "assistdebug";
#else
    public const string APPPROTOCOL = "assist";
#endif
    public bool IsElevated => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    public override void Initialize()
    {
        OnStartup();   
        AvaloniaXamlLoader.Load(this);
    }

    public override void RegisterServices()
    {
        base.RegisterServices();
    }

    public override void OnFrameworkInitializationCompleted()
    {

        if (OperatingSystem.IsWindows())
        {
            HandleWindowsDesktopInitialization();
        }
        
        base.OnFrameworkInitializationCompleted();
    }
    
    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        Log.Information("Exiting");
        AssistSettings.Save();
    }

    private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        Log.Information("Shutting Down");
        AssistSettings.Save();
    }
    private void OnStartup()
    {
        CreateDirectories();
        CreateLogger();
        CheckForSettings();
        ImageLoader.AsyncImageLoader = new DiskCachedWebImageLoader(AssistSettings.CacheFolderPath);
        HandleProtocol();
    }
    
    private void CreateDirectories()
    {
        Directory.CreateDirectory(GetApplicationDataFolder());
        Directory.CreateDirectory(Path.Combine(GetApplicationDataFolder(), "Logs"));
        Directory.CreateDirectory(Path.Combine(GetApplicationDataFolder(), "Deps"));
        Directory.CreateDirectory(Path.Combine(GetApplicationDataFolder(), "Accounts"));
    }

    private void CreateLogger()
    {
        var fileCount = Directory.GetFiles(Path.Combine(GetApplicationDataFolder(), "Logs"), "*", SearchOption.TopDirectoryOnly).Length;
#if DEBUG
        WindowsUtils.AllocConsole();
        Log.Logger = new LoggerConfiguration().WriteTo.Console(theme: AnsiConsoleTheme.Literate).CreateLogger();
#else
        Log.Logger = new LoggerConfiguration().WriteTo.Console(theme: AnsiConsoleTheme.Literate).WriteTo.File(
            Path.Combine(GetApplicationDataFolder(), "Logs", $"Assist-{DateTime.Now:yyyy-MM-dd}-{fileCount}.txt"),
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [Assist] {Message:lj}{NewLine}{Exception}").CreateLogger();
#endif
        
        // ty fmodel boys <3 - Mike
        Log.Information("Version {Version}", Process.GetCurrentProcess().MainModule.FileVersionInfo.FileVersion);
        Log.Information("{OS}", GetOperatingSystemProductName());
        Log.Information("System Culture {SysLang}", CultureInfo.CurrentCulture);
    }

    
    
    private static void CheckForSettings()
    {
        try
        {
            AssistSettings.Default =
                JsonSerializer.Deserialize<AssistSettings>(File.ReadAllText(AssistSettings.FilePath));
        }
        catch
        {
            AssistSettings.Default = new AssistSettings();
            
            // If Settings are new Attempt to set the culture to the system.
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentCulture;
            });
            
        }
        
        AssistSettings.Save();
        
        try
        {
            ModuleSettings.Default =
                JsonSerializer.Deserialize<ModuleSettings>(File.ReadAllText(ModuleSettings.FilePath));
        }
        catch
        {
            ModuleSettings.Default = new ModuleSettings();
        }
        
        ModuleSettings.Save();
        
        try
        {
            if (File.Exists(AccountSettings.FilePath))
            {
                Log.Information("Found Account Settings File!");
            }
            
            
            AccountSettings.Default =
                JsonSerializer.Deserialize<AccountSettings>(File.ReadAllText(AccountSettings.FilePath));
        }
        catch(Exception e)
        {
            Log.Error("Failed to read account settings");
            Log.Error(e.Message);
            AccountSettings.Default = new AccountSettings();
        }
        
        AccountSettings.Save();
        
    }
    
    
    private string GetOperatingSystemProductName()
    {
        // Shoutout to Fmodel for this <3
        // Added some other things cause other Platforms

        if (!OperatingSystem.IsWindows())
            return "Not Fucking Windows";
        
        var productName = string.Empty;
        try
        {
            productName = WindowsUtils.BrandingFormatString("%WINDOWS_LONG%");
        }
        catch
        {
            // ignored
        }

        if (string.IsNullOrEmpty(productName))
            productName = Environment.OSVersion.VersionString;

        return $"{productName} ({(Environment.Is64BitOperatingSystem ? "64" : "32")}-bit)";
    }
    
    public static void ChangeLanguage()
    {
        Log.Information("Changing Language");
        var language = AssistSettings.Default.Language;
        var attribute = language.GetAttribute<LanguageAttribute>();
        Log.Information(attribute.Code);
        var culture = new CultureInfo(attribute.Code, true);

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        });
    }
    
    private void HandleProtocol()
    {
        Log.Information("Handling Protocol");
        
        // First Check for Permissions of the app.
        RegistryKey key = Registry.ClassesRoot.OpenSubKey(APPPROTOCOL);
        if (key == null)
        {
            Log.Information("Key does not exist.");
            if (IsElevated)
                CreateAssistAppProtocol();
            else
                Log.Information("No Admin Access");
        }
    }

    private void HandleWindowsDesktopInitialization()
    {
        // args[0] is always going to be the application path. When using the protocol it will show it.
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {


            if (desktop.Args.Length <= 0)
            {
                desktop.MainWindow = new MainWindow();
                return;
            }
            
            switch (desktop.Args[0].ToLower())
            {
                case { } uri when uri.Contains($"{APPPROTOCOL}://launch/"):
                    desktop.MainWindow = new AssistLaunchWindow();
                    break;
                case { } uri when uri.Contains($"{APPPROTOCOL}://join/"):
                    desktop.MainWindow = new AssistJoinWindow();
                    break;
                default:
                    desktop.MainWindow = new MainWindow();
                    break;
            }
        }
    }
    private void CreateAssistAppProtocol()
    {
        Log.Information("Creating Assist Protocol");
        RegistryKey key = Registry.ClassesRoot.OpenSubKey(APPPROTOCOL);
        string applicationPath = Process.GetCurrentProcess().MainModule.FileName;
        if (key == null)
        {
            
            key = Registry.ClassesRoot.CreateSubKey(APPPROTOCOL);
            key.SetValue(string.Empty, "URL: " + APPPROTOCOL);
            key.SetValue("URL Protocol", string.Empty);

            key = key.CreateSubKey(@"shell\open\command");
            key.SetValue(string.Empty, applicationPath + " " + "%1");
            key.Close();
        }
        
        Log.Information("Created Assist Protocol");
    }
    
    private static string GetApplicationDataFolder() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AssistData");
}