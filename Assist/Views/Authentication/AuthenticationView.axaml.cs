using System;
using Assist.Objects.Enums;
using Assist.Services;
using Assist.ViewModels.Authentication;
using Assist.Views.Authentication.Sections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ValNet.Objects.Authentication;

namespace Assist.Views.Authentication
{
    public partial class AuthenticationView : UserControl
    {
        private readonly AuthenticationViewModel _viewModel;
        private EAuthenticationType _type;   
        public AuthenticationView()
        {
            DataContext = _viewModel = new AuthenticationViewModel();
            InitializeComponent();
        }
        /// <summary>
        /// Auth View Constructor to Directly Load into a Auth Method
        /// </summary>
        /// <param name="authT"></param>
        public AuthenticationView(EAuthenticationType authT)
        {
            DataContext = _viewModel = new AuthenticationViewModel();
            _type = authT;
            InitializeComponent();
            
        }
        

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void AuthView_Init(object? sender, EventArgs e)
        {
            AuthenticationNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("ContentView");

            if (_type != null)
            {
                if(_type == EAuthenticationType.USERNAME)
                    AuthenticationNavigationController.Change(new UsernameAuthentication());
            }

            _type = EAuthenticationType.UNKNOWN;
            
            AuthenticationNavigationController.Change(new AuthSelection());
        }
    }
}
