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
        try
        {
            AssistApplication.RiotWebsocketService.UserPresenceMessageEvent += RiotWebsocketServiceOnUserPresenceMessageEvent;
            if (AssistApplication.ActiveUser != null)
            {
                Log.Information("AttemptCurrentPage: Getting Presences");
                var pres = await AssistApplication.ActiveUser.Presence.GetPresences();
                Log.Information($"AttemptCurrentPage: Got Pres? {pres is not null}");
                var user = pres.presences.Find(p => p.puuid == AssistApplication.ActiveUser.UserData.sub);
                Log.Information($"AttemptCurrentPage: Got User? {user is not null}");
                PlayerPresence data = null;
                if (user != null)
                {
                    Log.Information($"AttemptCurrentPage: user != null, Getting Data");
                    data = await GetPresenceData(user);
                    Log.Information($"AttemptCurrentPage: Got Data");
                }
                await DeterminePage(data, user);
            }
        }
        catch (Exception e)
        {
            Log.Error("Error Attempting to find Current Page.");
            Log.Error(e.Message);
            Log.Error(e.StackTrace);
            Log.Error(e.Source);
        }
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
        Log.Information($"GetPresenceData: Checking if Data is Null {data is null}");
        if (data is null)
            return new PlayerPresence();
        
        Log.Information($"GetPresenceData: Checking string.IsNullOrEmpty(data.Private) {string.IsNullOrEmpty(data.Private)}");
        if (string.IsNullOrEmpty(data.Private))
            return new PlayerPresence();
        Log.Information($"GetPresenceData: Converting Data");
        var stringData = Convert.FromBase64String(data.Private);
        var decodedString = Encoding.UTF8.GetString(stringData);
        Log.Information($"GetPresenceData: Deserializing");
        return JsonSerializer.Deserialize<PlayerPresence>(decodedString);
    }

    private async Task DeterminePage(PlayerPresence? dataMessage, ChatV4PresenceObj.Presence fullMessage = null)
    {
        Log.Information($"Determining Page");
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            Log.Information($"Determining Page: is datamessage null? {dataMessage is null}");
            if (dataMessage != null)
            {
                switch (dataMessage!.sessionLoopState)
                {
                    case "MENUS":
                        if (CurrentPage != ELivePage.MENUS)
                        {
                            Log.Information("Changing to Menus");
                            ChangePage(new MenusPageView(fullMessage));
                            CurrentPage = ELivePage.MENUS;
                        };
                        break;
                    case "INGAME":
                        if (CurrentPage != ELivePage.INGAME) {
                            Log.Information("Changing to Ingame");
                            ChangePage(new IngamePageView());
                            CurrentPage = ELivePage.INGAME;
                        };
                        break;
                    case "PREGAME":
                        if (CurrentPage != ELivePage.PREGAME) {
                            Log.Information("Changing to Pregame");
                            ChangePage(new PregamePageView());
                            CurrentPage = ELivePage.PREGAME;
                        };
                        break;
                    default:
                        Log.Information("Unknown Session Loop State: " + dataMessage.sessionLoopState);
                        break;
                }
            }

            if (dataMessage is null)
            {
                Log.Information("USING BACKUP ON LIVEVIEWVIEWMODELPAGE");
                try
                {
                    var test1 = await AssistApplication.ActiveUser.Pregame.GetPlayer();
                    if (!string.IsNullOrEmpty(test1.MatchID))
                    {
                        if (CurrentPage != ELivePage.PREGAME) {
                            Log.Information("Changing to Pregame");
                            ChangePage(new PregamePageView());
                            CurrentPage = ELivePage.PREGAME;
                            return;
                        };
                    }
                }
                catch (Exception e)
                {
                    Log.Information("USING BACKUP ON LIVEVIEWVIEWMODELPAGE: Not Pregame");
                }
                
                try
                {
                    var test1 = await AssistApplication.ActiveUser.CoreGame.FetchPlayer();
                    if (!string.IsNullOrEmpty(test1.MatchID))
                    {
                        if (CurrentPage != ELivePage.INGAME) {
                            Log.Information("Changing to Ingame");
                            ChangePage(new IngamePageView());
                            CurrentPage = ELivePage.INGAME;
                            return;
                        };
                    }
                }
                catch (Exception e)
                {
                    Log.Information("USING BACKUP ON LIVEVIEWVIEWMODELPAGE: Not Coregame");
                }
                
                try
                {
                    var test1 = await AssistApplication.ActiveUser.Party.FetchParty();
                    if (!string.IsNullOrEmpty(test1.ID))
                    {
                        if (CurrentPage != ELivePage.MENUS)
                        {
                            Log.Information("Changing to Menus");
                            ChangePage(new MenusPageView(fullMessage));
                            CurrentPage = ELivePage.MENUS;
                            return;
                        };
                    }
                }
                catch (Exception e)
                {
                    Log.Information("USING BACKUP ON LIVEVIEWVIEWMODELPAGE: Not Party");
                }
            }
        });
        
        switch (dataMessage?.sessionLoopState)
        {
            case "MENUS":
                if (CurrentPage != ELivePage.MENUS)
                {
                    Log.Information("Changing to Menus");
                    ChangePage(new MenusPageView(fullMessage));
                    CurrentPage = ELivePage.MENUS;
                };
                break;
            case "INGAME":
                if (CurrentPage != ELivePage.INGAME) {
                    Log.Information("Changing to Ingame");
                    ChangePage(new IngamePageView());
                    CurrentPage = ELivePage.INGAME;
                };
                break;
            case "PREGAME":
                if (CurrentPage != ELivePage.PREGAME) {
                    Log.Information("Changing to Pregame");
                    ChangePage(new PregamePageView());
                    CurrentPage = ELivePage.PREGAME;
                };
                break;
            default:
                Log.Information("Unknown Session Loop State: " + dataMessage?.sessionLoopState);
                break;
        }
    }
    
    public void Unsubscribe(){
         AssistApplication.RiotWebsocketService.UserPresenceMessageEvent -= RiotWebsocketServiceOnUserPresenceMessageEvent; 
    }

    public void ChangePage(UserControl newPageView)
    {
        CurrentView = newPageView;
    }
}