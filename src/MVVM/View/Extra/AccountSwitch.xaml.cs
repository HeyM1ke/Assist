using Assist.MVVM.ViewModel;
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

namespace Assist.MVVM.View.Extra
{
    /// <summary>
    /// Interaction logic for AccountSwitch.xaml
    /// </summary>
    public partial class AccountSwitch : Window
    {

        AssistApplication _viewModel => AssistApplication.AppInstance;
        public static AccountSwitch instance;
        public AccountSwitch()
        {
            DataContext = _viewModel;
            instance = this;
            InitializeComponent();
        }


        #region Basic
        private void minimizeBTN_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void windowBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        #endregion
    }
}
