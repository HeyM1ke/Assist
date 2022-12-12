using Assist.Controls.Global;
using Assist.Controls.Settings.ViewModels;
using Assist.Services.Popup;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Assist.Controls.Settings
{
    public partial class ProfileControl : UserControl
    {
        private readonly ProfileControlViewModel _viewModel;

        public ProfileControl()
        {
            DataContext = _viewModel = new ProfileControlViewModel();
            InitializeComponent();
        }
        public ProfileControl(ProfileSettings profile)
        {
            DataContext = _viewModel = new ProfileControlViewModel();
            _viewModel.ProfileLinked = profile;
            InitializeComponent();
            _viewModel.SetupProfile();
        }

        
        private void SwitchBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            btn.IsEnabled = false;
            _viewModel.SwitchProfile();
            btn.IsEnabled = !false;
            
        }

        private void RemoveBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            
            var btn = sender as Button;
            btn.IsEnabled = false;
            _viewModel.RemoveProfile();
            btn.IsEnabled = !false;

        }
    }
}
