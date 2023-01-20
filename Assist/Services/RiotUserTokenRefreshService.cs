using System;
using Assist.ViewModels;
using Serilog;
using ValNet.Enums;

namespace Assist.Services;

public class RiotUserTokenRefreshService
{
    private bool _attemptingReauth = false;
    
    public RiotUserTokenRefreshService()
    {
        AssistApplication.Current.CurrentUser.TokensExpired += CurrentUserOnTokensExpired;
    }

    private async void CurrentUserOnTokensExpired()
    {
        // Do Something on TOken Expire Detected
        Log.Information("Token Expired Detected");
        if (_attemptingReauth) // Check in place just in case of multiple requests sent at once
        {
            Log.Information("Already Attempting Reauth");
            return;
        }

        try
        {
            _attemptingReauth = true;
            switch (AssistApplication.Current.CurrentUser.Authentication.AuthType)
            {
                case EAuthType.LOCAL:
                    await AssistApplication.Current.CurrentUser.Authentication.AuthenticateWithLocal();
                    break;
                case EAuthType.CLOUD:
                    await AssistApplication.Current.CurrentUser.Authentication.AuthenticateWithCookies(); // User doesnt need to reauth if cookies are stored from login
                    break;
                case EAuthType.COOKIE:
                    await AssistApplication.Current.CurrentUser.Authentication.AuthenticateWithCookies();
                    break;
                default:
                    Log.Information("Auth type of unknown found. No Reauth.");
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error("Failed to Reauth");
            Log.Error("Reauth Exception: " + e.Message);
            Log.Error("Reauth stack: " + e.StackTrace);
            _attemptingReauth = false;
        }
    }
}