using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Assist.Controls.Profile.ViewModel;
using Assist.Modules.Popup;
using Assist.MVVM.ViewModel;
using Assist.Settings;
using ValNet.Objects;

namespace Assist.Controls.Profile
{
    /// <summary>
    /// Interaction logic for ProfileShowcase.xaml
    /// </summary>
    public partial class ProfileShowcase : UserControl
    {
        private ProfileShowcaseViewModel _viewModel;
        public ProfileShowcase()
        {
            DataContext = _viewModel = new ProfileShowcaseViewModel();
            InitializeComponent();
        }

        public ProfileShowcase(ProfileSetting profile)
        {
            DataContext = _viewModel = new ProfileShowcaseViewModel();
            _viewModel.Profile = profile;
            InitializeComponent();
        }

        private void ProfileShowcase_Loaded(object sender, RoutedEventArgs e)
        {

            NoteBox.Text = _viewModel.Profile.profileNote;
            _viewModel.ProfileImage = App.LoadImageUrl($"https://cdn.assistapp.dev/PlayerCards/{_viewModel.Profile.PCID}_DisplayIcon.png", 80, 80);
            _viewModel.PlayerRankIcon = App.LoadImageUrl($"pack://application:,,,/Resources/RankLogos/TX_CompetitiveTier_Large_{_viewModel.Profile.Tier}.png");
        }

        private void SwitchBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.Profile.Equals(AssistApplication.AppInstance.CurrentProfile))
            {
                if(_viewModel.Profile is null)
                    return;

                PopupSystem.SpawnPopup(new PopupSettings()
                {
                    PopupTitle = "Switching Accounts",
                    PopupType = PopupType.LOADING
                });

                AssistApplication.AppInstance.AuthenticateWithProfileSetting(_viewModel.Profile);
            }
        }

        private void NoteInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.UpdateProfileNote(NoteBox.Text);
        }

        private async void RemoveBTN_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.RemoveProfile();
            var t = VisualTreeHelper.GetParent(this);
            if (t != null)
            {
                var g = t as WrapPanel;
                g.Children.Remove(this);
            }
        }

    }
}
