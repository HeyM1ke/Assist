
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using Assist.Models.Enums;
using Assist.Services.Navigation;
using Assist.Shared.Services.Utils;
using Assist.Shared.Settings;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Assist.ViewModels;
using Assist.Views;
using Assist.Views.Startup;
using AsyncImageLoader;
using AsyncImageLoader.Loaders;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Assist;

public partial class App : Application
{
    
    
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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += OnExit;
            desktop.MainWindow = new MainWindow();
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
        CreateLogger();
        CreateDirectories();
        CheckForSettings();
        ImageLoader.AsyncImageLoader = new DiskCachedWebImageLoader();
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
    
    private static string GetApplicationDataFolder() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AssistData");
}