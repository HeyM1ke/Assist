using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.ViewModels;
using AssistUser.Lib.V2.Models;
using AssistUser.Lib.V2.Models.Dodge;
using Serilog;

namespace Assist.Services.Assist;

/// <summary>
/// Dodge Service Link
/// </summary>
public class DodgeService
{
    /// <summary>
    /// Contains the List of Dodge List People Locally.
    /// </summary>
    public PlayerDodgeList DodgeList = new ();
    public static DodgeService Current { get; set; }
    public Action<UserDodgePlayer?> DodgeUserAddedToList;
    public Action<UserDodgePlayer?> DodgeUserRemovedFromList;
    
    public DodgeService()
    {
        if (Current is not null)
            return; 
            
        Current = this;
        LoadDodgeList();
    }

    public async void LoadDodgeList()
    {
        // First check if the user is currently signed in.

        if (string.IsNullOrEmpty(AssistApplication.AssistUser.userTokens.AccessToken)) // Simple Check before real checks later.
            return;
        
        try
        {
            Log.Information("Pinging Server to Receive Dodge List");

            var resp = await AssistApplication.AssistUser.DodgeList.GetPlayerDodgeList();

            if (resp.Code != 200)
            {
                Log.Error("Failed to receieve user dodge list.");
                Log.Error($"Resp Data: CODE: {resp.Code}");
                Log.Error($"Resp Data: MESSAGE: {resp.Message}");
                return;
            }

            DodgeList = JsonSerializer.Deserialize<PlayerDodgeList>(resp.Data.ToString());
            
            Log.Information("Successfully Read Dodge data from Server");
        }
        catch
        {
            Log.Error("Dodge Settings File was not found or tampered with.");
        }
    }
    
    public async Task UpdateDodgeList()
    {
        // First check if the user is currently signed in.

        if (string.IsNullOrEmpty(AssistApplication.AssistUser.userTokens.AccessToken)) // Simple Check before real checks later.
            return;
        
        try
        {
            Log.Information("Pinging Server to Receive Dodge List");

            var resp = await AssistApplication.AssistUser.DodgeList.GetPlayerDodgeList();

            if (resp.Code != 200)
            {
                Log.Error("Failed to receieve user dodge list.");
                Log.Error($"Resp Data: CODE: {resp.Code}");
                Log.Error($"Resp Data: MESSAGE: {resp.Message}");
                return;
            }

            DodgeList = JsonSerializer.Deserialize<PlayerDodgeList>(resp.Data.ToString());
            
            Log.Information("Successfully Read Dodge data from Server");
        }
        catch
        {
            Log.Error("Dodge Settings File was not found or tampered with.");
        }   
    }

    public async Task<UserDodgePlayer> AddPlayerToUserDodgeList(AddUserToDodgeListModel data)
    {
        var resp = await AssistApplication.AssistUser.DodgeList.AddPlayerToPlayerDodgeList(data);

        if (resp.Code != 200)
            throw new Exception(resp.Message);

        var newUser = JsonSerializer.Deserialize<UserDodgePlayer>(resp.Data.ToString());
        
        DodgeList.Players.Add(newUser); // This updates the list locally without having to pull from the server.
        
        DodgeUserAddedToList?.Invoke(newUser); // Alerts that there is a new user added
        
        return newUser;
    }
    
    public async Task<UserDodgePlayer> RemovePlayerFromUserDodgeList(string userIdToRemove)
    {
        var resp = await AssistApplication.AssistUser.DodgeList.RemovePlayerFromPlayerDodgeList(userIdToRemove);

        if (resp.Code != 200)
            throw new Exception(resp.Message);

        var newUser = JsonSerializer.Deserialize<UserDodgePlayer>(resp.Data.ToString());

        var oldP = DodgeList.Players.Find(x => x.PlayerId == newUser.PlayerId);
        
        DodgeList.Players.Remove(oldP); // This updates the list locally without having to pull from the server.
        
        DodgeUserRemovedFromList?.Invoke(oldP); // Alerts that there is a new user added
        
        return oldP;
    }
}