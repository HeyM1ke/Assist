using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Assist.Settings;
using System.IO;
using System.Windows;
using System.Windows.Data;
using ValNet;
using System.Net;
using System.Diagnostics;

namespace Assist.MVVM.ViewModel
{
    class InitPageViewModel : INotifyPropertyChanged
    {
        private string currentStatusMessage;

        public event PropertyChangedEventHandler? PropertyChanged;

        public InitPageViewModel()
        {
            currentStatusMessage = "Welcome!";
        }

        public async Task FirstTimeSetup()
        {
            if (AssistApplication.AppInstance.AssistApiController.bIsUpdate)
                return;

            AssistApplication.AppInstance.Log.Normal("Running First Time Setup");
            var clientPath = await UserSettings.Instance.FindRiotClientPath();
            if (clientPath is null)
            {
                // Do Something if the client path is invalid and the client does not excist.
                AssistApplication.AppInstance.Log.Error("NO RIOT CLIENT FOUND ON COMPUTER / IN SETTINGS FILE");
                Environment.Exit(2);
            }

            AssistApplication.AppInstance.Log.Normal("Setting Settings RiotClient Path to Settings, Path Found: " + clientPath);
            UserSettings.Instance.RiotClientInstallPath = clientPath; // Set the Client path to settings.

            // Next Step, Add account.
            OpenAccountLogin();

        }

        public async Task DefaultStartup()
        {
            if (AssistApplication.AppInstance.AssistApiController.bIsUpdate)
                return;

            AssistApplication.AppInstance.Log.Normal("Calling Default Startup");
            //Check if RiotClient is Valid
            var clientPath = await UserSettings.Instance.FindRiotClientPath();
            if (clientPath is not null)
                UserSettings.Instance.RiotClientInstallPath = clientPath; // Set the Client path to settings.
            
            //Check if There are accounts
            if(UserSettings.Instance.Accounts.Count == 0)
            {
                AssistApplication.AppInstance.Log.Normal("No Accounts Found");
                OpenAccountLogin();
            }
            else
            {
                if (!string.IsNullOrEmpty(UserSettings.Instance.DefaultAccount))
                {
                    var account = UserSettings.Instance.FindAccountById(UserSettings.Instance.DefaultAccount);

                    if (account != null)
                    {
                        RiotUser user = new RiotUser();

                        foreach (Cookie cookie in account.Convert64ToCookies().GetAllCookies())
                        {
                            user.UserClient.CookieContainer.Add(cookie);
                        }

                        var gamename = account.Gamename;
                        var tagLine = account.Tagline;

                        AssistApplication.AppInstance.Log.Normal($"Authentcating with Cookies for User {account.puuid} / {gamename}#{tagLine}");
                        try
                        {
                            await user.Authentication.AuthenticateWithCookies();
                        }
                        catch (Exception ex)
                        {
                            AssistApplication.AppInstance.Log.Error($"ACCOUNT NO LONGER VALID - {gamename}#{tagLine}");
                            string errorMess = $"Login for account: {gamename}#{tagLine} is expired. Please re-add the account.";
                            AssistApplication.AppInstance.OpenAssistErrorWindow(ex, errorMess);
                            AssistApplication.AppInstance.Log.Normal("Removing Account");
                            UserSettings.Instance.Accounts.Remove(account);

                            // Creates fallback to default account if the default account is invalid
                            if (UserSettings.Instance.Accounts.Count != 0)
                                UserSettings.Instance.DefaultAccount = UserSettings.Instance.Accounts[0].puuid;
                            else
                            {
                                UserSettings.Instance.DefaultAccount = null;
                            }
                            UserSettings.Instance.Save();

                            await DefaultLogin(); // Run Default Login
                        }

                        if (user.AuthType == AuthType.Cookie)
                        {
                            account.ConvertCookiesTo64(user.UserClient.CookieContainer); // Resaves cookies after each login to prevent invalid cookies.

                            if (!string.IsNullOrEmpty(user.tokenData.entitle) && !string.IsNullOrEmpty(user.tokenData.access))
                            {
                                AssistApplication.AppInstance.currentAccount = account;
                                AssistApplication.AppInstance.currentUser = user;
                            }
                        }
                    }
                    else
                        await DefaultLogin();


                }
                else
                {
                    await DefaultLogin();
                }


                if (AssistApplication.AppInstance.currentAccount == null || AssistApplication.AppInstance.currentUser == null)
                {
                    AssistApplication.AppInstance.Log.Normal("No Accounts were successfully logged into, opening account login.");
                    UserSettings.Instance.Accounts.Clear();
                    UserSettings.Instance.Save();
                    OpenAccountLogin();
                }
                else
                {

                    AssistApplication.AppInstance.Log.Normal("Login was successful, opening main window.");
                    AssistApplication.AppInstance.OpenAssistMainWindow();
                }

                // When Account is not valid

            }

            
        }
        
