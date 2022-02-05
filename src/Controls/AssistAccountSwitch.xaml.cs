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
using Assist.Settings;
using ValNet.Objects;

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistAccountSwitch.xaml
    /// </summary>
    public partial class AssistAccountSwitch : UserControl
    {
        AssistApplication _viewModel => AssistApplication.AppInstance;

        public AssistAccountSwitch()
        {
            DataContext = _viewModel;
            InitializeComponent();
            LoadAccounts();
        }

        private async void LoadAccounts()
        {
            accountLimitLabel.Content = $"{UserSettings.Instance.Accounts.Count}/5 Accounts";
            foreach(AccountSettings account in UserSettings.Instance.Accounts)
            {
                if (account.puuid == AssistApplication.AppInstance.currentAccount.puuid)
                {
                    var currentBtn = new AssistAccountSwitchButton()
                    {
                        Height = 50,
                        checkmarkOpac = 1,
                        accountRegion = Enum.GetName(typeof(RiotRegion), account.Region),
                        accountName = $"{ account.Gamename}#{account.Tagline}",
                        Margin = new Thickness(0, 0, 0, 5),
                        Account = account
                    };

                    accountList.Children.Add(currentBtn);
                    
                }
                else
                {
                    var button = new AssistAccountSwitchButton()
                    {
                        Height = 50,
                        accountRegion = Enum.GetName(typeof(RiotRegion), account.Region),
                        accountName = $"{ account.Gamename}#{account.Tagline}",
                        checkmarkOpac = 0,
                        Margin = new Thickness(0, 0, 0, 5),
                        Account = account
                    };

                    button.ButtonClick += new EventHandler(generatedButton_Click);

                    accountList.Children.Add(button);
                }

                
            }
        }

        private async void generatedButton_Click(object sender, EventArgs e)
        {
            await _viewModel.AccountSwitchControlViewModel.LoginIntoAccount(sender);
        }

        private async void addAccountBTN_Click(object sender, RoutedEventArgs e)
        {
            if(UserSettings.Instance.Accounts.Count == 5)
            {
                _viewModel.Log.Error("User tried adding account while at account limit.");
                _viewModel.OpenAssistErrorWindow(new Exception("Account Limit reached, please remove an account to add a new one."));
            }
            else
            {
                MVVM.View.Extra.AccountSwitch.instance.Close();
                _viewModel.OpenAccountLoginWindow();
            }
            
        }
    }
}
