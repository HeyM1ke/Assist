using System.Threading.Tasks;
using Assist.Objects.Enums;
using Assist.Services;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Serilog;
using ValNet;
using ValNet.Enums;
using ValNet.Objects;
using ValNet.Objects.Authentication;
using ValNet.Objects.Exceptions;

namespace Assist.Views.Authentication.Sections
{
    public partial class UsernameAuthentication : UserControl
    {
        private readonly UAVM _viewModel;

        public UsernameAuthentication()
        {
            DataContext = _viewModel = new UAVM();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            AuthenticationNavigationController.Change(new AuthSelection());
        }
        
        private async void LoginBtn_OnClick(object? sender, RoutedEventArgs e)
        {

            var btn = sender as Button;

            btn.IsEnabled = false;
            var uB = this.FindControl<TextBox>("UsernameBox");
            var pB = this.FindControl<TextBox>("PasswordBox");
            var user = uB.Text;
            var pass = pB.Text;

            RiotLogin l = new RiotLogin()
            {
                username = user,
                password = pass
            };

            await _viewModel.LoginWithUsername(l);
            btn.IsEnabled = true;
        }


        private async void SubmitBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var factorBox = this.FindControl<TextBox>("TwoFactorCode");
            btn.IsEnabled = false;

            await _viewModel.LoginWithTwoFactor(factorBox.Text);
            
            btn.IsEnabled = true;
        }

        private void PasswordBox_OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginBtn_OnClick(this.FindControl<Button>("LoginBtn"), null);
            }
        }
    }

    class UAVM : ViewModelBase
    {
        private bool _usernameEnabled = true;

        private string _errorMessage;

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }
        public bool UsernameEnabled
        {
            get => _usernameEnabled;
            set => this.RaiseAndSetIfChanged(ref _usernameEnabled, value);
        }

        private bool _factorEnabled = false;

        public bool FactorEnabled
        {
            get => _factorEnabled;
            set => this.RaiseAndSetIfChanged(ref _factorEnabled, value);
        }

        private RiotUser user;
        public async Task LoginWithUsername(RiotLogin l)
        {
            // Determine Platform (LATER)

            ErrorMessage = string.Empty;
            user = new RiotUserBuilder().WithCredentials(l).WithSettings(new RiotUserSettings(){AuthenticationMethod = AuthenticationMethod.CURL}).Build();
            AuthenticationResult r;
            try
            {
                r = await AuthenticationController.UsernameLogin(user);
            }
            catch (ValNetException ex)
            {
                ErrorMessage = ex.Message;
                Log.Error("Error Message: " + ex.Message);
                if (ex is RequestException e)
                {
                    Log.Error("Status Code: " + e.StatusCode);
                    Log.Error("Content: " + e.Content);
                }

                return;
            }

            if (r.error != null)
            {
                // Handle Error
            }

            if (r.IsAuthenticationSuccessful == false)
            {
                if (r.type != null && r.type.Equals("multifactor"))
                {
                    UsernameEnabled = false;
                    FactorEnabled = true;
                    return;
                }
            }

            await FinishAuthentication(user);
        }

        public async Task LoginWithTwoFactor(string code)
        {
            // Determine Platform (LATER)

            AuthenticationResult r;
            try
            {
                r = await AuthenticationController.SendTwoFactor(user, code);
            }
            catch (ValNetException ex)
            {
                ErrorMessage = ex.Message;
                Log.Error("Error Message: " + ex.Message);
                if (ex is RequestException e)
                {
                    Log.Error("Status Code: " + e.StatusCode);
                    Log.Error("Content: " + e.Content);
                }

                return;
            }

            // Handle Error
            if (r.IsAuthenticationSuccessful == false)
            {
                if (r.error != null && r.type.Equals("multifactor"))
                {
                    Log.Error("Error Message: " + r.error);

                    if(r.error.Equals("multifactor_attempt_failed"))
                        ErrorMessage = "Code is Incorrect.";

                    return;
                }
            }

            await FinishAuthentication(user);
        }

        private async Task FinishAuthentication(RiotUser u)
        {
            ProfileSettings pS = new ProfileSettings();

            pS.SetupProfile(u);
            pS.AuthType = EAuthenticationType.USERNAME;
            await AssistSettings.Current.SaveProfile(pS);

            pS.ConvertCookiesTo64(u.GetAuthClient().ClientCookies);

            AssistApplication.Current.CurrentUser  = u;
            AssistApplication.Current.CurrentProfile = pS;
            AssistApplication.Current.OpenMainView();
        }
    }
}