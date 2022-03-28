using System.Windows;
using System.Windows.Controls;
using Assist.MVVM.ViewModel;
using ValNet.Objects.Authentication;

namespace Assist.MVVM.View.LoginPage.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {

        AssistApplication _viewModel => AssistApplication.AppInstance;
        public LoginPage()
        {
            DataContext = _viewModel;
            InitializeComponent();
        }

        /*private void ShowAccMode()
        {
            rememberBox.Visibility = Visibility.Hidden;
            LoginBTN.Visibility = Visibility.Hidden;
            addBTN.Visibility = Visibility.Visible;
            backBtn.Visibility = Visibility.Visible;
        }*/


        private async void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            RiotLoginData loginData = new RiotLoginData()
            {
                username = usernameInput.Text,
                password= passwordInput.passwordBox.Password
            };

            await _viewModel.LoginPageViewModel.Login(loginData);
        }

        private async void addBTN_Click(object sender, RoutedEventArgs e)
        {
            RiotLoginData loginData = new RiotLoginData()
            {
                username = usernameInput.Text,
                password = passwordInput.passwordBox.Password
            };
            
            await _viewModel.LoginPageViewModel.Login();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            AccountLogin.current.loginFrame.GoBack();
        }
    }
}
