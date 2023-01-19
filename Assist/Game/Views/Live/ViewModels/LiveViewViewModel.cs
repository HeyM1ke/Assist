using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Models;
using Assist.Game.Services;
using Assist.Game.Views.Live.Pages;
using Assist.Objects.RiotSocket;
using Assist.ViewModels;
using Avalonia.Threading;
using ReactiveUI;
using Serilog;
using ValNet.Objects.Local;

namespace Assist.Game.Views.Live.ViewModels
{
    internal class LiveViewViewModel : ViewModelBase
    {

        private string _output = "START: ";

        public string Output
        {
            get => _output;
            set => this.RaiseAndSetIfChanged(ref _output, value);
        }

        public async void DisplayWebsocketData()
        {
            AssistApplication.Current.RiotWebsocketService.RecieveMessageEvent += delegate(object o)
            {
                Output += $"\nGot Basic Message to Socket";
            };

            AssistApplication.Current.RiotWebsocketService.PresenceMessageEvent += delegate(PresenceV4Message message)
            {
                {
                    Output += $"\nGot Basic Pres Message to Socket";
                }
                ;
            };

            AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent += delegate (PresenceV4Message message)
            {
                {
                    Output += $"\nGot Current User Pres Message to Socket";
                }
                ;
            };
        }

        public async Task Setup()
        {
            LiveViewNavigationController.Change(new UnkownPageView());

            //await AttemptCurrentPage();

            AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent += async delegate (PresenceV4Message message)
            {
                Log.Information("Received User Presence Data");
                var pres = await GetPresenceData(message.data.presences[0]);
                await DeterminePage(pres, message);
            };

        }

        private async Task AttemptCurrentPage()
        {
            var pres = await AssistApplication.Current.CurrentUser.Presence.GetPresences();

            var user = pres.presences.Find(p => p.puuid == AssistApplication.Current.CurrentUser.UserData.sub);
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
                    if (LiveViewNavigationController.CurrentPage != LivePage.MENUS)
                    {
                        LiveViewNavigationController.Change(new MenusPageView(fullMessage));
                    }
                    break;
                case "INGAME":
                    if (LiveViewNavigationController.CurrentPage != LivePage.INGAME)
                    {
                        LiveViewNavigationController.Change(new IngamePageView());
                    }
                        break;
                case "PREGAME":
                    if (LiveViewNavigationController.CurrentPage != LivePage.PREGAME)
                    {
                        LiveViewNavigationController.Change(new PregamePageView());
                    }
                    break;
                default:
                    break;
            }
            });
        }

        public async Task<PlayerPresence> GetPresenceData(ChatV4PresenceObj.Presence data)
        {
            if(string.IsNullOrEmpty(data.Private))
                return new PlayerPresence();
            byte[] stringData = Convert.FromBase64String(data.Private);
            string decodedString = Encoding.UTF8.GetString(stringData);
            return JsonSerializer.Deserialize<PlayerPresence>(decodedString);
        }

        private async Task DeterminePage(PlayerPresence dataMessage, ChatV4PresenceObj.Presence fullMessage = null)
        {
            /*Dispatcher.UIThread.InvokeAsync(() =>
            {
                switch (dataMessage!.sessionLoopState)
            {
                case "MENUS":
                    if (LiveViewNavigationController.CurrentPage != LivePage.MENUS)
                    {
                        LiveViewNavigationController.Change(new MenusPageView(fullMessage));
                    }
                    break;
                case "INGAME":
                    if (LiveViewNavigationController.CurrentPage != LivePage.INGAME)
                    {
                        LiveViewNavigationController.Change(new IngamePageView());
                    }
                        break;
                case "PREGAME":
                    if (LiveViewNavigationController.CurrentPage != LivePage.PREGAME)
                    {
                        LiveViewNavigationController.Change(new PregamePageView());
                    }
                    break;
                default:
                    break;
            }
            });*/
        }

        public async Task<PlayerPresence> GetPresenceData(PresenceV4Message.Presence data)
        {
            if(string.IsNullOrEmpty(data.Private))
                return new PlayerPresence();
            byte[] stringData = Convert.FromBase64String(data.Private);
            string decodedString = Encoding.UTF8.GetString(stringData);
            return JsonSerializer.Deserialize<PlayerPresence>(decodedString);
        }
    }
}
