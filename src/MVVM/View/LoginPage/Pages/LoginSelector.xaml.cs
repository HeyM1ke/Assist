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
    /// Interaction logic for LoginSelector.xaml
    /// </summary>
    public partial class LoginSelector : Page
    {
        public LoginSelector(bool addAccMode)
        {
            InitializeComponent();
            if (addAccMode)
                HomeBTN.Visibility = Visibility.Visible;

        }

        private void userPassLogin_Click(object sender, RoutedEventArgs e)
        {
            AccountLogin.current.loginFrame.Navigate(new Uri("/MVVM/View/LoginPage/Pages/LoginPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void riotGamesLogin_Click(object sender, RoutedEventArgs e)
        {
            AccountLogin.current.loginFrame.Navigate(new Uri("/MVVM/View/LoginPage/Pages/LoginPagev2.xaml", UriKind.RelativeOrAbsolute));
        }

        private void HomeBTN_Click(object sender, RoutedEventArgs e)
        {
            MVVM.ViewModel.AssistApplication.AppInstance.OpenAssistMainWindow();
        }
    }
}
