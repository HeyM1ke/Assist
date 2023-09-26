using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Assist.Views.Windows;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;

using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using Assist.Objects.Enums;
using Assist.Services.Utils;
using Assist.Settings;
using Avalonia;
using Avalonia.Platform;
using Assist.ViewModels.Windows;
using Avalonia.Controls;
using Avalonia.Threading;
using Squirrel;
using NuGet.Versioning;

namespace Assist
{
    
    
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            SetupLogger();
            SetupUpdator();
            ReadSettings();
            Log.Information("Starting application");
            Log.Information($"Getting Platform... WINDOWS: {OperatingSystem.IsWindows()} |  MACOS: {OperatingSystem.IsMacOS()} | LINUX: {OperatingSystem.IsLinux()} ");
        }

        public override void OnFrameworkInitializationCompleted()
        {
            Log.Debug("Initialization Complete/");
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Initial Window Opened at launch.
                desktop.Exit += OnExit;
                
                desktop.MainWindow = new StartupSplash();
            }
            
            base.OnFrameworkInitializationCompleted();
        }

        private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            Log.Information("Exiting");
            AssistSettings.Save();
            GameSettings.Save();
        }

        private static void SetupLogger()
        {

#if DEBUG 

            //if(OperatingSystem.IsWindows()) AllocConsole();
            
#endif

            var directory = GetApplicationDataFolder();
            var logsDirectory = Path.Combine(directory, "logs");
            Directory.CreateDirectory(logsDirectory);

            var fileCount = Directory.GetFiles(logsDirectory, "*", SearchOption.TopDirectoryOnly).Length;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
#if DEBUG
                .WriteTo.Console(
                    theme: AnsiConsoleTheme.Code,
                    outputTemplate: "[{Timestamp:G}] [{Level:u3}] {Message:l}{NewLine}")
#endif
            .WriteTo.File(
                    path: Path.Combine(logsDirectory, $"Log_{++fileCount}.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate:
                    "[{Timestamp:G}] [{Level:u3}] {Message:l}{NewLine:1}{Properties:1j}{NewLine:1}{Exception:1}")
                .CreateLogger();

            Log.Information("ASSIST LOG LIVE: " + AssistSettings.Current.ApplicationVersion);
        }

        private static string GetApplicationDataFolder()
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var directory = Path.Combine(appdata, "AssistX");

            return directory;
        }

        public static void ChangeLanguage()
        {
            Log.Information("Changing Language");
            var language = AssistSettings.Current.Language;
            var attribute = language.GetAttribute<LanguageAttribute>();
            Log.Information(attribute.Code);
            var culture = new CultureInfo(attribute.Code, true);

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            });
        }

        public static void ReadSettings()
        {
            try
            {
                var settingsContent = File.ReadAllText(AssistSettings.SettingsFilePath);
                AssistSettings.Current = JsonSerializer.Deserialize<AssistSettings>(settingsContent);
            }
            catch (Exception e)
            {
                Log.Fatal("Failed to Read Settings, Acting like Fresh Install.");
            }
        }

        public static void SetupUpdator()
        {
            Log.Information("Starting Updator");
            SquirrelAwareApp.HandleEvents(onInitialInstall: OnAppInstall, onAppUninstall: OnAppUninstall);
        }

        private static void OnAppInstall(SemanticVersion version, IAppTools tools)
        {
            tools.CreateShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }

        private static void OnAppUninstall(SemanticVersion version, IAppTools tools)
        {
            tools.RemoveShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }
    }
}
