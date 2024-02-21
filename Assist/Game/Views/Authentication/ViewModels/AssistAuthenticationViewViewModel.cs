using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Objects.Helpers;
using Assist.Services;
using Assist.Services.Server;
using Assist.Settings;
using Assist.ViewModels;
using Assist.Views;
using AssistUser.Lib.Account.Models;
using Avalonia.Threading;
using ReactiveUI;
using Serilog;
using Tmds.DBus;

namespace Assist.Game.Views.Authentication.ViewModels
{
    internal class AssistAuthenticationViewViewModel : ViewModelBase
    {
        private bool _authProcessActive = false;

        public bool AuthProcessActive
        {
            get => _authProcessActive;
            set => this.RaiseAndSetIfChanged(ref _authProcessActive, value);
        }

        private bool _hideButtons = true;

        public bool ShowButtons
        {
            get => _hideButtons;
            set => this.RaiseAndSetIfChanged(ref _hideButtons, value);
        }

        private bool _usernameNeeded = false;

        public bool UsernameNeeded
        {
            get => _usernameNeeded;
            set => this.RaiseAndSetIfChanged(ref _usernameNeeded, value);
        }

        private string? _errorMessage;

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }


        private ConnectionHub _connectionHub = new ConnectionHub();
        public async Task DiscordAuth()
        {
            ShowButtons = false;
            AuthProcessActive = true;
            
            // On Press connect to Server
            await _connectionHub.Connect();

            _connectionHub.RedirectCodeEvent += ConnectionHubOnRedirectCodeEvent;

            var currentConnectionId = _connectionHub.ConnectionId;
            var discordAuthUrl = await GenerateDiscordOAuthUrl(currentConnectionId);

            // Open Browser
            if (discordAuthUrl == null)
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = discordAuthUrl,
                UseShellExecute = true
            });

        }

        private async void ConnectionHubOnRedirectCodeEvent(string obj)
        {
            Log.Information("Code Recieved by Client");
            

            try
            {
                var authResp = await AssistApplication.Current.AssistUser.Authentication.AuthenticateWithRedirectCode(obj);

                //Temp Location for Refresh token, save settings
                AssistSettings.Current.AssistUserCode = authResp.RefreshToken;
                AssistSettings.Save();

                var userInfo = await AssistApplication.Current.AssistUser.Account.GetUserInfo();

                if (string.IsNullOrEmpty(userInfo.username))
                {
                    AuthProcessActive = false;
                    UsernameNeeded = true;
                    return;
                }

                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    if (AssistApplication.Current.Mode != AssistMode.GAME)
                    {
                        MainWindowContentController.Change(new MainView());
                        return;
                    }
                    Log.Information("Attempting To Connect to Game Server");
                    await AssistApplication.Current.GameServerConnection.Connect();

                    AssistApplication.Current.OpenGameView();
                });
            }
            catch (Exception ex)
            {
                Log.Fatal("Failed to Authenticate");
                Log.Fatal("Failed Auth Error Messages ====");
                Log.Fatal(ex.Message);
                Log.Fatal(ex.StackTrace);
            }
        }

        public async Task SetUsername(string userName)
        {
            try
            {
                var authResp = await AssistApplication.Current.AssistUser.Account.ChangeUsername(new AssistChangeUsernameModel()
                {
                    username = userName
                });

                if (authResp)
                {
                    if (AssistApplication.Current.Mode != AssistMode.GAME)
                    {
                        MainWindowContentController.Change(new MainView());
                        return;
                    }
                    AssistApplication.Current.OpenGameView();
                }
                else
                {
                    ErrorMessage = "Username Taken.";
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        private async Task<string> GenerateDiscordOAuthUrl(string id)
        {
            return
                $"https://discord.com/api/oauth2/authorize?client_id=984912187837526038&redirect_uri=https%3A%2F%2Fapi.assistapp.dev%2Fauthentication%2Fredirect&response_type=code&scope=identify%20email%20connections&state={id}";
        }
    }
}
