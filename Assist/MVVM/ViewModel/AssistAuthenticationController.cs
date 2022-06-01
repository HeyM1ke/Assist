using Assist.Settings;

using Serilog;

using System;
using System.Net;
using System.Threading.Tasks;

using ValNet;
using ValNet.Objects;

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
            var user = new RiotUser();
            AddCookiesToUser(cc, user);

            Log.Information("Authenticating with New User");
            try
            {
                Log.Information("Authenticating with Cookies for New User");
                await user.Authentication.AuthenticateWithCookies();
            }
            catch (ValNetException ex)
            {
                throw ex;
            }

            return user;

        }

        // todo: handle exception properly
        public static async Task<RiotUser> ProfileLogin(ProfileSetting profile)
        {
            var user = new RiotUser();
            AddCookiesToUser(profile, user);

            var gamename = profile.Gamename;
            var tagLine = profile.Tagline;

            Log.Information($"Authenticating with Cookies for User {profile.ProfileUuid} / {gamename}#{tagLine}");

            try
            {
                Log.Information("Authenticating with Cookies for New User");
                await user.Authentication.AuthenticateWithCookies();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


            return user;
        }

        private static void AddCookiesToUser(ProfileSetting profile, RiotUser user)
        {
            var cookies = profile.Convert64ToCookies().GetAllCookies();
            foreach (Cookie cookie in cookies)
            {
                AddCookie(cookie, user);
            }
        }

        private static void AddCookiesToUser(CookieContainer container, RiotUser user)
        {
            var cookies = container.GetAllCookies();
            foreach (Cookie cookie in cookies)
            {
                AddCookie(cookie, user);
            }
        }

        private static void AddCookie(Cookie cookie, RiotUser user)
        {
            user.UserClient.CookieContainer.Add(cookie);
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
