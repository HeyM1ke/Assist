using System;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Objects.AssistApi.Game;
using Assist.Services.Server;
using Assist.ViewModels;

namespace Assist.Game.Services;

public class AssistGameServerConnection : HubClient
{
    public event Action<object>? RecieveMessageEvent;
    private const string GAMESERVERURL = "https://api.assistapp.dev/game/main";
    public async Task Connect()
    {
        HubConnectionUrl = GAMESERVERURL;

        base.InitWithAuth(AssistApplication.Current.AssistUser.Tokens.AccessToken);

        await StartHubInternal();
    }


    public async void PartyInviteRecievedFromAssist(string data)
    {
        var inviteData = JsonSerializer.Deserialize<InvitePlayerData>(data);
        Console.WriteLine("Recieved new InviteData Data From Server:" + data);
    }
}