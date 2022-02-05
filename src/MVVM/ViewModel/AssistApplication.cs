using ValNet;
using Assist.Settings;
using System.Windows;
using System.Threading.Tasks;
using System.IO;
using YamlDotNet;
using System;
using Assist.MVVM.Model;
using System.Net;
using Assist.MVVM.View.Extra;

namespace Assist.MVVM.ViewModel
{
    internal class AssistApplication
    {
        public static AssistApplication AppInstance { get; } = new();

        public AssistApiController AssistApiController { get; set; }
        //Page Models
        public AssistMainWindowViewModel MainWindowViewModel {  get; set; }
        public InitPageViewModel InitPageViewModel {  get; set; }
        public LoginPageViewModel LoginPageViewModel {  get; set; }
        public AssistFeaturedViewModel assistFeaturedViewModel { get; set; }
        public AssistStoreViewModel StorePageViewModel { get; set; }
        public AssistAccountSwitchViewModel AccountSwitchControlViewModel { get; set; }
        // Control Models

        public AssistRankGraphMicroViewModel RankMicroGraphViewModel { get; set; }
        public AssistStoreBundleViewModel StoreBundleViewModel { get; set; }
        public AssistStoreItemViewModel StoreItemViewModel { get; set;}
        public ProgressionBattlepassViewmodel BattlepassViewModel { get; set; }
        public AssistLaunchControlViewModel LaunchControlViewModel { get; set; }
        // Backbone
        public RiotUser currentUser { get; set; }
        public AccountSettings currentAccount { get; set; }
        public AssistLog Log { get; }

        public AssistApplication()
        {
            // Application Models
            AssistApiController = new();
            InitPageViewModel = new();
            Log = new AssistLog();
            
            // General Viewmodels
            AccountSwitchControlViewModel = new();

            //Page Viewmodels
            StorePageViewModel = new();

            // Store Models
            StoreItemViewModel = new();
            StoreBundleViewModel = new();
            LoginPageViewModel = new();
            LaunchControlViewModel = new();

            //Control Models
            RankMicroGraphViewModel = new();
        }


        public void OpenAssistMainWindow()
        {
            var temp = Application.Current.MainWindow;
            temp.Visibility = Visibility.Hidden;
            Application.Current.MainWindow = new MainWindow();
            Application.Current.MainWindow.Show();
            temp.Close();
        }
        public void OpenAssistMainWindowToSettings()
        {
            var temp = Application.Current.MainWindow;
            temp.Visibility = Visibility.Hidden;
            Application.Current.MainWindow = new MainWindow();
            Application.Current.MainWindow.Show();
            MainWindow.MainWindowInstance.mainContentFrame.Navigate(new Uri("/MVVM/View/SettingsPage/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
            temp.Close();
        }
        public void OpenAccountLoginWindow()
        {
            Application.Current.MainWindow.Visibility = Visibility.Hidden;
            AccountLogin accLogin = new AccountLogin(true);
            Application.Current.MainWindow.Close();
            accLogin.Show();
            Application.Current.MainWindow = accLogin;
        }
        public void OpenAssistErrorWindow(Exception ex, string extraParam = null)
        {
            this.Log.Error($"Opened Error Window Method Called ; MESSAGE: {ex.Message} | CUSTOM MESSAGE: {extraParam} ");
            var errorS = new ErrorScreen(ex, extraParam);
            this.Log.Error("Opened Error Window");
            errorS.ShowDialog();
            this.Log.Error("Closed Error Window");
        }
        public async Task CreateAuthenticationFile()
        {
            string pSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data", "RiotGamesPrivateSettings.yaml");

            // Create File
            var settings = new ClientPrivateModel(currentUser);

            // Create RiotClientPrivateSettings.yaml
            using (TextWriter writer = File.CreateText(pSettingsPath))
            {
                settings.CreateFile().Save(writer, false);
            }

        }
        public async Task AuthenticateWithAccountSetting(AccountSettings account)
        {
            RiotUser user = new RiotUser();

            foreach (Cookie cookie in account.Convert64ToCookies().GetAllCookies())
            {
                user.UserClient.CookieContainer.Add(cookie);
            }

            var gamename = account.Gamename;
            var tagLine = account.Tagline;
            try
            {
                this.Log.Normal($"Authentcating with Cookies for User {account.puuid} / {gamename}#{tagLine}");
                await user.Authentication.AuthenticateWithCookies();
            }
            catch (Exception ex)
            {
                this.Log.Error($"ACCOUNT NO LONGER VALID - {gamename}#{tagLine}");

                string errorMess = $"Login to account: {gamename}#{tagLine}, has expired. Please re-add the account.";
                AssistApplication.AppInstance.OpenAssistErrorWindow(ex, errorMess);
                
                
                this.Log.Normal("Removing Account");
                UserSettings.Instance.Accounts.Remove(account);
                UserSettings.Instance.Save();
            }


            account.ConvertCookiesTo64(user.UserClient.CookieContainer); // Resaves cookies after each login to prevent invalid cookies.

            if (!string.IsNullOrEmpty(user.tokenData.entitle) && !string.IsNullOrEmpty(user.tokenData.access))
            {
                AppInstance.currentAccount = account;
                AppInstance.currentUser = user;
            }

            OpenAssistMainWindow();
        }

    }
}
