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
using Assist.MVVM.View.Authentication.ViewModels;
using Assist.Settings;

namespace Assist.MVVM.View.Authentication.AuthenticationPages
{
    /// <summary>
    /// Interaction logic for TwoFactorAuthentication.xaml
    /// </summary>
    public partial class TwoFactorAuthentication : Page
    {
        public TwoFactorAuthentication()
        {
            DataContext = UsernameAuthViewmodel.instanceModel;
            InitializeComponent();
        }

        private async void Submit_Btn(object sender, RoutedEventArgs e)
        {
            await UsernameAuthViewmodel.instanceModel.SubmitFactorCode(CodeBox.Text);
            
        }
    }
}
