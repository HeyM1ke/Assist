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
using Assist.MVVM.View.Authentication.AuthenticationPages;


namespace Assist.MVVM.View.Authentication
{
    /// <summary>
    /// Interaction logic for Authentication.xaml
    /// </summary>
    public partial class Authentication : Window
    {
        public static Frame ContentFrame;

        public Authentication()
        {
            InitializeComponent();
            ContentFrame = LoginFrame;

        }


        #region Window Bar

        private void windowBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void minimizeBTN_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion


        private void AuthenticationLoaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Uri("MVVM/View/Authentication/AuthenticationPages/InitialAuthentication.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
