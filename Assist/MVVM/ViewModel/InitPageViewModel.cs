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
using System.Threading;
using Assist.MVVM.ViewModel;

namespace Assist.MVVM.ViewModel
{
    class InitPageViewModel : ViewModelBase
    {
        private string _currentStatusMessage = "Loading..";
        public string CurrentStatusMessage
        {
            get => _currentStatusMessage;
            set => SetProperty(ref _currentStatusMessage, value);
        }
        public async Task FirstTimeSetup()
        {
            if (AssistApplication.AppInstance.AssistApiController.bIsUpdate)
                return;

            AssistLog.Normal("Running First Time Setup");
            var clientPath = await AssistSettings.Current.FindRiotClient();

            if (clientPath is null)
            {
                // Do Something if the client path is invalid and the client does not excist.
                AssistLog.Error("NO RIOT CLIENT FOUND ON COMPUTER / IN SETTINGS FILE");
                CurrentStatusMessage = "No Riot Client Found, Please install Riot Client.";
                Thread.Sleep(5000);
                Environment.Exit(0);
            }

            CurrentStatusMessage = "Riot Client Found";
            AssistLog.Normal("Setting Settings RiotClient Path to Settings, Path Found: " + clientPath);
            AssistSettings.Current.RiotClientInstallPath = clientPath; // Set the Client path to settings.


            // Next Step, Add account.
            AssistApplication.AppInstance.OpenAccountLoginWindow(false);
        }
        public async Task DefaultStartup()
        {
            if (AssistApplication.AppInstance.AssistApiController.bIsUpdate)
                return;

            AssistLog.Normal("Calling Default Startup");

            //Check if RiotClient is Valid
            var clientPath = await AssistSettings.Current.FindRiotClient();
            if (clientPath != null)
                AssistSettings.Current.RiotClientInstallPath = clientPath; // Set the Client path to settings.
            else
            {
                var ex = new Exception(
                    "Riot Client was not found on PC, Please install the riot client. If you have the riot client installed. Please Look for Support on Discord.");
                AssistApplication.AppInstance.OpenAssistErrorWindow(ex, "");
                Environment.Exit(1);

            }


            //Check if There are accounts within settings.
            if(AssistSettings.Current.Profiles.Count == 0)
            {
                AssistLog.Normal("No Accounts Found in User's Settings, Opening Account Login.");
                AssistApplication.AppInstance.OpenAccountLoginWindow(false);
            }
            else
            {
                if (!string.IsNullOrEmpty(AssistSettings.Current.DefaultAccount))
                {
                    var profile = AssistSettings.Current.FindProfileById(AssistSettings.Current.DefaultAccount);

                    if (profile != null)
                    {
                        RiotUser user = new RiotUser();

                        
                        try
                        {
                            user = await AssistAuthenticationController.ProfileLogin(profile);
                        }
                        catch (Exception ex)
                        {
                            AssistLog.Error($"Account is no longer valid - {profile.RiotId} | Removing Account");
                            AssistApplication.AppInstance.OpenAssistErrorWindow(ex, $"Login for account: {profile.RiotId} is expired. Please re-add the account.");
                            AssistSettings.Current.Profiles.Remove(profile);

                            // Creates fallback to default account if the default account is invalid
                            if (AssistSettings.Current.Profiles.Count != 0)
                                AssistSettings.Current.DefaultAccount = AssistSettings.Current.Profiles[0].ProfileUuid;
                            else
                            {
                                AssistSettings.Current.DefaultAccount = null;
                            }
                            AssistSettings.Save();

                            await DefaultLogin(); // Run Default Login
                        }

                        if (user.AuthType == AuthType.Cookie)
                        {
                            profile.ConvertCookiesTo64(user.UserClient.CookieContainer); // Resaves cookies after each login to prevent invalid cookies.
                            await profile.SetupProfile(user);
                            if (!string.IsNullOrEmpty(user.tokenData.entitle) && !string.IsNullOrEmpty(user.tokenData.access))
                            {
                                AssistApplication.AppInstance.CurrentProfile = profile;
                                AssistApplication.AppInstance.CurrentUser = user;
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


                if (AssistApplication.AppInstance.CurrentProfile == null || AssistApplication.AppInstance.CurrentUser == null)
                {
                    AssistLog.Normal("No Accounts were successfully logged into, opening account login.");
                    AssistSettings.Current.Profiles.Clear();
                    AssistSettings.Save();
                    AssistApplication.AppInstance.OpenAccountLoginWindow(false);
                }
                else
                {
                    AssistLog.Normal("Login was successful, opening main window.");
                    AssistApplication.AppInstance.OpenAssistMainWindow();
                }

            }

            
        }
        private async Task DefaultLogin()
        {
            for (int i = 0; i < AssistSettings.Current.Profiles.Count; i++)
            {
                var profile = AssistSettings.Current.Profiles[i];

                RiotUser user = new RiotUser(); ;

                try
                {
                    user = await AssistAuthenticationController.ProfileLogin(profile);

                }
                catch (Exception ex)
                {
                    AssistLog.Error($"ACCOUNT NO LONGER VALID - {profile.RiotId} || Removing Account");
                    AssistApplication.AppInstance.OpenAssistErrorWindow(ex, $"Could not login to account: {profile.RiotId}, it has been removed from your account list. Please re-add the account.");
                    AssistSettings.Current.Profiles.Remove(profile);
                    AssistSettings.Save();
                    i--;
                }

                if (user.AuthType == AuthType.Cookie)
                {
                    profile.ConvertCookiesTo64(user.UserClient.CookieContainer); // Resaves cookies after each login to prevent invalid cookies.
                    profile.SetupProfile(user);

                    if (AssistSettings.Current.DefaultAccount is null)
                        AssistSettings.Current.DefaultAccount = AssistSettings.Current.Profiles[i].ProfileUuid;

                    if (!string.IsNullOrEmpty(user.tokenData.entitle) && !string.IsNullOrEmpty(user.tokenData.access))
                    {
                        AssistApplication.AppInstance.CurrentProfile = profile;
                        AssistApplication.AppInstance.CurrentUser = user;
                        break;
                    }


                }


            }
        }

    }
}