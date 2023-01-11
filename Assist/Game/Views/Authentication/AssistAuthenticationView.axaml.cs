using Assist.Game.Views.Authentication.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Assist.Game.Views.Authentication
{
    public partial class AssistAuthenticationView : UserControl
    {
        private readonly AssistAuthenticationViewViewModel _viewModel;

        public AssistAuthenticationView()
        {
            DataContext = _viewModel = new AssistAuthenticationViewViewModel();
            InitializeComponent();
        }

        private async void DiscordButton_Click(object? sender, RoutedEventArgs e)
        {
            await _viewModel.DiscordAuth();
        }

        private async void ConfirmUsername_Click(object? sender, RoutedEventArgs e)
        {
            var textBox = this.FindControl<TextBox>("UsernameBox");


            if (textBox.Text.Length >= 4)
                await _viewModel.SetUsername(textBox.Text);
            else
            {
                _viewModel.ErrorMessage = "Username is not Long Enough.";
            }
        }
    }
}
