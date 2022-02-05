using Assist.MVVM.ViewModel;
using Assist.Settings;
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
using ValNet.Objects;

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistAccountSettingsManager.xaml
    /// </summary>
    public partial class AssistAccountSettingsManager : UserControl
    {
        public AssistAccountSettingsManager()
        {
            InitializeComponent();
        }

        private void AccountManager_Initialized(object sender, EventArgs e)
        {
            foreach(var account in UserSettings.Instance.Accounts)
            {
                if (account.puuid == AssistApplication.AppInstance.currentAccount.puuid)
                {
                    AccountContainer.Children.Add(new AssistAccountSettingsButton()
                    {
                        Height = 70,
                        checkmarkOpac = 1,
                        accountRegion = Enum.GetName(typeof(RiotRegion), account.Region),
                        accountName = $"{ account.Gamename}#{account.Tagline}",
                        Margin = new Thickness(0, 0, 0, 5),
                        Account = account
                    });
                }
                else
                {
                    var button = new AssistAccountSettingsButton()
                    {
                        Height = 70,
                        accountRegion = Enum.GetName(typeof(RiotRegion), account.Region),
                        accountName = $"{ account.Gamename}#{account.Tagline}",
                        checkmarkOpac = 0,
                        Margin = new Thickness(0, 0, 0, 5),
                        Account = account
                    };


                    button.ButtonClick += new EventHandler(generatedButton_Click); ;

                    AccountContainer.Children.Add(button);
                }

                
            }
        }

        private async void generatedButton_Click(object sender, EventArgs e)
        {
            await AssistApplication.AppInstance.AuthenticateWithAccountSetting(((AssistAccountSwitchButton)sender).Account);
        }
    }
}
