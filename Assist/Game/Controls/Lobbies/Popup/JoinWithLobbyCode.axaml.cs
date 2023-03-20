using System;
using Assist.Game.Services;
using Assist.Objects.AssistApi.Game;
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
        var joinPtyResp = await AssistApplication.Current.AssistUser.Lobbies.JoinLobbyByCode(codeBox.Text, passBox.Text);

        if (!joinPtyResp.IsSuccessful)
        {
            _viewModel.Message = joinPtyResp.Message;
            (sender as Button).IsEnabled = true;
            return;
        }
        
        // Send Join Message to Lobbies Service to handle Valorant side.
        if (joinPtyResp.PartyClosed)
        {
            // This means the VALORANT party is closed on VALORANT. Needs to request invite.

            var pres = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
            var p = pres.presences.Find(pres => pres.puuid == AssistApplication.Current.CurrentUser.UserData.sub);
            // Get CurrentUsername
            RequestPartyJoin data = new RequestPartyJoin()
            {
                CurrentGameName = p.game_name,
                CurrentTag = p.game_tag,
                IsPrivate = true,
                PartyId = joinPtyResp.PartyId
            };
            LobbyService.Instance.CurrentLobbyOwner = false;
            await LobbyService.Instance.RequestPartyJoin(data);
        }
        else
        {
            // Party is Open
            try
            {
                
                Log.Information("Attempting to Join Party of id " + joinPtyResp.PartyId);
                LobbyService.Instance.CurrentLobbyOwner = false;
                AssistApplication.Current.CurrentUser.Party.JoinParty(joinPtyResp.PartyId);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to Join party from base Lobbie.");
                Log.Error("Failed to Join party from base Lobbie. MESSAGE " + ex.Message);
                Log.Error("Failed to Join party from base Lobbie. STACK " + ex.StackTrace);
                return;
            }
        }
        
        LobbyService.Instance.CurrentLobbyOwner = false;
        PopupSystem.KillPopups();// close popup
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