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
using System.Windows.Shapes;
using Assist.MVVM.View.LoginPage.Pages;
using Assist.MVVM.ViewModel;
using Assist.Settings;

namespace Assist
{
    /// <summary>
    /// Interaction logic for AccountLogin.xaml
    /// </summary>
    public partial class AccountLogin : Window
    {
        AssistApplication _viewModel => AssistApplication.AppInstance;
        public static AccountLogin current;
        public AccountLogin(bool addAccMode)
        {
            DataContext = _viewModel;
            current = this;
            InitializeComponent();
            loginFrame.Content = new LoginSelector(addAccMode);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void windowBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
