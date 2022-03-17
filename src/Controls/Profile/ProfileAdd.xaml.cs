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

namespace Assist.Controls.Profile
{
    /// <summary>
    /// Interaction logic for ProfileAdd.xaml
    /// </summary>
    public partial class ProfileAdd : UserControl
    {
        public ProfileAdd()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AssistApplication.AppInstance.OpenAccountLoginWindow(true);
        }
    }
}
