using Assist.Game.Services;
using Assist.Objects.AssistApi.Game;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace Assist.Game.Views.Lobbies.Pages;

public partial class CreateLobbyView : UserControl
{
    private CreateLobbyVm _viewModel;
    private bool _alreadyClicked = false;
    public CreateLobbyView()
    {
        DataContext = _viewModel = new CreateLobbyVm();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void CreateLobby_Click(object? sender, RoutedEventArgs e)
    {
        if (_alreadyClicked)
            return;
        _alreadyClicked = true;
        (sender as Button).IsEnabled = false;
        _viewModel.Message = "";
        var lobbyNameBox = this.FindControl<TextBox>("LobbyNameBox");
        var passwordBox = this.FindControl<TextBox>("PasswordBox");
        var lobbyCodeBox = this.FindControl<TextBox>("LobbyCodeBox");
        var privateLobbyBox = this.FindControl<CheckBox>("PrivateLobbyBox");

        if (string.IsNullOrEmpty(lobbyNameBox.Text)){
            _viewModel.Message = "Lobby Name is Required"; _alreadyClicked = false;
            (sender as Button).IsEnabled = true;
            return;
        }
        
        if (lobbyNameBox.Text.Length < 4)
        {
            _viewModel.Message = "Lobby Name is too short."; _alreadyClicked = false;
            (sender as Button).IsEnabled = true;
            return;
        }
        
        if (passwordBox.Text?.Length < 4)
        {
            _viewModel.Message = "Password is too short.";
            _alreadyClicked = false;
            (sender as Button).IsEnabled = true;
            return;
        }
        
        if (lobbyCodeBox.Text?.Length < 4)
        {
            _viewModel.Message = "Code is too short.";
            _alreadyClicked = false;
            (sender as Button).IsEnabled = true;
            return;
        }

        var d = new CreateLobbyData()
        {
            isPrivate = (bool)privateLobbyBox.IsChecked,
            code = string.IsNullOrEmpty(lobbyCodeBox?.Text) ? "" : lobbyCodeBox?.Text,
            lobbyName = string.IsNullOrEmpty(lobbyNameBox?.Text) ? "" : lobbyNameBox?.Text,
            password = string.IsNullOrEmpty(passwordBox?.Text) ? "" : passwordBox?.Text
        };
        LobbyService.Instance.CreateLobby(d);
        
        _alreadyClicked = false;
        (sender as Button).IsEnabled = true;
    }
}

internal class CreateLobbyVm : ViewModelBase
{
    private string _message;

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }
}