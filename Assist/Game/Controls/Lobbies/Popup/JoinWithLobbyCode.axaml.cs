using Assist.Services.Popup;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Serilog;

namespace Assist.Game.Controls.Lobbies.Popup;

public partial class JoinWithLobbyCode : UserControl
{
    private readonly JoinLobbyCodePopupVM _viewModel;

    public JoinWithLobbyCode()
    {
        DataContext = _viewModel = new JoinLobbyCodePopupVM();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void JoinLobby_Click(object? sender, RoutedEventArgs e)
    {
        (sender as Button).IsEnabled = false;

        var passBox = this.FindControl<TextBox>("LobbyPasswordBox");
        var codeBox = this.FindControl<TextBox>("LobbyCodeBox");
        
        Log.Information("Attempting to Join Party with Code of");
        
        // Attempt to join party
        var joinPtyResp = await AssistApplication.Current.AssistUser.JoinLobbyByCode(codeBox.Text, passBox.Text);

        if (!joinPtyResp.IsSuccessful)
        {
            _viewModel.Message = joinPtyResp.Message;
            return;
        }
        
        // Send Join Message to Lobbies Service to handle Valorant side.
        
        
    }

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        PopupSystem.KillPopups();
    }
}

internal class JoinLobbyCodePopupVM : ViewModelBase
{
    private string _message;

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }
}