using Assist.MVVM.View.Extra;
using Assist.Settings;

using Serilog;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using ValNet;
using ValNet.Objects;

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
            Log.Information("Running First Time Setup");
            var clientPath = await AssistSettings.Current.FindRiotClient();

            if (clientPath is null)
            {
                // Do Something if the client path is invalid and the client does not excist.
                Log.Error("NO RIOT CLIENT FOUND ON COMPUTER / IN SETTINGS FILE");
                CurrentStatusMessage = "No Riot Client Found, Please install Riot Client.";
                Thread.Sleep(5000);
                Environment.Exit(0);
            }

            CurrentStatusMessage = "Riot Client Found";
            Log.Information("Setting Settings RiotClient Path to Settings, Path Found: " + clientPath);
            AssistSettings.Current.RiotClientInstallPath = clientPath; // Set the Client path to settings.

            if (!AssistSettings.Current.SetupLangSelected)
            {
                Application.Current.MainWindow.Visibility = Visibility.Hidden;
                LanguageSelectWindow sW = new LanguageSelectWindow();
                Application.Current.MainWindow.Close();
                sW.Show();
                Application.Current.MainWindow = sW;
                return;
            }

                // Next Step, Add account.
            AssistApplication.AppInstance.OpenAccountLoginWindow(false);
        }
        public async Task DefaultStartup()
        {
            Log.Information("Calling Default Startup");

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
                Log.Information("No Accounts Found in User's Settings, Opening Account Login.");
                AssistApplication.AppInstance.OpenAccountLoginWindow(false);
            }
            else
            {
                if (AssistSettings.Current.UseAccountLaunchSelection)
                {
                    var profile = AssistSettings.Current.FindProfileById(AssistSettings.Current.DefaultAccount);
                    if (profile == null)
                    {
                        if (AssistSettings.Current.Profiles.Count != 0)
                            AssistSettings.Current.DefaultAccount = AssistSettings.Current.Profiles[0].ProfileUuid;
                    }
                    
                    AssistApplication.AppInstance.OpenStartupWindow();
                    return;
                }

                if (!string.IsNullOrEmpty(AssistSettings.Current.DefaultAccount))
                {
                    var profile = AssistSettings.Current.FindProfileById(AssistSettings.Current.DefaultAccount);

                    if (profile != null)
                    {
                        RiotUser user = new RiotUser(AssistApplication.AgentFormat);

                        
                        try
                        {
                            user = await AssistAuthenticationController.ProfileLogin(profile);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Account is no longer valid - {profile.RiotId} | Removing Account");
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
                    Log.Information("No Accounts were successfully logged into, opening account login.");
                    AssistSettings.Current.Profiles.Clear();
                    AssistSettings.Save();
                    AssistApplication.AppInstance.OpenAccountLoginWindow(false);
                }
                else
                {
                    AssistSettings.Save();
                    Log.Information("Login was successful, opening main window.");
                    AssistApplication.AppInstance.OpenAssistMainWindow();
                }

            }

            
        }
        private async Task DefaultLogin()
        {
            for (int i = 0; i < AssistSettings.Current.Profiles.Count; i++)
            {
                var profile = AssistSettings.Current.Profiles[i];

                RiotUser user = new RiotUser(AssistApplication.AgentFormat);

                try
                {
                    user = await AssistAuthenticationController.ProfileLogin(profile);

                }
                catch (Exception ex)
                {
                    if (ex is ValNetException valnetException)
                    {
                        Log.Error("Catched a ValNet exception. ({Message}) StatusCode: {StatusCode}", valnetException.Message, valnetException.RequestStatusCode);
                        Log.Error("Content: {Content}", valnetException.RequestContent);
                    }

                    Log.Error($"ACCOUNT NO LONGER VALID - {profile.RiotId} || Removing Account");
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