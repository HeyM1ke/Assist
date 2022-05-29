using ValNet;
using Assist.Settings;
using System.Windows;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Diagnostics;
using System.Globalization;
using Assist.MVVM.Model;
using System.Net;
using System.Threading;
using Assist.MVVM.View.Authentication;
using Assist.MVVM.View.Extra;
using Assist.Services;
using RestSharp;

namespace Assist.MVVM.ViewModel
{
    internal class AssistApplication
    {
        public static AssistApplication AppInstance { get; } = new();
        public TokenServiceBackgroundService TokenService { get; set; }
        public AssistApiController AssistApiController { get; set; }
        // Control Models

        // Backbone
        public RiotUser CurrentUser { get; set; }
        public ProfileSetting CurrentProfile { get; set; }

        public static double GlobalScaleRate { get; set; } = 1.00;

        public AssistApplication()
        {
            AssistApiController = new();

        }


        public void OpenAssistMainWindow()
        {
            var temp = Application.Current.MainWindow;
            temp.Visibility = Visibility.Hidden;
            Application.Current.MainWindow = new AssistMainWindow();
            Application.Current.MainWindow.Show();
            temp.Close();
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
            AssistLog.Error($"Opened Error Window Method Called ; MESSAGE: {ex.Message} | CUSTOM MESSAGE: {extraParam} ");
            var errorS = new ErrorScreen(ex, extraParam);
            AssistLog.Error("Opened Error Window");
            errorS.ShowDialog();
            AssistLog.Error("Closed Error Window");
        }
        public async Task CreateAuthenticationFile()
        {
            string pSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data", "RiotGamesPrivateSettings.yaml");
            string pSettingsPathBackup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data", "RiotClientPrivateSettings.yaml");
            string pClientSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Config", "RiotClientSettings.yaml");

            var fileInfo = FileVersionInfo.GetVersionInfo(AssistSettings.Current.RiotClientInstallPath);

            AssistLog.Normal("Version of Client: " + fileInfo.FileVersion);

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


            await RedoCookies(CurrentUser);

        }
        public async Task AuthenticateWithProfileSetting(ProfileSetting profile)
        {
            RiotUser user = new RiotUser();

            foreach (Cookie cookie in profile.Convert64ToCookies().GetAllCookies())
            {
                user.UserClient.CookieContainer.Add(cookie);
            }

            var gamename = profile.Gamename;
            var tagLine = profile.Tagline;
            try
            {
                AssistLog.Normal($"Authentcating with Cookies for User {profile.ProfileUuid} / {gamename}#{tagLine}");
                await user.Authentication.AuthenticateWithCookies();
            }
            catch (Exception ex)
            {
                AssistLog.Error($"ACCOUNT NO LONGER VALID - {gamename}#{tagLine}");

                string errorMess = $"Login to account: {gamename}#{tagLine}, has expired. Please re-add the account.";
                AssistApplication.AppInstance.OpenAssistErrorWindow(ex, errorMess);


                AssistLog.Normal("Removing Account");
                AssistSettings.Current.Profiles.Remove(profile);
                AssistSettings.Save();
                
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
            var tempUser = new RiotUser();
            foreach (Cookie cook in user.UserClient.CookieContainer.GetAllCookies())
            {
                tempUser.UserClient.CookieContainer.Add(new Cookie(cook.Name, cook.Value, "/", cook.Domain));
            }

            await tempUser.Authentication.AuthenticateWithCookies();

            AssistApplication.AppInstance.CurrentUser = tempUser;
            AssistApplication.AppInstance.CurrentProfile.ConvertCookiesTo64(tempUser.UserClient.CookieContainer);
        }

        public void ChangeLanguage()
        {
            var curr = AssistSettings.Current.Language;

            switch (curr)
            {
                case Enums.ELanguage.en_us:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en_us", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en_us", true);
                    break;
                case Enums.ELanguage.ja_jp:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("ja-JP", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP", true);
                    break;
            }
        }
    }
}
