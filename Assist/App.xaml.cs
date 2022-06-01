using Assist.Attributes;
using Assist.MVVM.View.Extra;
using Assist.MVVM.View.InitPage;
using Assist.MVVM.ViewModel;
using Assist.Settings;
using Assist.Update;
using Assist.Utils;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Assist
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetupLogger();

            if (!HasInternet())
            {
                AssistApplication.AppInstance.OpenAssistErrorWindow(new Exception("You are not connected to the Internet, Please Connect to the internet before using Assist."));
                return;
            }

            var shouldUpdate = await CheckForUpdatesAsync();
            if (shouldUpdate)
                return;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            Log.Information("Starting application");
            Log.Information("Reading the settings file");
            try
            {
                var settingsContent = await File.ReadAllTextAsync(AssistSettings.SettingsFilePath);
                AssistSettings.Current = JsonSerializer.Deserialize<AssistSettings>(settingsContent);
                
                Log.Information("Successfully read the settings file");
            }
            catch
            {
                Log.Error("Settings File was not found or tampered with.");
                AssistSettings.Current = new AssistSettings();
            }

            ChangeLanguage();
            Log.Information("Starting InitPage");

            Current.MainWindow = new InitPage();

            var targetScreen = Screen.PrimaryScreen;
            var viewport = targetScreen.WorkingArea;

            Current.MainWindow.Top = (viewport.Height - Current.MainWindow.Height) / 2 + viewport.Top;
            Current.MainWindow.Left = (viewport.Width - Current.MainWindow.Width) / 2 + viewport.Left;
            Current.MainWindow.Show();
        }

        private void AppExit(object sender, ExitEventArgs e)
        {
            AssistSettings.Save();
            Environment.Exit(0);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;

            Log.Fatal(exception, "Unhandled exception.");
            MessageBox.Show(e.Exception.Message, "Assist hit a fatal exception. If the error persists please reach out on the official discord server.", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static async Task<bool> CheckForUpdatesAsync()
        {
            var timeout = TimeSpan.FromSeconds(10);
            var updater = new ApplicationUpdateChecker(timeout);
            var result = await updater.ShouldUpdateAsync();

            var shouldUpdate = !result.IsUpdated && result is { EventArgs: { } };
            if (shouldUpdate)
                OpenUpdateWindow(result);

            return shouldUpdate;
        }

        private static void OpenUpdateWindow(UpdateCheckResult result)
        {
            var updateWindow = new UpdateWindow(result.EventArgs);
            updateWindow.Show();
        }

        private static void SetupLogger()
        {
#if DEBUG
            Win32.AllocConsole();
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
        }

        private static string GetApplicationDataFolder()
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var directory = Path.Combine(appdata, "Assist");

            return directory;
        }

        public static BitmapImage LoadImageUrl(string url, BitmapCacheOption op = BitmapCacheOption.OnDemand)
        {
            // Allows the image to be loaded with the resolution it is intended to be used for.
            // Because the program is a solo resolution that doesnt change res, this is fine.

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = op;
            image.UriSource = new Uri(url, UriKind.Absolute);
            image.EndInit();
            
            return image;
        }

        public static BitmapImage LoadImageUrl(string url, int imageWidth, int imageHeight, BitmapCacheOption op = BitmapCacheOption.OnDemand)
        {
            // Allows the image to be loaded with the resolution it is intended to be used for.
            // Because the program is a solo resolution that doesnt change res, this is fine.

            var image = new BitmapImage();
            image.BeginInit();
            image.DecodePixelHeight = (int)(imageHeight * AssistApplication.GlobalScaleRate);
            image.DecodePixelWidth = (int)(imageWidth * AssistApplication.GlobalScaleRate);
            image.CacheOption = op;
            image.UriSource = new Uri(url, UriKind.Absolute);
            image.EndInit();
            
            return image;
        }

        public static void ChangeLanguage()
        {
            Log.Information("Changing Language");
            var language = AssistSettings.Current.Language;
            var attribute = language.GetAttribute<LanguageAttribute>();

            var culture = new CultureInfo(attribute.Code, true);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        public static void ShutdownAssist() 
            => Current.Shutdown();

        public bool HasInternet()
        {
            // todo
            return true;
        }

    }
}
