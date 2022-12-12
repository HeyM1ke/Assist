using Assist.Views.Authentication.Sections.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Assist.Views.Authentication.Sections
{
    public partial class RiotClientAuthenticationView : UserControl
    {
        private readonly RCAuthViewModel _viewModel;
        public RiotClientAuthenticationView()
        {
            DataContext = _viewModel = new RCAuthViewModel();
            InitializeComponent();
        }

        private async void LoginStartBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            LoginStartBtn.IsEnabled = false;
            await _viewModel.StartLogin();
            
        }
    }
}
