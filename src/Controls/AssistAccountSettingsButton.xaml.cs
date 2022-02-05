using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using Assist.Settings;

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistAccountSettingsButton.xaml
    /// </summary>
    public partial class AssistAccountSettingsButton : UserControl
    {
        public AssistAccountSettingsButton()
        {
            InitializeComponent();
        }

        internal AccountSettings Account
        {
            get { return (AccountSettings)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Account.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AccountProperty =
            DependencyProperty.Register("Account", typeof(AccountSettings), typeof(AssistAccountSettingsButton));




        public double checkmarkOpac
        {
            get { return (double)GetValue(checkmarkOpacProperty); }
            set { SetValue(checkmarkOpacProperty, value); }
        }

        // Using a DependencyProperty as the backing store for checkmarkOpac.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty checkmarkOpacProperty =
            DependencyProperty.Register("checkmarkOpac", typeof(double), typeof(AssistAccountSettingsButton));



        public string accountRegion
        {
            get { return (string)GetValue(accountRegionProperty); }
            set { SetValue(accountRegionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for accountRegion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty accountRegionProperty =
            DependencyProperty.Register("accountRegion", typeof(string), typeof(AssistAccountSettingsButton));



        public string accountName
        {
            get { return (string)GetValue(accountNameProperty); }
            set { SetValue(accountNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for accountName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty accountNameProperty =
            DependencyProperty.Register("accountName", typeof(string), typeof(AssistAccountSettingsButton));


        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonClick;


        private void Delete_Icon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Account is null)
                return;

            AssistApplication.AppInstance.Log.Normal("Removing Account");
            UserSettings.Instance.Accounts.Remove(Account);

            // Creates fallback to default account if the default account is invalid
            if (UserSettings.Instance.Accounts.Count != 0)
                UserSettings.Instance.DefaultAccount = UserSettings.Instance.Accounts[0].puuid;
            else
                UserSettings.Instance.DefaultAccount = null;
            
            UserSettings.Instance.Save();

            // Self-Remove from list

            ((StackPanel)this.Parent).Children.Remove(this);

            //Reload Window with removed Acc

            if(UserSettings.Instance.Accounts.Count == 0)
                AssistApplication.AppInstance.OpenAccountLoginWindow();
            else
                AssistApplication.AppInstance.OpenAssistMainWindowToSettings();
        }

        
    }
}
