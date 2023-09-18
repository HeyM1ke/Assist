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
using AssistUser.Lib.Profiles.Models;
using AssistUser.Lib.Reputations.Models;
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

        public static Dictionary<string, AssistReputationUserV2> ReputationUserV2s = new Dictionary<string, AssistReputationUserV2>();
        public static Dictionary<string, AssistProfile> AssistProfiles = new Dictionary<string, AssistProfile>();

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
                await DeterminePage(pres, message);
            };

            await AttemptCurrentPage();
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


        public static async Task GetUserReputations(List<string> ids)
        {
            var resp = await AssistApplication.Current.AssistUser.Reputation.GetUserReputationV2(ids);
            if (resp.Code != 200)
            {
                Log.Error("Failed to Request to get reputation.");
                return;
            }

            var data = JsonSerializer.Deserialize<List<AssistReputationUserV2>>(resp.Data.ToString());

            foreach (var player in data)
            {
                // Try to get the value, if it exists update it.
               ReputationUserV2s.TryGetValue(player.Id, out AssistReputationUserV2? possibleStoredPlayer);

               if (possibleStoredPlayer is null)
               {
                   Log.Information("Player requested is new, adding player to storage.");
                   ReputationUserV2s.TryAdd(player.Id, player);
               }
               else
               {
                   Log.Information("Player requested is old, updating player to storage.");
                   ReputationUserV2s[player.Id] = player;
               }
            }
            
        }
        
        public static async Task GetUserProfiles(List<string> ids)
        {

            foreach (var riotId in ids)
            {
                var resp = await AssistApplication.Current.AssistUser.Profile.GetProfileByRiotId(riotId);
                if (resp.Code != 200)
                {
                    Log.Error("Riot ID requested is not a Valid Profile.");
                    return;
                }
                var data = JsonSerializer.Deserialize<AssistProfile>(resp.Data.ToString());
                
                
                if (AssistProfiles.TryGetValue(riotId, out AssistProfile? possibleStoredPlayer))
                {
                    Log.Information("Player Profile requested is old, updating player to storage.");
                    AssistProfiles[riotId] = data;
                }
                else
                {
                    Log.Information("Player Profile requested is new, adding player to storage.");
                    AssistProfiles.TryAdd(riotId, data);
                }
            }
        }
        
        public static async Task<bool> GetUserProfile(string riotId)
        {
            var resp = await AssistApplication.Current.AssistUser.Profile.GetProfileByRiotId(riotId);
            if (resp.Code != 200)
            {
                Log.Error("Riot ID requested is not a Valid Profile.");
                return false;
            }
            var data = JsonSerializer.Deserialize<AssistProfile>(resp.Data.ToString());
                
                
            if (AssistProfiles.TryGetValue(riotId, out AssistProfile? possibleStoredPlayer))
            {
                Log.Information("Player Profile requested is old, updating player to storage.");
                AssistProfiles[riotId] = data;
            }
            else
            {
                Log.Information("Player Profile requested is new, adding player to storage.");
                AssistProfiles.TryAdd(riotId, data);
            }
            
            return true;
        }
    }
}
