using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Models.Enums;
using Assist.Models.Game;
using Assist.Models.Socket;
using Assist.Services.Modules;
using Assist.Views.Game.Live.Pages;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using ValNet.Objects.Local;

namespace Assist.ViewModels.Game;

public partial class LiveViewViewModel : ViewModelBase
{
    [ObservableProperty] private static ELivePage _currentPage = ELivePage.UNKNOWN;

    [ObservableProperty] private UserControl _currentView = new UserControl();
    
    public static Dictionary<string, ValorantPlayerStorage> ValorantPlayers = new Dictionary<string, ValorantPlayerStorage>();
    
    public async Task Setup()
    {
        Log.Information("Setting up LiveView Page");
        ChangePage(new UnknownPageView());
    }

    private async void RiotWebsocketServiceOnUserPresenceMessageEvent(PresenceV4Message message)
    {
        Log.Information("Received User Presence Data");
        var pres = await GetPresenceData(message.MessageData.Presences[0]);
        await DeterminePage(pres, message);
    }


    public async Task AttemptCurrentPage()
    {
        AssistApplication.RiotWebsocketService.UserPresenceMessageEvent += RiotWebsocketServiceOnUserPresenceMessageEvent; 
        var pres = await AssistApplication.ActiveUser.Presence.GetPresences();
        var user = pres.presences.Find(p => p.puuid == AssistApplication.ActiveUser.UserData.sub);
        var data = await GetPresenceData(user);
        await DeterminePage(data, user);
    }

    private async Task DeterminePage(PlayerPresence dataMessage, PresenceV4Message fullMessage = null)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            switch (dataMessage!.sessionLoopState)
            {
                case "MENUS":
                    if (CurrentPage != ELivePage.MENUS)
                    {
                        ChangePage(new MenusPageView(fullMessage));
                        CurrentPage = ELivePage.MENUS;
                    };
                    break;
                case "INGAME":
                    if (CurrentPage != ELivePage.INGAME) {
                        ChangePage(new IngamePageView());
                        CurrentPage = ELivePage.INGAME;
                    };
                    break;
                case "PREGAME":
                    if (CurrentPage != ELivePage.PREGAME) {
                        ChangePage(new PregamePageView());
                        CurrentPage = ELivePage.PREGAME;
                    };
                    break;
                default:
                    Log.Information("Unknown Session Loop State: " + dataMessage.sessionLoopState);
                    break;
            }
        });
    }

    public async Task<PlayerPresence> GetPresenceData(ChatV4PresenceObj.Presence data)
    {
        if (data is null)
            return new PlayerPresence();
        
        if (string.IsNullOrEmpty(data.Private))
            return new PlayerPresence();
        var stringData = Convert.FromBase64String(data.Private);
        var decodedString = Encoding.UTF8.GetString(stringData);
        return JsonSerializer.Deserialize<PlayerPresence>(decodedString);
    }

    private async Task DeterminePage(PlayerPresence dataMessage, ChatV4PresenceObj.Presence fullMessage = null)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            switch (dataMessage!.sessionLoopState)
            {
                case "MENUS":
                    if (CurrentPage != ELivePage.MENUS)
                    {
                        ChangePage(new MenusPageView(fullMessage));
                        CurrentPage = ELivePage.MENUS;
                    };
                    break;
                case "INGAME":
                    if (CurrentPage != ELivePage.INGAME) {
                        ChangePage(new IngamePageView());
                        CurrentPage = ELivePage.INGAME;
                    };
                    break;
                case "PREGAME":
                    if (CurrentPage != ELivePage.PREGAME) {
                        ChangePage(new PregamePageView());
                        CurrentPage = ELivePage.PREGAME;
                    };
                    break;
                default:
                    Log.Information("Unknown Session Loop State: " + dataMessage.sessionLoopState);
                    break;
            }
        });
    }
    
    public void Unsubscribe(){
         AssistApplication.RiotWebsocketService.UserPresenceMessageEvent -= RiotWebsocketServiceOnUserPresenceMessageEvent; 
    }

    private void ChangePage(UserControl newPageView)
    {
        CurrentView = newPageView;
    }
}