using System;
using System.Threading.Tasks;
using Assist.Game.Services;
using Assist.ViewModels;
using Avalonia.Controls;
using ReactiveUI;
using Serilog;
using ValNet;
using ValNet.Enums;
using ValNet.Objects;
using ValNet.Objects.Exceptions;

namespace Assist.Game.Views.Initial.ViewModels
{
    internal class GameInitialViewViewModel : ViewModelBase
    {

        private string _message = "Loading";

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public async Task Setup()
        {
            // Start Setup

            // Check if you are in Design Mode
            if(Design.IsDesignMode)
                return;

            // Connect to Valorant Websocket Through Socket Service.
            Message = "Connecting to Game...";
            await ConnectToGame();
            Message = "Connecting to Live Data Socket...";
            await StartSocketConnection();

            new DodgeService();

            AssistApplication.Current.OpenGameView();
        }

        private async Task StartSocketConnection()
        {
            AssistApplication.Current.RiotWebsocketService.RecieveMessageEvent += delegate (object o) { Log.Information(o.ToString()); };
            await AssistApplication.Current.RiotWebsocketService.Connect();
            Message = "Connected to Live Data Socket.";
            
        }

        private async Task ConnectToGame()
        {
            RiotUser user = new RiotUserBuilder().WithSettings(new RiotUserSettings()
            {
                AuthenticationMethod = AuthenticationMethod.CURL,
            }).Build();

            try
            {
                var r = await user.Authentication.AuthenticateWithLocal();
            }
            catch (Exception e)
            {
                Log.Error("Error On Local Auth Connection");
                if (e is ValNetException)
                {
                    var ex = e as ValNetException;
                    Log.Fatal("ERROR:" + ex.Message);
                }

            }
            Message = "Connection Successful";

            AssistApplication.Current.CurrentUser = user;

            


        }
    }
}
