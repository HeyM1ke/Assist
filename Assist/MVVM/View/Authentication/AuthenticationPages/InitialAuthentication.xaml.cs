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
using Assist.MVVM.ViewModel;

namespace Assist.MVVM.View.Authentication.AuthenticationPages
{
    /// <summary>
    /// Interaction logic for InitialAuthentication.xaml
    /// </summary>
    public partial class InitialAuthentication : Page
    {
        public InitialAuthentication()
        {
            InitializeComponent();
            if(!Authentication.bAddMode)
                HomeBtn.Visibility = Visibility.Collapsed;
        }
        private void UsernameBTN_Click(object sender, RoutedEventArgs e)
        {
            Authentication.ContentFrame.Navigate(new Uri(
                "MVVM/View/Authentication/AuthenticationPages/UsernameAuthentication.xaml",
                UriKind.RelativeOrAbsolute));
        }

        private void RitoBTN_Click(object sender, RoutedEventArgs e)
        {
            Authentication.ContentFrame.Navigate(new Uri(
                "MVVM/View/Authentication/AuthenticationPages/RitoAuthentication.xaml",
                UriKind.RelativeOrAbsolute));
        }

        private void HomeBTN_Click(object sender, RoutedEventArgs e)
        {
            AssistApplication.AppInstance.OpenAssistMainWindow();
        }
    }
}
