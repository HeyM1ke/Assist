using Assist.MVVM.View.LoginPage.Pages;
using Assist.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ValNet;
using ValNet.Objects.Authentication;

namespace Assist.MVVM.ViewModel
{
    internal class LoginPageViewModel : INotifyPropertyChanged
    {
        public Frame LoginWindowFrame { get; set; }


        public bool rememberLogin = true;
        internal string redirectUrl;
        internal CookieContainer container = new CookieContainer();

        // Riot Web Auth
        internal string ssid_Value;
        internal Cookie cookie_Ssid;

        internal RiotUser user;
        public async Task Login()
        {
            RiotUser user = new RiotUser();

            user.UserClient.CookieContainer.Add(cookie_Ssid);

            try
            {
                await user.Authentication.AuthenticateWithCookies();
            }
            catch (Exception ex)
            {
                AssistApplication.AppInstance.OpenAssistErrorWindow(ex);
                return;
            }

            // Implement a way to check if auth is successuful

            // Save Cookies
            AccountSettings userSettings = new AccountSettings()
            {
                Gamename = user.UserData.acct.game_name,
                Tagline = user.UserData.acct.tag_line,
                puuid = user.UserData.sub,
                Region = user.UserRegion,
            };

            userSettings.ConvertCookiesTo64(user.UserClient.CookieContainer);

            // Add login to Local Settings for future use, if Remember Me is chosen.
            if(!cookie_Ssid.Expires.ToString().Contains("1/1/0001") && UserSettings.Instance.FindAccountById(userSettings.puuid) is null)
                UserSettings.Instance.Accounts.Add(userSettings);

            if (UserSettings.Instance.DefaultAccount == null)
                UserSettings.Instance.DefaultAccount = userSettings.puuid;

            AssistApplication.AppInstance.currentUser = user;
            AssistApplication.AppInstance.currentAccount = userSettings;
            AssistApplication.AppInstance.OpenAssistMainWindow();

        }

        public async Task Login(RiotLoginData data)
        {
            user = new RiotUser();
            user.ChangeCredentials(data);
            try
            {
                var resp = await user.Authentication.AuthenticateWithCloud();

                if (resp.bIsAuthComplete == false)
                {
                    if (resp.type is not null && resp.type.Equals("multifactor"))
                    {
                        AccountLogin.current.loginFrame.Navigate(new Uri("/MVVM/View/LoginPage/Pages/LoginPage_Multifactor.xaml", UriKind.RelativeOrAbsolute));
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                AssistApplication.AppInstance.OpenAssistErrorWindow(ex);
                return;
            }

            // Implement a way to check if auth is successuful

            // Save Cookies
            AccountSettings userSettings = new AccountSettings()
            {
                Gamename = user.UserData.acct.game_name,
                Tagline = user.UserData.acct.tag_line,
                puuid = user.UserData.sub,
                Region = user.UserRegion,
            };

            userSettings.ConvertCookiesTo64(user.UserClient.CookieContainer);

            // Add login to Local Settings for future use, if Remember Me is chosen.
            if(UserSettings.Instance.FindAccountById(userSettings.puuid) is null)
                UserSettings.Instance.Accounts.Add(userSettings);

            if (UserSettings.Instance.DefaultAccount is null)
                UserSettings.Instance.DefaultAccount = userSettings.puuid;

            AssistApplication.AppInstance.currentUser = user;
            AssistApplication.AppInstance.currentAccount = userSettings;
            AssistApplication.AppInstance.OpenAssistMainWindow();

        }


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
