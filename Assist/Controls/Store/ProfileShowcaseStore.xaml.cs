using Assist.Controls.Store.ViewModels;
using Assist.MVVM.ViewModel;

using System;
using System.Windows;

namespace Assist.Controls.Store
{
    /// <summary>
    /// Interaction logic for ProfileShowcaseStore.xaml
    /// </summary>
    public partial class ProfileShowcaseStore
    {

        private readonly ProfileShowcaseStoreViewModel _viewModel;

        public ProfileShowcaseStore()
        {
            DataContext = _viewModel = new ProfileShowcaseStoreViewModel();
            _viewModel.Profile = AssistApplication.AppInstance.CurrentProfile;

            InitializeComponent();
            SetupControl();
        }

        private async void SetupControl()
        {
            _viewModel.ProfileImage = App.LoadImageUrl("https://cdn.assistapp.dev/PlayerCards/" + _viewModel.Profile.PCID + "_DisplayIcon.png", 79, 79);
            _viewModel.BackingImage = App.LoadImageUrl("https://cdn.rumblemike.com/Maps/2FB9A4FD-47B8-4E7D-A969-74B4046EBD53_splash.png", 217, 89);

            if(_viewModel.Profile != null)
                await _viewModel.GetPlayerBalance();
        }

        private void Switch_Click(object sender, RoutedEventArgs e)
        {
            AssistMainWindow.Current.UncheckNavigationButtons();
            AssistMainWindow.Current.ContentFrame.Navigate(new Uri("/MVVM/View/Profiles/Profiles.xaml", UriKind.RelativeOrAbsolute));
        }

    }
}
