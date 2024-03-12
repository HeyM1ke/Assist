using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Assist.Models.Enums;
using Assist.Services.Assist;
using Assist.Services.Modules;
using Assist.Services.Riot;
using Assist.Shared.Settings;
using Assist.Shared.Settings.Modules;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using ValNet;
using ValNet.Enums;
using ValNet.Objects;
using ValNet.Objects.Exceptions;

namespace Assist.ViewModels.Game;

public partial class GameInitalStartupViewModel : ViewModelBase
{
    [ObservableProperty] private string _message = "";
    
    public async Task Inital()
    {
        if (Design.IsDesignMode)
            return;
        
        // Start Setup
        await AssistApplication.SwapAssistMode(EAssistMode.GAME);
        while (!IsValorantRunning())
        {
            // wait this is really funny hold on.
            Message = "Valorant is not Running. Refreshing in 5 seconds";
            await Task.Delay(5000);
        }

        // Connect to Valorant Websocket Through Socket Service.
        Log.Information("Attempting to create local valorant user");
        Message = "Connecting to VALORANT";
        await ConnectToGame();
        Log.Information("Created local valorant user");
        Log.Information("Connecting to Socket");
        await StartSocketConnection();
        
        Log.Information("Checking if an Assist account is currently logged in");
        
        try
        {
            var t = await AssistApplication.AssistUser.Account.GetAccountInfo();
            
            if (t.Code != 200)
            {
                var authResp = await AssistApplication.AssistUser.Authentication.AuthenticateWithRefreshToken(AssistSettings.Default.GetAssistUserCode());
                AssistSettings.Default.SaveAssistUserCode(authResp.RefreshToken);
                AssistSettings.Save();
            }
        }
        catch (Exception e)
        {
            Log.Fatal("Assist Account Token is not Valid");
        }
        
        try
        {
            Message = "Connecting to Assist Servers";
            Log.Information("Attempting To Connect to Game Server");
        }
        catch (Exception e)
        {
            Log.Fatal("Failed to Connect to Game Server");
            return;
        }

        // TODO: Start Module Service which will then active these in turn if they are enabled.
        new DodgeService();
        new RecentService();

        if (ModuleSettings.Default.RichPresenceSettings.IsEnabled)
        {
            Message = "Starting Discord Presence";
            RichPresenceService.Default.Initialize();
        }
        
        Message = "Welcome to Assist";
        await AssistApplication.SetupComplete_Game();
    }
    
    private bool IsValorantRunning()
    {
        var processlist = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Where(process => process.Id != Process.GetCurrentProcess().Id).ToList();
        processlist.AddRange(Process.GetProcessesByName("VALORANT-Win64-Shipping"));
            
        return processlist.Any();
    }

    private async Task StartSocketConnection()
    {
        
        Log.Information("Attempting to connect to Socket");
        await AssistApplication.RiotWebsocketService.Connect();
        Message = "Connected to Live Data Socket.";
        Log.Information("Connected to Socket");
            
    }

    private async Task ConnectToGame()
    {
        RiotUser user = new RiotUserBuilder().WithSettings(new RiotUserSettings()
        {
            AuthenticationMethod = AuthenticationMethod.CURL,
        }).Build();

        try
        {
            var r = await user.Authentication.AuthenticateWithLocal();
        }
        catch (Exception e)
        {
            Log.Error("Error On Local Auth Connection");
            if (e is ValNetException)
            {
                var ex = e as ValNetException;
                Log.Fatal("ERROR:" + ex.Message);
            }

            throw e;
        }
        Message = "Connection Successful";

        AssistApplication.ActiveUser = user;
    }
}