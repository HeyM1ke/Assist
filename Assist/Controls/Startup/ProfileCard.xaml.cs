using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Assist.Controls.Startup.ViewModel;
using Assist.Controls.Store.ViewModels;
using Assist.Modules.Popup;
using Assist.MVVM.ViewModel;
using Assist.Settings;
using UserControl = System.Windows.Controls.UserControl;

namespace Assist.Controls.Startup
{
    /// <summary>
    /// Interaction logic for ProfileCard.xaml
    /// </summary>
    public partial class ProfileCard : UserControl
    {
        private ProfileCardViewModel _viewModel;
        public ProfileCard()
        {
            DataContext = _viewModel = new ProfileCardViewModel();
            InitializeComponent();
        }

        public ProfileCard(ProfileSetting setting)
        {
            DataContext = _viewModel = new ProfileCardViewModel();
            _viewModel.Profile = setting;
            InitializeComponent();
        }

        private void ProfileCard_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.ProfileImage = App.LoadImageUrl($"https://cdn.assistapp.dev/PlayerCards/{_viewModel.Profile.PCID}_DisplayIcon.png", 80, 80);
            _viewModel.PlayerRankIcon = App.LoadImageUrl($"pack://application:,,,/Resources/RankLogos/TX_CompetitiveTier_Large_{_viewModel.Profile.Tier}.png");
            
        }

        private async void ProfileCard_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.Profile.Equals(AssistApplication.AppInstance.CurrentProfile))
            {
                if (_viewModel.Profile is null)
                    return;

                MVVM.View.Startup.Startup.countdownTimer.Stop();
                await AssistApplication.AppInstance.AuthenticateWithProfileSetting(_viewModel.Profile);
            }
        }
    }
}
