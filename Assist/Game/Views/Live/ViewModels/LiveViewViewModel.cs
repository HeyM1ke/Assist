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
            AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent += async delegate (PresenceV4Message message)
            {
                Log.Information("Received User Presence Data");
                var pres = await GetPresenceData(message.data.presences[0]);
                Log.Information(pres.ToString());
                await DeterminePage(pres);
            };

        }

        private async Task DeterminePage(PlayerPresence dataMessage)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                switch (dataMessage!.sessionLoopState)
            {
                case "MENUS":
                    LiveViewNavigationController.Change(new MenusPageView());
                    break;
                case "INGAME":
                    LiveViewNavigationController.Change(new IngamePageView());
                    break;
                case "PREGAME":
                    LiveViewNavigationController.Change(new PregamePageView());
                    break;
                default:
                    break;
            }
            });
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
