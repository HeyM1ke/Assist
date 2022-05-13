using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Assist.MVVM.View.InitPage;
using Assist.MVVM.View.Extra;
using Assist.MVVM.ViewModel;
using Assist.Settings;
using System.Text.Json;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Assist.Modules.XMPP;
using Assist.MVVM.Model;
using ValNet;
using ValNet.Objects.Authentication;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Assist
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        

        protected override void OnStartup(StartupEventArgs e)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            AssistLog.Normal("Program Started");
            //Startup Code here.
            base.OnStartup(e);
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assist"));
            AssistLog.Normal("Created assist dir" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assist"));
            try
            {
                AssistLog.Normal("Trying to read settings");
                AssistSettings.Current = JsonSerializer.Deserialize<AssistSettings>(File.ReadAllText(AssistSettings.SettingsFilePath));
                AssistLog.Normal("Read Settings");
            }
            catch
            {
                AssistLog.Error("Settings File was not found or tampered with.");
                AssistSettings.Current = new AssistSettings();
            }

            ChangeLanguage();
            
            AssistLog.Normal("Starting InitPage");

            if(new Ping().Send("www.google.com").Status != IPStatus.Success)
                AssistApplication.AppInstance.OpenAssistErrorWindow(new Exception("You are not connected to the Internet, Please Connect to the internet before using Assist."));


            AssistApplication.AppInstance.AssistApiController.CheckForAssistUpdates().GetAwaiter().GetResult();

            AssistLog.Normal("Starting Init Page");
            Current.MainWindow = new InitPage();

            Screen targetScreen = Screen.PrimaryScreen;

            Rectangle viewport = targetScreen.WorkingArea;
            Current.MainWindow.Top = (viewport.Height - Current.MainWindow.Height) / 2
                                     + viewport.Top;
            Current.MainWindow.Left = (viewport.Width - Current.MainWindow.Width) / 2
                                      + viewport.Left; ;
            Current.MainWindow.Show();
        }



        private void AppExit(object sender, ExitEventArgs e)
        {
            AssistSettings.Save();
            Environment.Exit(0);
        }
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            AssistLog.Error("Unhandled Ex Source: " + e.Exception.Source);
            AssistLog.Error("Unhandled Ex StackTrace: " + e.Exception.StackTrace);
            AssistLog.Error("Unhandled Ex Message: " + e.Exception.Message);
            MessageBox.Show(e.Exception.Message, "Assist Hit an Error : Logfile Created : If the error persists please reach out on the official discord server.", MessageBoxButton.OK, MessageBoxImage.Warning);

        }
        public static async Task<BitmapImage> LoadImageUrl(string url, BitmapCacheOption op = BitmapCacheOption.OnDemand)
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
        public static async Task<BitmapImage> LoadImageUrl(string url, int imageWidth, int imageHeight, BitmapCacheOption op = BitmapCacheOption.OnDemand)
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
            AssistLog.Normal("Changing Language");
            var curr = AssistSettings.Current.Language;

            switch (curr)
            {
                case Enums.ELanguage.en_us:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US", true);
                    AssistLog.Normal("Changed language to english");
                    break;
                case Enums.ELanguage.ja_jp:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("ja-JP", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP", true);
                    AssistLog.Normal("Changed language to japanese");
                    break;
                case Enums.ELanguage.es_es:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES", true);
                    AssistLog.Normal("Changed language to spanish");
                    break;
                case Enums.ELanguage.fr_fr:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR", true);
                    AssistLog.Normal("Changed language to french");
                    break;
                case Enums.ELanguage.pt_br:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR", true);
                    AssistLog.Normal("Changed language to portuguese");
                    break;
                case Enums.ELanguage.de_de:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE", true);
                    AssistLog.Normal("Changed language to german");
                    break;
                case Enums.ELanguage.tr_tr:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR", true);
                    AssistLog.Normal("Changed language to turkish");
                    break;
                case Enums.ELanguage.nl_nl:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("nl-NL", true);
                    AssistLog.Normal("Changed language to dutch");
                    break;
                case Enums.ELanguage.zh_cn:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-CN", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN", true);
                    AssistLog.Normal("Changed language to turkish");
                    break;

            }
        }

        public static void ShutdownAssist() => Application.Current.Shutdown();
    }
}
