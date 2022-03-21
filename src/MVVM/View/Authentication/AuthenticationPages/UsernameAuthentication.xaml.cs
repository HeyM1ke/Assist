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

using System.Globalization;
using Assist.MVVM.View.Authentication.ViewModels;
using ValNet;
using ValNet.Objects.Authentication;

namespace Assist.MVVM.View.Authentication.AuthenticationPages
{
    /// <summary>
    /// Interaction logic for UsernameAuthentication.xaml
    /// </summary>
    public partial class UsernameAuthentication : Page
    {
        private UsernameAuthViewmodel _viewModel;
        public UsernameAuthentication()
        {
            DataContext = _viewModel = new UsernameAuthViewmodel();
            InitializeComponent();
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            Authentication.ContentFrame.GoBack();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginBtn.IsEnabled = false;
            RiotLoginData loginData = new RiotLoginData()
            {
                username = usernameBox.Text,
                password = passwordBox.passwordBox.Password
            };

            await _viewModel.Login(loginData);
            LoginBtn.IsEnabled = true;
        }
    }
}
