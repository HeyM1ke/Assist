using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Assist.Game.Services;
using Assist.Services;
using Assist.Services.Popup;
using Assist.Settings;
using Assist.ViewModels;
using Assist.Views.Startup;
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
            if (Design.IsDesignMode)
                return;

            PopupSystem.KillPopups();
            // Start Setup

            while (!IsValorantRunning())
            {
                Message = "Valorant is not Running, Heading Back to the Launcher.";
                await Task.Delay(5000);
                MainWindowContentController.Change(new InitialScreen());
                return;
            }

            // Connect to Valorant Websocket Through Socket Service.

             Message = "Connecting to Game...";
            await ConnectToGame();

            Message = "Connecting to Live Data Socket...";
            await StartSocketConnection();

            new DodgeService();
            
            // Introduce Authentication
            if (string.IsNullOrEmpty(AssistSettings.Current.AssistUserCode))
            {
                AssistApplication.Current.OpenAssistAuthenticationView();
                return;
            }

            Message = "Logging into Assist";
            // Authenticate User with Code
            try
            {
                if (AssistApplication.Current.AssistUser.Account.AccountInfo is null)
                {
                    var authResp = await AssistApplication.Current.AssistUser.Authentication.AuthenticateWithRefreshToken(AssistSettings.Current
                        .AssistUserCode);
                    AssistSettings.Current.AssistUserCode = authResp.RefreshToken;
                    AssistSettings.Save();
                    await AssistApplication.Current.AssistUser.Account.GetUserInfo();
                }
            }
            catch (Exception e)
            {
                Log.Fatal("Account Token is not Valid");
                Log.Fatal("Opening Auth View");
                AssistApplication.Current.OpenAssistAuthenticationView();
                return;
            }

            if (GameSettings.Current.DiscordPresenceEnabled)
            {
                await DiscordPresenceController.ControllerInstance.Initalize();
            }
            
            //Connect to Assist Game Server.
            try
            {
                Message = "Connecting to Assist";
                Log.Information("Attempting To Connect to Game Server");
                await AssistApplication.Current.GameServerConnection.Connect();
            }
            catch (Exception e)
            {
                Log.Fatal("Failed to Connect to Game Server");
                return;
            }
            new LobbyService();
            AssistApplication.Current.OpenGameView();
        }

        private bool IsValorantRunning()
        {
            var processlist = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Where(process => process.Id != Process.GetCurrentProcess().Id).ToList();
            processlist.AddRange(Process.GetProcessesByName("VALORANT-Win64-Shipping"));
            
            return processlist.Any();
        }

        private async Task StartSocketConnection()
        {
            AssistApplication.Current.RiotWebsocketService.RecieveMessageEvent += delegate (object o) {  };
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

                throw e;
            }
            Message = "Connection Successful";

            AssistApplication.Current.CurrentUser = user;

            AssistApplication.Current.RefreshService = new RiotUserTokenRefreshService();


        }
    }
}

