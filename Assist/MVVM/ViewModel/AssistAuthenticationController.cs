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
        public static async Task<RiotUser> CookieLogin(CookieContainer cc)
        {
            RiotUser user = new RiotUser();

            await AddCookiesToUser(cc, user);
            AssistLog.Normal($"Authenticating with New User");
            try
            {
                AssistLog.Normal($"Authenticating with Cookies for New User");
                await user.Authentication.AuthenticateWithCookies();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return user;

        }

        // Profile Login
        public static async Task<RiotUser> ProfileLogin(ProfileSetting profile)
        {
            RiotUser user = new RiotUser();

            await AddCookiesToUser(profile, user);

            var gamename = profile.Gamename;
            var tagLine = profile.Tagline;

            AssistLog.Normal($"Authenticating with Cookies for User {profile.ProfileUuid} / {gamename}#{tagLine}");
            try
            {
                AssistLog.Normal($"Authenticating with Cookies for User {profile.ProfileUuid} / {gamename}#{tagLine}");
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

        private static async Task AddCookiesToUser(CookieContainer cc, RiotUser u)
        {
            foreach (Cookie cookie in cc.GetAllCookies())
            {
                u.UserClient.CookieContainer.Add(cookie);
            }
        }


        public static async Task<ProfileSetting> CreateProfile(RiotUser user)
        {
            // Save Cookies
            ProfileSetting userSettings = new ProfileSetting()
            {
                Gamename = user.UserData.acct.game_name,
                Tagline = user.UserData.acct.tag_line,
                ProfileUuid = user.UserData.sub,
                Region = user.UserRegion,
            };

            userSettings.ConvertCookiesTo64(user.UserClient.CookieContainer);
            await userSettings.SetupProfile(user);

            return userSettings;
        }
    }
}
