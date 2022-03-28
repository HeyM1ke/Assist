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
using Assist.Controls.Store.ViewModels;
using Assist.MVVM.ViewModel;

namespace Assist.Controls.Store
{
    /// <summary>
    /// Interaction logic for ProfileShowcaseStore.xaml
    /// </summary>
    public partial class ProfileShowcaseStore : UserControl
    {
        private ProfileShowcaseStoreViewModel _viewModel;
        public ProfileShowcaseStore()
        {
            DataContext = _viewModel = new ProfileShowcaseStoreViewModel();
            _viewModel.Profile = AssistApplication.AppInstance.CurrentProfile;
            InitializeComponent();
            SetupControl();
        }

        private async void SetupControl()
        {
            _viewModel.ProfileImage = await App.LoadImageUrl("https://media.valorant-api.com/playercards/" + _viewModel.Profile.PCID + "/smallart.png", 79, 79);
            _viewModel.BackingImage = await App.LoadImageUrl("https://cdn.rumblemike.com/Maps/2FB9A4FD-47B8-4E7D-A969-74B4046EBD53_splash.png", 217, 89);

            if(_viewModel.Profile is not null)
                await _viewModel.GetPlayerBalance();
        }

        private void Switch_Click(object sender, RoutedEventArgs e)
        {
            AssistMainWindow.Current.UncheckBtns();
            AssistMainWindow.Current.ContentFrame.Navigate(new Uri("/MVVM/View/Profiles/Profiles.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
