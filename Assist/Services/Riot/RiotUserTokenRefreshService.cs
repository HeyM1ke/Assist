using System;
using System.Threading.Tasks;
using Assist.ViewModels;
using Serilog;
using ValNet.Enums;

namespace Assist.Services.Riot;

public class RiotUserTokenRefreshService
{
    private bool _attemptingReauth = false;
    private DateTime timeOfLastRe;
    public RiotUserTokenRefreshService()
    {
        
    }

    public async Task CurrentUserOnTokensExpired()
    {
        if (timeOfLastRe != null)
        {
            if (DateTime.Compare(DateTime.Now, timeOfLastRe) < 0)
            {
                return;
            }
        }
        
        
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
            switch (AssistApplication.ActiveUser.Authentication.AuthType)
            {
                case EAuthType.LOCAL:
                    await AssistApplication.ActiveUser.Authentication.AuthenticateWithLocal();
                    break;
                case EAuthType.CLOUD:
                    await AssistApplication.ActiveUser.Authentication.AuthenticateWithCookies(); // User doesnt need to reauth if cookies are stored from login
                    break;
                case EAuthType.COOKIE:
                    await AssistApplication.ActiveUser.Authentication.ReAuthWithCookies();
                    break;
                default:
                    Log.Information("Auth type of unknown found. No Reauth.");
                    break;
            }
            
            timeOfLastRe = DateTime.Now.AddMinutes(1);
            _attemptingReauth = false;
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