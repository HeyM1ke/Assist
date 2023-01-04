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
using Assist.Game.Views.Live.Pages;
using Assist.Objects.Helpers;
using Assist.Settings;
using YamlDotNet.Core;

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

            RichPresence newPresence = new RichPresence()
            {
                Buttons = clientButtons
            };

            switch (pres.sessionLoopState)
            {
                case "MENUS":
                    DetermineMenusPresence(newPresence, pres);
                    break;
                case "INGAME":
                    DetermineIngamePresence(newPresence, pres);
                    break;
                case "PREGAME":
                    DeterminePregamePresence(newPresence, pres);
                    break;
                default:
                    break;
            }


        }

        private async void DetermineIngamePresence(RichPresence rpObj, PlayerPresence playerPres)
        {
            string details = string.Empty;
            ulong timeStart = 0;

            DiscordRPC.Party.PrivacySetting privacy;

            if (playerPres.partyAccessibility.Equals("CLOSED"))
            {
                privacy = DiscordRPC.Party.PrivacySetting.Private;
            }else
            {
                privacy = DiscordRPC.Party.PrivacySetting.Public;
            }

            if (GameSettings.Current.RichPresenceSettings.ShowRank && string.Equals(GameSettings.Current.RichPresenceSettings.DetailsTextData, "Rank")) // Show Rank as Details
            {
                details = $"{CompetitiveNames.RankNames[playerPres.competitiveTier]}";
            }

            if (!string.Equals(GameSettings.Current.RichPresenceSettings.DetailsTextData, "Rank") || !GameSettings.Current.RichPresenceSettings.ShowRank) // Fall back to anything as this is the only other option.
            {
                if (GameSettings.Current.RichPresenceSettings.ShowMode)
                {
                    if (playerPres.partyState.Contains("CUSTOM_GAME"))
                        details += "Custom Game";
                    else
                    {
                        var queue = await DetermineQueueKey(playerPres);
                        details = $"{queue}";
                    }
                }

                if (GameSettings.Current.RichPresenceSettings.ShowScore)
                {
                    if (details != string.Empty)
                        details += " || ";

                    details +=
                        $"{playerPres.partyOwnerMatchScoreAllyTeam} - {playerPres.partyOwnerMatchScoreEnemyTeam}";
                }
            }

            rpObj.Assets = new Assets
            {
                LargeImageText = "Powered By Assist"
            };

            switch (GameSettings.Current.RichPresenceSettings.LargeImageData)
            {
                case "Map":
                    rpObj.Assets.LargeImageKey = MapNames.MapsByPath[playerPres.matchMap].ToLower();
                    break;
                case "Agent":
                    if(GameSettings.Current.RichPresenceSettings.ShowAgent)
                        rpObj.Assets.LargeImageKey = "default"; // Add in AGENT
                    else
                        rpObj.Assets.LargeImageKey = "default";
                    break;
                default:
                    rpObj.Assets.LargeImageKey = "default";
                    break;

            }

            if (GameSettings.Current.RichPresenceSettings.ShowRank && GameSettings.Current.RichPresenceSettings.SmallImageData.Equals("Rank"))
            {
                rpObj.Assets.SmallImageKey = $"rank_{playerPres.competitiveTier}";
                rpObj.Assets.SmallImageText = CompetitiveNames.RankNames[playerPres.competitiveTier];
            }

            if (GameSettings.Current.RichPresenceSettings.ShowParty)
            {
                var Party = new DiscordRPC.Party()
                {
                    ID = playerPres.partyId,
                    Max = playerPres.maxPartySize,
                    Privacy = privacy,
                    Size = playerPres.partySize
                };

                rpObj.State = "Party: ";

                rpObj.Party = Party;
            }

            rpObj.Details = details;

            this.UpdatePresence(rpObj);
        }

        private async void DeterminePregamePresence(RichPresence rpObj, PlayerPresence playerPres)
        {

            if (playerPres.partyState.Equals("MATCHMADE_GAME_STARTING"))
                return;

            if (string.IsNullOrEmpty(playerPres.matchMap))
            {
                return;
            }

            string details = string.Empty;
            ulong timeStart = 0;

            DiscordRPC.Party.PrivacySetting privacy;

            if (playerPres.partyAccessibility.Equals("CLOSED"))
            {
                privacy = DiscordRPC.Party.PrivacySetting.Private;
            }
            else
            {
                privacy = DiscordRPC.Party.PrivacySetting.Public;
            }

            string state = "Agent Select";

            Log.Information("DISCORD PRC MAP: " + playerPres.matchMap);
            string mapName = MapNames.MapsByPath[playerPres.matchMap];

            if (GameSettings.Current.RichPresenceSettings.ShowRank && string.Equals(GameSettings.Current.RichPresenceSettings.DetailsTextData, "Rank")) // Show Rank as Details
            {
                details = $"{CompetitiveNames.RankNames[playerPres.competitiveTier]}";
            }

            if (!string.Equals(GameSettings.Current.RichPresenceSettings.DetailsTextData, "Rank") || !GameSettings.Current.RichPresenceSettings.ShowRank) // Fall back to anything as this is the only other option.
            {
                if (playerPres.partyState.Contains("CUSTOM_GAME"))
                    details = "Custom Game";
                else
                {
                    if (GameSettings.Current.RichPresenceSettings.ShowMode)
                        details = char.ToUpper(playerPres.queueId[0]) + playerPres.queueId.Substring(1);
                }
            }

            rpObj.Assets = new Assets
            {
                LargeImageKey = mapName.ToLower(),
                LargeImageText = "Powered By Assist"
            };

            if (GameSettings.Current.RichPresenceSettings.ShowRank && GameSettings.Current.RichPresenceSettings.SmallImageData.Equals("Rank"))
            {
                rpObj.Assets.SmallImageKey = $"rank_{playerPres.competitiveTier}";
                rpObj.Assets.SmallImageText = CompetitiveNames.RankNames[playerPres.competitiveTier];
            }

            rpObj.Details = details;
            rpObj.State = state;

            if (GameSettings.Current.RichPresenceSettings.ShowParty)
            {
                var Party = new DiscordRPC.Party()
                {
                    ID = playerPres.partyId,
                    Max = playerPres.maxPartySize,
                    Privacy = privacy,
                    Size = playerPres.partySize
                };

                state = "Party: ";

                rpObj.Party = Party;
            }

            rpObj.Secrets = null;
            rpObj.Timestamps = null;

            this.UpdatePresence(rpObj);
        }

        private async void DetermineMenusPresence(RichPresence rpObj, PlayerPresence playerPres)
        {
            string details = string.Empty;
            ulong timeStart = 0;

            switch (playerPres.partyState)
            {
                case "DEFAULT":
                    details = "In Lobby";
                    break;
                case "MATCHMAKING":
                    if(GameSettings.Current.RichPresenceSettings.ShowMode)
                        details = $"Queuing {char.ToUpper(playerPres.queueId[0]) + playerPres.queueId.Substring(1)}"; // magic woo, Capitalizes first letter.
                    else
                        details = $"In Queue";
                    break;
                default:
                    details = "In Lobby";
                    break;
            }

            // Set Details
            rpObj.Details = details;

            string state = string.Empty;

            DiscordRPC.Party.PrivacySetting privacy;

            if (playerPres.partyAccessibility.Equals("CLOSED"))
            {
                privacy = DiscordRPC.Party.PrivacySetting.Private;
            }
            else
            {
                privacy = DiscordRPC.Party.PrivacySetting.Public;
            }

            if (GameSettings.Current.RichPresenceSettings.ShowParty)
            {
                var Party = new DiscordRPC.Party()
                {
                    ID = playerPres.partyId,
                    Max = playerPres.maxPartySize,
                    Privacy = privacy,
                    Size = playerPres.partySize
                };

                state = "Party: ";

                rpObj.Party = Party;
            }

            rpObj.State = state;

            rpObj.Assets = new Assets
            {
                LargeImageKey = "default",
                LargeImageText = "Powered By Assist"
            };

            if (GameSettings.Current.RichPresenceSettings.ShowRank && GameSettings.Current.RichPresenceSettings.SmallImageData.Equals("Rank"))
            {
                rpObj.Assets.SmallImageKey = $"rank_{playerPres.competitiveTier}";
                rpObj.Assets.SmallImageText = CompetitiveNames.RankNames[playerPres.competitiveTier];
            }

            rpObj.Secrets = null;

            this.UpdatePresence(rpObj);
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

        private async Task<string> DetermineQueueKey(PlayerPresence playerPres)
        {
            switch (playerPres.queueId)
            {
                case "ggteam":
                    return "Escalation";
                case "deathmatch":
                    return "Deathmatch";
                case "spikerush":
                    return "SpikeRush";
                case "competitive":
                    return "Competitive";
                case "unrated":
                    return "Unrated";
                default:
                    return "VALORANT";
            }

        }
    }
}
