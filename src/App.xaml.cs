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
using System.Net;

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
            AssistApplication.AppInstance.Log.Normal("Program Started");
            //Startup Code here.
            base.OnStartup(e);
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assist"));


            if (File.Exists(UserSettings.SettingsFilePath))
                try
                {
                    UserSettings.Instance = JsonSerializer.Deserialize<UserSettings>(File.ReadAllText(UserSettings.SettingsFilePath));
                }
                catch (Exception ex)
                {
                    AssistApplication.AppInstance.Log.Error("Settings File was tampered with" + ex.Message);
                    new UserSettings();
                }
            else
            {
                new UserSettings();
            }


            AssistApplication.AppInstance.Log.Normal("Starting InitPage");
            AssistApplication.AppInstance.AssistApiController.CheckForAssistUpdates();
            Current.MainWindow = new InitPage();
            Current.MainWindow.Show();
        }



        private void AppExit(object sender, ExitEventArgs e)
        {
            UserSettings.Instance.Save();
            Environment.Exit(0);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            AssistApplication.AppInstance.Log.Error("Unhandled Ex Source: " + e.Exception.Source);
            AssistApplication.AppInstance.Log.Error("Unhandled Ex StackTrace: " + e.Exception.StackTrace);
            AssistApplication.AppInstance.Log.Error("Unhandled Ex Message: " + e.Exception.Message);
            MessageBox.Show(e.Exception.Message, "Assist Hit an Error", MessageBoxButton.OK, MessageBoxImage.Warning);

        }
    }
}
