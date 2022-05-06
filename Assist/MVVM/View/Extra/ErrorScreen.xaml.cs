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
    /// Interaction logic for ErrorScreen.xaml
    /// </summary>
    public partial class ErrorScreen : Window
    {
        Exception ex { get; set; }
        public ErrorScreen(Exception exData, string? extraData = null)
        {
            ex = exData;
            InitializeComponent();
            errorText.Text = ex.Message + $"\n\n{extraData}";
        }

        private void errorOkBTn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region basic
        private void minimizeBTN_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void windowBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        #endregion
    }
}
