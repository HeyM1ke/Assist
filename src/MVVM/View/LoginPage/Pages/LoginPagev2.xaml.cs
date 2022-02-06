using System.Windows;
using System.Windows.Controls;
using Assist.MVVM.ViewModel;
using ValNet.Objects.Authentication;

namespace Assist.MVVM.View.LoginPage.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPagev2 : Page
    {

        LoginPageViewModel _viewModel;
        public LoginPagev2()
        {
            DataContext = _viewModel = new LoginPageViewModel();
            AssistApplication.AppInstance.LoginPageViewModel = _viewModel;
            InitializeComponent();
        }

        private void backBTN_Click(object sender, RoutedEventArgs e)
        {
            AccountLogin.current.loginFrame.GoBack();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new LoginWindow();
            AssistApplication.AppInstance.Log.Normal("Login Window is Open");
            screen.ShowDialog();
            AssistApplication.AppInstance.Log.Normal("Login Window is Completed");
            if(AssistApplication.AppInstance.LoginPageViewModel.cookie_Ssid != null)
                await _viewModel.Login();
        }
    }
}
