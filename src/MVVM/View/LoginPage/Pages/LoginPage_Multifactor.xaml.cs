using Assist.MVVM.ViewModel;
using Assist.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Assist.MVVM.View.LoginPage.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage_Multifactor.xaml
    /// </summary>
    public partial class LoginPage_Multifactor : Page
    {
        public LoginPage_Multifactor()
        {
            InitializeComponent();
        }

        private async void SubmitBTN_Click(object sender, RoutedEventArgs e)
        {
            var resp = await AssistApplication.AppInstance.LoginPageViewModel.user.Authentication.AuthenticateTwoFactorCode(verifcationCodeInput.Text);

            if(resp.error is not null && resp.error == "multifactor_attempt_failed")
            {
                errorMessage.Content = "Multifactor Code is not correct.";
                return;
            }

            if (resp.bIsAuthComplete)
            {
                // Save Cookies
                AccountSettings userSettings = new AccountSettings()
                {
                    Gamename = AssistApplication.AppInstance.LoginPageViewModel.user.UserData.acct.game_name,
                    Tagline = AssistApplication.AppInstance.LoginPageViewModel.user.UserData.acct.tag_line,
                    puuid = AssistApplication.AppInstance.LoginPageViewModel.user.UserData.sub,
                    Region = AssistApplication.AppInstance.LoginPageViewModel.user.UserRegion,
                };

                userSettings.ConvertCookiesTo64(AssistApplication.AppInstance.LoginPageViewModel.user.UserClient.CookieContainer);

                // Add login to Local Settings for future use, if Remember Me is chosen.
                UserSettings.Instance.Accounts.Add(userSettings);

                // Open Mainwindow
                //Application.Current.MainWindow.Close();

                AssistApplication.AppInstance.currentUser = AssistApplication.AppInstance.LoginPageViewModel.user;
                AssistApplication.AppInstance.currentAccount = userSettings;
                AssistApplication.AppInstance.OpenAssistMainWindow();
            }
        }
    }
}
