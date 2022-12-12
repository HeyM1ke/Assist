using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia.Logging;
using Serilog;
using ValNet;
using ValNet.Objects;
using ValNet.Objects.Authentication;
using ValNet.Objects.Exceptions;

namespace Assist.Services
{
    internal class AuthenticationController
    {

        public static async Task<AuthenticationResult> UsernameLogin(RiotUser user)
        {
            Log.Information("Authenticating with Username");
            try
            {
                var r = await user.Authentication.AuthenticateWithCloud();
                return r;
            }
            catch (ValNetException ex)
            {
                throw ex;
            }
        }

        public static async Task<AuthenticationResult> SendTwoFactor(RiotUser user, string code)
        {
            Log.Information("Authenticating Two Factor Code");
            try
            {
                var r = await user.Authentication.AuthenticateTwoFactorCode(code);
                return r;
            }
            catch (ValNetException ex)
            {
                throw ex;
            }
        }

        public static async Task<AuthenticationResult> CookieLogin(ProfileSettings ps, RiotUser user)
        {
            var cookies = ps.Convert64ToCookies();

            user.GetAuthClient().SetCookies(cookies);
            Log.Information("Authenticating with Cookies");
            try
            {
                var r = await user.Authentication.AuthenticateWithCookies();
                return r;
            }
            catch (ValNetException ex)
            {
                throw ex;
            }
        }

        public static async Task<AuthenticationResult> CookieLogin(RiotUser user)
        {
            
            Log.Information("Authenticating with User Cookies");
            try
            {
                var r = await user.Authentication.AuthenticateWithCookies();
                return r;
            }
            catch (ValNetException ex)
            {
                throw ex;
            }
        }
    }
}
