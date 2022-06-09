using Assist.MVVM.Model;
using Assist.MVVM.View.Authentication;
using Assist.MVVM.View.Extra;
using Assist.Services;
using Assist.Settings;

using Serilog;

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Assist.MVVM.View.Selector;
using ValNet;

namespace Assist.MVVM.ViewModel
{
    // todo: cleanup
    internal class AssistApplication
    {

        public static AssistApiService ApiService { get; } = new();
        public static AssistApplication AppInstance { get; } = new();

        public static string AgentFormat { get; set; } =
            "RiotClient/51.0.0.4429735.4381201 {0} (Windows;10;;Professional, x64)";
        public TokenServiceBackgroundService TokenService { get; set; }
        // Control Models

        // Backbone
        public RiotUser CurrentUser { get; set; }
        public ProfileSetting CurrentProfile { get; set; }

        public static double GlobalScaleRate { get; set; } = 1.00;
        public static int SelectorSeconds
        {
            get => 10;
        }
        public void OpenAssistMainWindow()
        {
            var window = Application.Current.MainWindow!;
            window.Visibility = Visibility.Hidden;

            var mainWindow = new AssistMainWindow();
            mainWindow.Show();

            Application.Current.MainWindow = mainWindow;
            window.Close();
        }

        public void OpenAssistMainWindowToSettings()
        {
            var temp = Application.Current.MainWindow;
            temp.Visibility = Visibility.Hidden;

            Application.Current.MainWindow = new AssistMainWindow();
            Application.Current.MainWindow.Show();
            AssistMainWindow.Current.SettingsBTN_Click(null, null);

            temp.Close();
        }

        public void OpenAccountLoginWindow(bool bAddProfile)
        {
            Application.Current.MainWindow.Visibility = Visibility.Hidden;
            Authentication accLogin = new Authentication(bAddProfile);
            Application.Current.MainWindow.Close();
            accLogin.Show();
            Application.Current.MainWindow = accLogin;
        }

        public void OpenAssistErrorWindow(Exception ex, string extraParam = null)
        {
            Log.Error($"Opened Error Window Method Called ; MESSAGE: {ex.Message} | CUSTOM MESSAGE: {extraParam} ");
            var errorS = new ErrorScreen(ex, extraParam);
            Log.Error("Opened Error Window");
            errorS.ShowDialog();
            Log.Error("Closed Error Window");
        }

        public void OpenStartupWindow()
        {
            Application.Current.MainWindow.Visibility = Visibility.Hidden;
            Startup startupPage = new Startup();
            Application.Current.MainWindow.Close();
            startupPage.Show();
            Application.Current.MainWindow = startupPage;
        }

        public async Task CreateAuthenticationFile()
        {
            string pSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data", "RiotGamesPrivateSettings.yaml");
            string pSettingsPathBackup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data", "RiotClientPrivateSettings.yaml");
            string pClientSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Config", "RiotClientSettings.yaml");

            var fileInfo = FileVersionInfo.GetVersionInfo(AssistSettings.Current.RiotClientInstallPath);

            Log.Information("Version of Client: " + fileInfo.FileVersion);

            // Create File
            var settings = new ClientGameModel(CurrentUser);
            var settings2 = new ClientPrivateModel(CurrentUser);
            var cSettings = new ClientSettingsModel();

            if (fileInfo.FileMajorPart >= 46)
            {
                // Create File
                using (TextWriter writer = File.CreateText(pClientSettingsPath))
                {
                    cSettings.CreateSettings().Save(writer, false);
                }
                // Create RiotClientPrivateSettings.yaml
                using (TextWriter writer = File.CreateText(pSettingsPath))
                {
                    settings.CreateFileWRegion().Save(writer, false);
                }

                using (TextWriter writer = File.CreateText(pSettingsPathBackup))
                {
                    settings2.CreateFile().Save(writer, false);
                }
            }
            else
            {
                // Create File
                // Create RiotClientPrivateSettings.yaml
                using (TextWriter writer = File.CreateText(pSettingsPath))
                {
                    settings.CreateFile().Save(writer, false);
                }


                using (TextWriter writer = File.CreateText(pSettingsPathBackup))
                {
                    settings2.CreateFile().Save(writer, false);
                }
            }


            //await RedoCookies(CurrentUser);

        }
        public async Task AuthenticateWithProfileSetting(ProfileSetting profile)
        {
            RiotUser user = new RiotUser(AssistApplication.AgentFormat);

            foreach (Cookie cookie in profile.Convert64ToCookies().GetAllCookies())
            {
                user.UserClient.CookieContainer.Add(cookie);
            }

            var gamename = profile.Gamename;
            var tagLine = profile.Tagline;
            try
            {
                Log.Information($"Authentcating with Cookies for User {profile.ProfileUuid} / {gamename}#{tagLine}");
                await user.Authentication.AuthenticateWithCookiesCurl();
            }
            catch (Exception ex)
            {
                Log.Error($"ACCOUNT NO LONGER VALID - {gamename}#{tagLine}");

                string errorMess = $"Login to account: {gamename}#{tagLine}, has expired. Please re-add the account.";
                AssistApplication.AppInstance.OpenAssistErrorWindow(ex, errorMess);


                Log.Information("Removing Account");
                AssistSettings.Current.Profiles.Remove(profile);
                AssistSettings.Save();
                throw new Exception("Acc Invalid");
            }


            profile.ConvertCookiesTo64(user.UserClient.CookieContainer); // Resaves cookies after each login to prevent invalid cookies.

            if (!string.IsNullOrEmpty(user.tokenData.entitle) && !string.IsNullOrEmpty(user.tokenData.access))
            {
                
                AppInstance.CurrentProfile = profile;
                AppInstance.CurrentUser = user;
                await profile.SetupProfile(user);
            }

            OpenAssistMainWindow();
        }

        private async Task RedoCookies(RiotUser user)
        {
            var tempUser = new RiotUser(AssistApplication.AgentFormat);
            foreach (Cookie cook in user.UserClient.CookieContainer.GetAllCookies())
            {
                tempUser.UserClient.CookieContainer.Add(new Cookie(cook.Name, cook.Value, "/", cook.Domain));
            }

            try
            {
                Log.Information("Creating Second Account for Riot Client");
                await tempUser.Authentication.AuthenticateWithCookiesCurl();
            }
            catch (Exception e)
            {
                Log.Information("Failed to create Second Account for Riot Client, Trying Again.");
                await RedoCookies(user);
                return;
            }
            

            AppInstance.CurrentUser = tempUser;
            AppInstance.CurrentProfile.ConvertCookiesTo64(tempUser.UserClient.CookieContainer);
        }

    }
}
