using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.MVVM.ViewModel;
using Assist.Settings;
using AssistWPFTest.MVVM.ViewModel;
using ValNet;
using ValNet.Objects.Authentication;
using System.Globalization;

namespace Assist.MVVM.View.Authentication.ViewModels
{
    internal class UsernameAuthViewmodel : ViewModelBase
    {
        public static UsernameAuthViewmodel instanceModel;
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public UsernameAuthViewmodel()
        {
            instanceModel = this;
        }


        internal RiotUser user;
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
                        Authentication.ContentFrame.Navigate(new Uri("/MVVM/View/Authentication/AuthenticationPages/TwoFactorAuthentication.xaml", UriKind.RelativeOrAbsolute));
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }

            // Save Cookies
            ProfileSetting userSettings = new ProfileSetting()
            {
                Gamename = user.UserData.acct.game_name,
                Tagline = user.UserData.acct.tag_line,
                ProfileUuid = user.UserData.sub,
                Region = user.UserRegion,
            };

            userSettings.ConvertCookiesTo64(user.UserClient.CookieContainer);
            await userSettings.SetupProfile(user);

            if (UserSettings.Instance.DefaultAccount == null)
                UserSettings.Instance.DefaultAccount = userSettings.ProfileUuid;

            AssistSettings.Current.Profiles.Add(userSettings);
            AssistApplication.AppInstance.CurrentUser = user;
            AssistApplication.AppInstance.CurrentProfile = userSettings;
            AssistApplication.AppInstance.OpenAssistMainWindow();
        }

        public async Task SubmitFactorCode(string code)
        {
            var resp = await user.Authentication.AuthenticateTwoFactorCode(code);

            if (resp.error is not null && resp.error == "multifactor_attempt_failed")
            {
                ErrorMessage = Properties.Languages.Lang.Authentication_2FactorPlaceholder;
                return;
            }

            if (resp.bIsAuthComplete)
            {
                // Save Cookies
                ProfileSetting userSettings = new ProfileSetting()
                {
                    Gamename = user.UserData.acct.game_name,
                    Tagline = user.UserData.acct.tag_line,
                    ProfileUuid = user.UserData.sub,
                    Region = user.UserRegion,
                };

                userSettings.ConvertCookiesTo64(user.UserClient.CookieContainer);
                await userSettings.SetupProfile(user);

                AssistSettings.Current.Profiles.Add(userSettings);
                AssistApplication.AppInstance.CurrentUser = user;
                AssistApplication.AppInstance.CurrentProfile = userSettings;
                AssistApplication.AppInstance.OpenAssistMainWindow();
            }
            
        }
    }
}
