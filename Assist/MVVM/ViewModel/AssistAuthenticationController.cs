using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Assist.Settings;
using ValNet;

namespace Assist.MVVM.ViewModel
{
    /// <summary>
    /// Handles all authentication within the Assist Program.
    /// </summary>
    internal class AssistAuthenticationController
    {
        // Username Pass Login

        // Cookie Login
        public async Task CookieLogin()
        {
            
        }

        // Profile Login
        public static async Task<RiotUser> ProfileLogin(ProfileSetting profile)
        {
            RiotUser user = new RiotUser();

            await AddCookiesToUser(profile, user);

            var gamename = profile.Gamename;
            var tagLine = profile.Tagline;

            AssistLog.Normal($"Authentcating with Cookies for User {profile.ProfileUuid} / {gamename}#{tagLine}");
            try
            {
                AssistLog.Normal($"Authentcating with Cookies for User {profile.ProfileUuid} / {gamename}#{tagLine}");
                await user.Authentication.AuthenticateWithCookies();
            }
            catch (Exception ex)
            {
                throw new Exception("Acc not Valid");
            }

            return user;
        }


        private static async Task AddCookiesToUser(ProfileSetting p, RiotUser u)
        {
            foreach (Cookie cookie in p.Convert64ToCookies().GetAllCookies())
            {
                u.UserClient.CookieContainer.Add(cookie);
            }
        }
    }
}
