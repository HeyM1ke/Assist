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

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for PlayLaunchControl.xaml
    /// </summary>
    public partial class PlayLaunchControl : UserControl
    {
        LaunchControlViewModel _viewModel;

        public PlayLaunchControl()
        {
            //this.DataContext = _viewModel = AssistApplication.AppInstance.LaunchControlViewModel = new LaunchControlViewModel();
            InitializeComponent();
            accountNameShow.Text = $"Logged in as: {AssistApplication.AppInstance.currentAccount.Gamename}#{AssistApplication.AppInstance.currentAccount.Tagline}";
        }

        private async void launchBTN_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.LaunchGame();
        }

        private void SettingsPanel_Loaded(object sender, RoutedEventArgs e)
        {
            discordRpcToggle.toggle.Checked += discordRpcToggle_Checked;
            discordRpcToggle.toggle.Unchecked += discordRpcToggle_UnChecked;
            discordRpcToggle.IsCheck = UserSettings.Instance.LaunchSettings.ValDscRpcEnabled;


            if (!string.IsNullOrEmpty(UserSettings.Instance.LaunchSettings.CustomValParams))
            {
                customParamToggle.IsCheck = true;
                customParamInput.Text = UserSettings.Instance.LaunchSettings.CustomValParams;
                
            }
        }

        private void discordRpcToggle_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateDiscordSetting(true);
        }

        private void discordRpcToggle_UnChecked(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateDiscordSetting(false);
        }

        private void customParamInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.UpdateParamSetting(customParamInput.Text); 
        }
    }
}
