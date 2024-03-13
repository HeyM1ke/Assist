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
                    Log.Information("Attempting Token Refresh with Local");
                    await AssistApplication.ActiveUser.Authentication.AuthenticateWithLocal();
                    break;
                case EAuthType.CLOUD:
                    Log.Information("Attempting Token Refresh with Cloud");
                    await AssistApplication.ActiveUser.Authentication.ReAuthWithCookies(); // User doesnt need to reauth if cookies are stored from login
                    break;
                case EAuthType.COOKIE:
                    Log.Information("Attempting Token Refresh with Cookies");
                    await AssistApplication.ActiveUser.Authentication.ReAuthWithCookies();
                    break;
                default:
                    Log.Information("Auth type of unknown found. No Reauth.");
                    break;
            }
            
            timeOfLastRe = DateTime.Now.AddMinutes(1);
            
            Log.Information("Time of Last Reauth: " + timeOfLastRe.ToLongTimeString());
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