        private void OpenAccountLogin()
        {
            AssistApplication.AppInstance.Log.Normal("Opening Account Login");
            AccountLogin accLogin = new AccountLogin(false);
            Application.Current.MainWindow.Close();
            Application.Current.MainWindow = accLogin;
            accLogin.Show();
        }
        private void CheckForUpdate()
        {
            AssistApplication.AppInstance.Log.Normal("Checking For Updates");
            AssistApplication.AppInstance.AssistApiController.CheckForAssistUpdates();
            Trace.WriteLine("Update Done?");
            
        }
        public bool isFirstTime()
        {
            // Returns True if it is the first time setup, by checking if the settings file excists
            return !File.Exists(UserSettings.SettingsFilePath) ? true : false;
        }
        private async Task DefaultLogin()
        {
            for (int i = 0; i < UserSettings.Instance.Accounts.Count; i++)
            {
                RiotUser defLoginUser = new RiotUser();

                foreach (Cookie cookie in UserSettings.Instance.Accounts[i].Convert64ToCookies().GetAllCookies())
                {
                    defLoginUser.UserClient.CookieContainer.Add(cookie);
                }

                var gamename = UserSettings.Instance.Accounts[i].Gamename;
                var tagLine = UserSettings.Instance.Accounts[i].Tagline;
                try
                {
                    AssistApplication.AppInstance.Log.Normal($"Authentcating with Cookies for User {UserSettings.Instance.Accounts[i].puuid} / {gamename}#{tagLine}");
                    await defLoginUser.Authentication.AuthenticateWithCookies();
                }
                catch (Exception ex)
                {
                    AssistApplication.AppInstance.Log.Error($"ACCOUNT NO LONGER VALID - {gamename}#{tagLine} || Removing Account");
                    AssistApplication.AppInstance.OpenAssistErrorWindow(ex, $"Could not login to account: {gamename}#{tagLine}, it has been removed from your account list. Please re-add the account.");
                    UserSettings.Instance.Accounts.Remove(UserSettings.Instance.Accounts[i]);
                    i--;
                    UserSettings.Instance.Save();
                }


                if (defLoginUser.AuthType == AuthType.Cookie)
                {
                    UserSettings.Instance.Accounts[i].ConvertCookiesTo64(defLoginUser.UserClient.CookieContainer); // Resaves cookies after each login to prevent invalid cookies.

                    if (UserSettings.Instance.DefaultAccount is null)
                        UserSettings.Instance.DefaultAccount = UserSettings.Instance.Accounts[i].puuid;

                    if (!string.IsNullOrEmpty(defLoginUser.tokenData.entitle) && !string.IsNullOrEmpty(defLoginUser.tokenData.access))
                    {
                        AssistApplication.AppInstance.currentAccount = UserSettings.Instance.Accounts[i];
                        AssistApplication.AppInstance.currentUser = defLoginUser;
                        break;
                    }

                    
                }


            }
        }
        public string CurrentStatusMessage
        {
            get { return currentStatusMessage; }
            set
            {
                currentStatusMessage = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string message = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(message));
        }
    }
}