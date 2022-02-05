using Assist.Settings;
using System;
using System.Collections.Generic;
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

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistAccountSettingsDefaultAccount.xaml
    /// </summary>
    public partial class AssistAccountSettingsDefaultAccount : UserControl
    {
        public AssistAccountSettingsDefaultAccount()
        {
            InitializeComponent();
        }

        private async void Control_Initialized(object sender, EventArgs e)
        {
            foreach (var Account in UserSettings.Instance.Accounts)
            {
                if(Account.puuid == UserSettings.Instance.DefaultAccount)
                {
                    var item = AccountComboBox.Items.Add(new ComboBoxItem { Content = $"{Account.Gamename}#{Account.Tagline}" });
                    
                    AccountComboBox.SelectedIndex = AccountComboBox.Items.Count - 1;
                }
                else
                    AccountComboBox.Items.Add(new ComboBoxItem { Content = $"{Account.Gamename}#{Account.Tagline}"});
            }
        }

        private void AccountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Trace.WriteLine("Selection Changed");
            var result = UserSettings.Instance.FindAccountByGameNameTagLine((string)((ComboBoxItem)((ComboBox)sender).SelectedItem).Content);

            UserSettings.Instance.DefaultAccount = result.puuid;
            UserSettings.Instance.Save();
        }
    }
}
