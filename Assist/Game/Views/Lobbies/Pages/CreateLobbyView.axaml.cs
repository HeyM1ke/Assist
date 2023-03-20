using System.Threading.Tasks;
using Assist.Game.Services;
using Assist.Objects.AssistApi.Game;
using Assist.ViewModels;
using AssistUser.Lib.Lobbies.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Serilog;

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

    private async void CreateLobby_Click(object? sender, RoutedEventArgs e)
    {
        if (_alreadyClicked)
            return;
        _alreadyClicked = true;
        _viewModel.IsEnabled = false;
        
        _viewModel.Message = "";
        var lobbyNameBox = this.FindControl<TextBox>("LobbyNameBox");
        var passwordBox = this.FindControl<TextBox>("PasswordBox");
        var lobbyCodeBox = this.FindControl<TextBox>("LobbyCodeBox");
        var privateLobbyBox = this.FindControl<CheckBox>("PrivateLobbyBox");

        if (string.IsNullOrEmpty(lobbyNameBox.Text)){
            _viewModel.Message = "Lobby Name is Required"; _alreadyClicked = false;
            _viewModel.IsEnabled = true;
            return;
        }
        
        if (!string.IsNullOrEmpty(lobbyNameBox.Text) &&lobbyNameBox.Text.Length < 4)
        {
            _viewModel.Message = "Lobby Name is too short."; _alreadyClicked = false;
            lobbyNameBox.Text = string.Empty;
            _viewModel.IsEnabled = true;
            return;
        }
        
        if ( !string.IsNullOrEmpty(passwordBox.Text) && passwordBox.Text?.Length < 4)
        {
            _viewModel.Message = "Password is too short.";
            passwordBox.Text = string.Empty;
            _alreadyClicked = false;
            _viewModel.IsEnabled = true;
            return;
        }
        
        if (!string.IsNullOrEmpty(lobbyCodeBox.Text) &&lobbyCodeBox.Text?.Length < 4)
        {
            _viewModel.Message = "Code is too short.";
            lobbyCodeBox.Text = string.Empty;
            _alreadyClicked = false;
            _viewModel.IsEnabled = true;
            return;
        }

        var d = new CreateLobbyData()
        {
            isPrivate = (bool)privateLobbyBox.IsChecked,
            code = string.IsNullOrEmpty(lobbyCodeBox?.Text) ? "" : lobbyCodeBox?.Text,
            lobbyName = string.IsNullOrEmpty(lobbyNameBox?.Text) ? "" : lobbyNameBox?.Text,
            password = string.IsNullOrEmpty(passwordBox?.Text) ? "" : passwordBox?.Text
        };

        await _viewModel.CreateLobby(d);
        _alreadyClicked = false;
        _viewModel.IsEnabled = false;
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
    
    private bool _isEnabled = true;

    public bool IsEnabled
    {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }

    public async Task CreateLobby(CreateLobbyData d )
    {
        Log.Information("Attempting to Create Lobby.");
        LobbyService.Instance.CreateLobby(d);
    }
}