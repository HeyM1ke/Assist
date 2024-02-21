using System;
using Assist.Game.Controls.Global;
using Assist.Game.Services;
using Assist.Objects.AssistApi.Game;
using Assist.Services.Popup;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Serilog;

namespace Assist.Game.Controls.Lobbies;

public partial class LobbyJoinButton : UserControl
{
    
    public static readonly StyledProperty<string> AssistLobbyIdProperty = AvaloniaProperty.Register<LobbyJoinButton, string>(nameof(AssistLobbyId), defaultValue: "0-0-0-0");

    public string AssistLobbyId
    {
        get { return GetValue(AssistLobbyIdProperty); }
        set { SetValue(AssistLobbyIdProperty, value); }
    }
    public LobbyJoinButton()
    {
        InitializeComponent();
        AddHandler(PointerPressedEvent, PointerPressed_Event);
    }

    private async void PointerPressed_Event(object? sender, PointerPressedEventArgs e)
    {
        Log.Information("Pointer Pressed on LobbyJoinBtn, Attempting ID of: " + AssistLobbyId);
        (sender as LobbyJoinButton).IsEnabled = false;
        
        var joinPtyResp = await AssistApplication.Current.AssistUser.Lobbies.JoinLobbyByCode(AssistLobbyId);
        if (!joinPtyResp.IsSuccessful)
        {
            Log.Error(joinPtyResp.Message);
            return;
        }
        
        // Send Join Message to Lobbies Service to handle Valorant side.
        if (joinPtyResp.PartyClosed)
        {
            // This means the VALORANT party is closed on VALORANT. Needs to request invite.
            PopupSystem.SpawnCustomPopup(new LoadingPopup());
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
            PopupSystem.KillPopups();
        }
        else
        {
            // Party is Open
            try
            {
                Log.Information("Attempting to Join Party of id " + joinPtyResp.PartyId);
                PopupSystem.SpawnCustomPopup(new LoadingPopup());
                LobbyService.Instance.CurrentLobbyOwner = false;
                AssistApplication.Current.CurrentUser.Party.JoinParty(joinPtyResp.PartyId);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to Join party from base Lobbie.");
                Log.Error("Failed to Join party from base Lobbie. MESSAGE " + ex.Message);
                Log.Error("Failed to Join party from base Lobbie. STACK " + ex.StackTrace);
                PopupSystem.KillPopups();
                (sender as Button).IsEnabled = true;
                return;
            }
        }
        
        LobbyService.Instance.CurrentLobbyOwner = false;
    }

}