using Assist.Services;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace Assist.Views.Authentication.Sections
{
    public partial class AuthSelection : UserControl
    {
        private ASelectVM _viewModel;
        public AuthSelection()
        {
            DataContext = _viewModel = new ASelectVM();
            InitializeComponent();
        }

        
        private void UsernameBtn_OnPointerEnter(object? sender, PointerEventArgs e)
        {
            _viewModel.AuthTypePopup = Properties.Resources.Authentication_UsernamePasswordDescription;
        }

        private void RiotCloud_OnPointerEnter(object? sender, PointerEventArgs e)
        {
            _viewModel.AuthTypePopup = "Login with your Riot Games Login through a Browser.";
        }

        private void RiotClient_OnPointerEnter(object? sender, PointerEventArgs e)
        {
            _viewModel.AuthTypePopup = Properties.Resources.Authentication_RiotClientTitleDescription;
        }
        
        private void AuthMethod_OnPointerLeave(object? sender, PointerEventArgs e)
        {
            //_viewModel.AuthTypePopup = string.Empty; Looks Better?
        }

        private void UsernameBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            AuthenticationNavigationController.Change(new UsernameAuthentication());
        }

        private void RiotClient_OnClick(object? sender, RoutedEventArgs e)
        {
            AuthenticationNavigationController.Change(new RiotClientAuthenticationView());
        }
    }

    class ASelectVM : ViewModelBase
    {
        private string _authTypePopup;
        public string AuthTypePopup
        {
            get => _authTypePopup;
            set => this.RaiseAndSetIfChanged(ref _authTypePopup, value);
        }
    }
}