using DiscordRPC;
using DiscordRPC.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Objects.RiotSocket;
using Assist.ViewModels;
using CliWrap;
using Serilog;
using Assist.Game.Models;

namespace Assist.Game.Services
{
    internal class DiscordPresenceController
    {
        public static DiscordPresenceController ControllerInstance = new DiscordPresenceController();
        private const string APPID = "925134832453943336";

        public DiscordRpcClient Client;
        private RichPresence _currentPresence;
        public bool BDiscordPresenceActive;
        public DateTime timeStart;

        public static Button[] clientButtons =
        {
            new Button() { Label = "Download Assist", Url = "https://github.com/HeyM1ke/Assist/" }
        };

        public DiscordPresenceController()
        {
            Client = new DiscordRpcClient(APPID);

        }

        public async Task Initalize()
        {
            Client.OnReady += delegate(object sender, ReadyMessage args)
            {
                Log.Information("Discord Presence Client Ready, User: " + args.User.Username);
                timeStart = DateTime.Now;
            };
            Client.OnConnectionFailed += delegate(object sender, ConnectionFailedMessage args)
            {
                Log.Information("Discord Presence Client Failed to Connect to Discord, failed pipe number: " +
                                args.FailedPipe);
                return;
            };
            _currentPresence = new RichPresence
            {
                Buttons = clientButtons,
                Assets = new Assets()
                {
                    LargeImageKey = "default",
                    LargeImageText = "Powered By Assist"
                },
                Details = "Chilling",
                Party = new Party()
                {
                    Max = 5,
                    Size = 1
                },
                Secrets = null,
                State = "VALORANT",
            };

            Client.SetPresence(_currentPresence);

            try
            {
                Client.Initialize();
                BDiscordPresenceActive = true;
            }
            catch (Exception e)
            {
                Log.Error("Unhandled Ex Source: " + e.Source);
                Log.Error("Unhandled Ex StackTrace: " + e.StackTrace);
                Log.Error("Unhandled Ex Message: " + e.Message);
            }

            AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent += UpdateDiscordRpcWithDataFromPresence;
        }

        private async void UpdateDiscordRpcWithDataFromPresence(PresenceV4Message obj)
        {
            // Decode Pres
            var pres = await GetPresenceData(obj.data.presences[0]);

        }

        public async Task UpdatePresence(RichPresence data)
        {
            _currentPresence = data;
            // Send Data to Discord
            Client.SetPresence(_currentPresence);
            Client.Invoke();
        }

        public async Task Deinitalize()
        {
            try
            {
                Client.Deinitialize();
                BDiscordPresenceActive = false;
            }
            catch (Exception e)
            {
                Log.Error("Unhandled Ex Source: " + e.Source);
                Log.Error("Unhandled Ex StackTrace: " + e.StackTrace);
                Log.Error("Unhandled Ex Message: " + e.Message);
            }


        }

        public async Task Shutdown()
        {
            if (!Client.IsDisposed)
                Client.Dispose();

            if (Client.IsInitialized)
                Client.Deinitialize();

            Client = new DiscordRpcClient(APPID);
            BDiscordPresenceActive = false;
        }


        private async Task<PlayerPresence> GetPresenceData(PresenceV4Message.Presence data)
        {
            if (string.IsNullOrEmpty(data.Private))
                return new PlayerPresence();
            byte[] stringData = Convert.FromBase64String(data.Private);
            string decodedString = Encoding.UTF8.GetString(stringData);
            return JsonSerializer.Deserialize<PlayerPresence>(decodedString);
        }
    }
}
