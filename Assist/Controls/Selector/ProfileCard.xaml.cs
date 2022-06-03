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
using Assist.Controls.Selector.ViewModel;
using Assist.Controls.Store.ViewModels;
using Assist.Modules.Popup;
using Assist.MVVM.ViewModel;
using Assist.Settings;
using Serilog;
using UserControl = System.Windows.Controls.UserControl;

namespace Assist.Controls.Selector
{
    /// <summary>
    /// Interaction logic for ProfileCard.xaml
    /// </summary>
    public partial class ProfileCard : UserControl
    {
        public ProfileCardViewModel ViewModel;
        public ProfileCard()
        {
            DataContext = ViewModel = new ProfileCardViewModel();
            InitializeComponent();
        }

        public ProfileCard(ProfileSetting setting)
        {
            DataContext = ViewModel = new ProfileCardViewModel();
            ViewModel.Profile = setting;
            InitializeComponent();
        }

        private void ProfileCard_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.ProfileImage = App.LoadImageUrl($"https://cdn.assistapp.dev/PlayerCards/{ViewModel.Profile.PCID}_DisplayIcon.png", 80, 80);
            ViewModel.PlayerRankIcon = App.LoadImageUrl($"pack://application:,,,/Resources/RankLogos/TX_CompetitiveTier_Large_{ViewModel.Profile.Tier}.png");
        }


        private void StopStartupTimer()
        {
            Log.Information("Stopped Startup Timer");
            MVVM.View.Selector.Startup.countdownTimer.Stop();
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (this.Click != null)
            {
                this.Click(this, e);
            }
        }

        public event RoutedEventHandler Click;

    }
}
