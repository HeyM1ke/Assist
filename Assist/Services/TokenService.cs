using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Assist.MVVM.ViewModel;
using Serilog;
using ValNet;

namespace Assist.Services
{
    /// <summary>
    /// Service that refreshes Tokens within current Riot User in the application.
    /// </summary>
    internal class TokenService
    {
        public async Task RefreshCurrentUser()
        {
            var u = AssistApplication.AppInstance.CurrentUser;

            await RefreshUser(u);

        }

        public async Task RefreshUser(RiotUser u)
        {
            var tempUser = new RiotUser();
            foreach (Cookie cook in u.UserClient.CookieContainer.GetAllCookies())
            {
                tempUser.UserClient.CookieContainer.Add(cook);
            }

            await tempUser.Authentication.AuthenticateWithCookies();

            AssistApplication.AppInstance.CurrentUser = tempUser;
            AssistApplication.AppInstance.CurrentProfile.ConvertCookiesTo64(tempUser.UserClient.CookieContainer);
        }

    }

    public class TokenServiceBackgroundService : BackgroundWorker
    {
        private TokenService _tokenService = new();
        private BackgroundWorker _worker = new();
        private const int REFRESHINMINUTES = 30;
        public TokenServiceBackgroundService()
        {
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerAsync();
        }

        private void _worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            RunRefresh();
        }

        public async void RunRefresh()
        {
            Log.Debug("Backgorund Refresh Start");
            Thread.Sleep(REFRESHINMINUTES * 60000);
            await _tokenService.RefreshCurrentUser();

            Log.Debug("Refreshed");
            Log.Debug(AssistApplication.AppInstance.CurrentUser.tokenData.access);
        }
    }
}
