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

namespace Assist.MVVM.View.Authentication.AuthenticationPages
{
    /// <summary>
    /// Interaction logic for RitoAuthentication.xaml
    /// </summary>
    public partial class RitoAuthentication : Page
    {
        public RitoAuthentication()
        {
            InitializeComponent();
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            Authentication.ContentFrame.GoBack();
        }
    }
}
