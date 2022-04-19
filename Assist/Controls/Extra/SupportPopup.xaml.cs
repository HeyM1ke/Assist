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
using Assist.Modules.Popup;

namespace Assist.Controls.Extra
{
    /// <summary>
    /// Interaction logic for SupportPopup.xaml
    /// </summary>
    public partial class SupportPopup : UserControl
    {
        public SupportPopup()
        {
            InitializeComponent();
        }

        private void Back_Btn_Click(object sender, RoutedEventArgs e)
        {
            PopupSystem.KillPopups();
        }

        private void Support_Btn_Click(object sender, RoutedEventArgs e)
        {
            // Open to Support Page
        }
    }
}
