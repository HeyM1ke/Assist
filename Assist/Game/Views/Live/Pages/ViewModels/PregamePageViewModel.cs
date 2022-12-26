using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Game.Controls.Live;
using Assist.Game.Services;
using Assist.Objects.Helpers;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using Serilog;
using ValNet.Objects.Exceptions;
using ValNet.Objects.Local;
using ValNet.Objects.Pregame;

namespace Assist.Game.Views.Live.Pages.ViewModels
{
    internal class PregamePageViewModel : ViewModelBase
    {
        // Control Container for User Controls
        private List<GameUserControl> _userControls = new List<GameUserControl>();

        public List<GameUserControl> UserControls
        {
            get => _userControls;
            set => this.RaiseAndSetIfChanged(ref _userControls, value);
        }

        List<string> SelectedAgentIds = new List<string>();
        private string? MatchId;

        private string? _errorMessage;

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        private string? _mapImage = "https://content.assistapp.dev/maps/Ascent_BWlistview.png";

        public string? MapImage
        {
            get => _mapImage;
            set => this.RaiseAndSetIfChanged(ref _mapImage, value);
        }

        private string? _mapName = "FRACTURE";

        public string? MapName
        {
            get => _mapName;
            set => this.RaiseAndSetIfChanged(ref _mapName, value);
        }


        public async Task Setup()
        {
            // Get the current match ID for the game. 
            try
            {
                var getPlayerResp = await AssistApplication.Current.CurrentUser.Pregame.GetPlayer();
                if (getPlayerResp != null)
                {
                    MatchId = getPlayerResp.MatchID;
                }

                if (string.IsNullOrEmpty(MatchId))
                {
                    ErrorMessage = "Failed to Get MatchID";
                    return;
                }
            }
            catch (RequestException e)
            {
                Log.Fatal("Error on getting player pregame");
                Log.Fatal("PREGAME ERROR: " + e.StatusCode);
                Log.Fatal("PREGAME ERROR: " + e.Content);
                Log.Fatal("PREGAME ERROR: " + e.Message);
            }


            // First Subscribe to Updates from the PREGAME Api on Websocket. To Update the Data for whenever there is an UPDATE.
            await UpdateData(); // Do inital Pregame Check

            AssistApplication.Current.RiotWebsocketService.PregameMessageEvent += async o =>
            {
                // On message recieved Check if it is a PREGAME Message.
                await UpdateData();
            };
        }

        public async Task UpdateData()
        {
            PregameMatch MatchResp = new PregameMatch();
            ChatV4PresenceObj PresenceResp = new ChatV4PresenceObj();
            try
            {
                MatchResp = await AssistApplication.Current.CurrentUser.Pregame.GetMatch(MatchId!);
                PresenceResp = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
                if (MatchResp == null || PresenceResp == null)
                {
                    ErrorMessage = "Failed on getting update data. Match or Presence";
                    return;
                }
            }
            catch (RequestException e)
            {
                Log.Fatal("Error on getting player match or Pres");
                Log.Fatal("PREGAME ERROR: " + e.StatusCode);
                Log.Fatal("PREGAME ERROR: " + e.Content);
                Log.Fatal("PREGAME ERROR: " + e.Message);
            }

            if(MatchResp.AllyTeam == null)
                return;

            // Now that we have the match data lets go through it. 
            // First check if this is our first time around.
            if (UserControls.Count == 0) // Checking if the List is empty to signify if this is our first time around
            {
                foreach (var allyPlayer in MatchResp.AllyTeam.Players)
                {
                    await AddUserToList(allyPlayer);
                }
            }
            else
            {
                // For every other time around, Update the Players that already excist.
                foreach (var allyPlayer in MatchResp.AllyTeam.Players)
                {
                    await UpdateUserInList(allyPlayer);
                }
            }

            // Update Map Data
            HandleMapData(MatchResp);
        }

        private async Task AddUserToList(PregameMatch.Player Player)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var temp = new List<GameUserControl>();
                temp.Add(new GameUserControl(Player));
                UserControls = UserControls.Concat(temp).ToList();
            });
        }

        private async Task UpdateUserInList(PregameMatch.Player Player)
        {
            var control = UserControls.Find(control => control.PlayerId == Player.Subject);

            if (control != null)
            {
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await control.UpdatePlayer(Player);
                });

                return;
            }

            Log.Fatal("Tried Updating Control for User that does not exist?");
            Log.Fatal("Adding User to list.");
            await AddUserToList(Player);

        }

        private async Task HandleMapData(PregameMatch Match)
        {
            if (Match.MapID != null)
            {
                MapName = MapNames.MapsByPath?[Match.MapID].ToUpper();
                MapImage =
                    $"https://content.assistapp.dev/maps/{MapNames.MapsByPath?[Match.MapID]}_BWlistview.png";
            }
        }


    }
}
