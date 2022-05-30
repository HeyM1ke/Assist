using Assist.MVVM.Model;
using Assist.MVVM.View.InitPage;
using Assist.MVVM.ViewModel;
using Assist.Settings;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
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

        [DllImport("kernel32")]
        private static extern bool AllocConsole();

        protected override async void OnStartup(StartupEventArgs e)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            SetupLogger();

            Log.Information("Starting application");
            base.OnStartup(e);

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

            if(!InternetCheck())
                AssistApplication.AppInstance.OpenAssistErrorWindow(new Exception("You are not connected to the Internet, Please Connect to the internet before using Assist."));

            await AssistApplication.AppInstance.AssistApiController.CheckForAssistUpdates();
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

        private static void SetupLogger()
        {
#if DEBUG
            AllocConsole();
#endif

            var directory = GetApplicationDataFolder();
            var logsDirectory = Path.Combine(directory, "logs");
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
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
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

            switch (language)
            {
                case Enums.ELanguage.en_us:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US", true);
                    Log.Information("Changed language to english");
                    break;
                case Enums.ELanguage.ja_jp:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("ja-JP", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP", true);
                    Log.Information("Changed language to japanese");
                    break;
                case Enums.ELanguage.es_es:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES", true);
                    Log.Information("Changed language to spanish");
                    break;
                case Enums.ELanguage.fr_fr:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR", true);
                    Log.Information("Changed language to french");
                    break;
                case Enums.ELanguage.pt_br:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR", true);
                    Log.Information("Changed language to portuguese");
                    break;
                case Enums.ELanguage.de_de:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE", true);
                    Log.Information("Changed language to german");
                    break;
                case Enums.ELanguage.tr_tr:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR", true);
                    Log.Information("Changed language to turkish");
                    break;
                case Enums.ELanguage.nl_nl:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("nl-NL", true);
                    Log.Information("Changed language to dutch");
                    break;
                case Enums.ELanguage.ru_ru:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU", true);
                    Log.Information("Changed language to Russian");
                    break;
                case Enums.ELanguage.zh_cn:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-CN", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN", true);
                    Log.Information("Changed language to chinese");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void ShutdownAssist() 
            => Current.Shutdown();

        public bool InternetCheck()
        {
            // todo
            return true;
        }

    }
}
