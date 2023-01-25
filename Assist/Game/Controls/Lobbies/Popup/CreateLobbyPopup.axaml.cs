using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Assist.Objects.AssistApi.Game;
using Assist.Services.Popup;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace Assist.Game.Controls.Lobbies.Popup;

public partial class CreateLobbyPopup : UserControl
{
    private readonly CreateLobbyVm _viewModel;

    public CreateLobbyPopup()
    {
        DataContext = _viewModel = new CreateLobbyVm();
        InitializeComponent();
    }
    
    public CreateLobbyPopup(CreateLobbyResp data)
    {
        DataContext = _viewModel = new CreateLobbyVm();
        _viewModel.CreateLobbyData = data;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void CreateLobbyPopup_Init(object? sender, EventArgs e)
    {
        _viewModel.Setup();
    }

    private void BackBtn_Click(object? sender, RoutedEventArgs e)
    {
        PopupSystem.KillPopups();
    }
}

internal class CreateLobbyVm : ViewModelBase
{

    private string _createdMessage;
    public CreateLobbyResp CreateLobbyData;

    public string CreatedMessage
    {
        get => _createdMessage;
        set => this.RaiseAndSetIfChanged(ref _createdMessage, value);
    }
    
    private string _yourLobbyCode;

    public string YourLobbyCode
    {
        get => _yourLobbyCode;
        set => this.RaiseAndSetIfChanged(ref _yourLobbyCode, value);
    }
    
    private string _yourLobbyPassword;

    public string YourLobbyPassword
    {
        get => _yourLobbyPassword;
        set => this.RaiseAndSetIfChanged(ref _yourLobbyPassword, value);
    }

    private bool _lobbySuccess = false;

    public bool LobbySuccessful
    {
        get => _lobbySuccess;
        set => this.RaiseAndSetIfChanged(ref _lobbySuccess, value);
    }
    
    private string _lobbyMessage;

    public string LobbyMessage
    {
        get => _lobbyMessage;
        set => this.RaiseAndSetIfChanged(ref _lobbyMessage, value);
    }

    public async Task Setup()
    {
        if (!CreateLobbyData.IsSuccessful)
        {
            CreatedMessage = "FAILED";
            LobbySuccessful = CreateLobbyData.IsSuccessful;
            LobbyMessage = CreateLobbyData.Message;
            return;
        }
        CreatedMessage = "CREATED";
        LobbySuccessful = CreateLobbyData.IsSuccessful;

        YourLobbyCode = "Your Lobby Code is: " + CreateLobbyData.Lobby.Code;
        YourLobbyPassword = "This Lobby does required the password you wanted.";

    }
}