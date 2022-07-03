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
    /// Interaction logic for RiotForbidWindow.xaml
    /// </summary>
    public partial class RiotForbidWindow : Window
    {
        public RiotForbidWindow()
        {
            InitializeComponent();
        }

        #region basic
        private void windowBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        #endregion

        private void quitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
