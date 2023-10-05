using System.Threading.Tasks;
using Assist.Services.Popup;
using Assist.ViewModels;
using AssistUser.Lib.Account.Models;
using ReactiveUI;
using Serilog;
using ValNet.Enums;

namespace Assist.Game.Views.LinkRiot.ViewModels;

public class LinkRiotViewModel : ViewModelBase
{
    private string _valorantAccountRiotName;

    public string ValorantAccountRiotName
    {
        get => _valorantAccountRiotName;
        set => this.RaiseAndSetIfChanged(ref _valorantAccountRiotName, value);
    }
    
    private string _valorantAccountRegion;

    public string ValorantAccountRegion
    {
        get => _valorantAccountRiotName;
        set => this.RaiseAndSetIfChanged(ref _valorantAccountRiotName, value);
    }
    
    private string _valorantProfilePicture;

    public string ValorantProfilePicture
    {
        get => _valorantProfilePicture;
        set => this.RaiseAndSetIfChanged(ref _valorantProfilePicture, value);
    }

    public string _errorMessage;

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public async Task LinkRiotAccountWithCurrentSocketAccount()
    {
        if (AssistApplication.Current.CurrentUser.Authentication.AuthType != EAuthType.LOCAL)
        {
            //TODO Replace with Localized Error Message
            ErrorMessage = "Please Link while in Game Mode";
            return;
        }

        RiotConnection rtc;
        if (AssistApplication.Current.CurrentUser.Authentication.AuthType == EAuthType.LOCAL)
        {
            var p = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
            var playerP = p.presences.Find(x => x.puuid == AssistApplication.Current.CurrentUser.UserData.sub);
            
            rtc = new RiotConnection()
            {
                Id = AssistApplication.Current.CurrentUser.UserData.sub,
                Region = AssistApplication.Current.CurrentUser.GetRegion().ToString(),
                RiotId = playerP.game_name,
                RiotTag = playerP.game_tag,
                VerificationCode = AssistApplication.Current.CurrentUser.TokenData.AccessToken
            };
        }
        else
        {
            rtc = new RiotConnection()
            {
                Id = AssistApplication.Current.CurrentUser.UserData.sub,
                Region = AssistApplication.Current.CurrentUser.GetRegion().ToString(),
                RiotId = AssistApplication.Current.CurrentUser.UserData.acct.game_name,
                RiotTag = AssistApplication.Current.CurrentUser.UserData.acct.tag_line,
                VerificationCode = AssistApplication.Current.CurrentUser.TokenData.AccessToken
            };
        }
        var resp = await AssistApplication.Current.AssistUser.Account.LinkRiotConnection(rtc);
        Log.Information(AssistApplication.Current.AssistUser.userTokens.AccessToken);
        if (resp.Code != 200)
        {
            ErrorMessage = resp.Message;
            Log.Error("Failed to Link Accounts.");
            Log.Error(resp.Message);
            return;
        }
        
        PopupSystem.KillPopups();
        
    }

    public async void Setup()
    {
        if (AssistApplication.Current.CurrentUser.Authentication.AuthType == EAuthType.LOCAL)
        {
            var p = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
            var playerP = p.presences.Find(x => x.puuid == AssistApplication.Current.CurrentUser.UserData.sub);
            
            ValorantAccountRiotName =
                $"{playerP.game_name}#{playerP.game_tag}";
            ValorantAccountRegion = AssistApplication.Current.CurrentUser.GetRegion().ToString();
            if (AssistApplication.Current.CurrentUser?.Inventory.CurrentInventory is null)
                await AssistApplication.Current.CurrentUser.Inventory.GetPlayerInventory();
            ValorantProfilePicture = $"https://content.assistapp.dev/playercards/{AssistApplication.Current.CurrentUser.Inventory!.CurrentInventory!.PlayerData.PlayerCardID}_SmallArt.png";
            return;
        }


        ValorantAccountRiotName =
            $"{AssistApplication.Current.CurrentUser.UserData.acct.game_name}#{AssistApplication.Current.CurrentUser.UserData.acct.tag_line}";
        ValorantAccountRegion = AssistApplication.Current.CurrentUser.GetRegion().ToString();
        if (AssistApplication.Current.CurrentUser?.Inventory.CurrentInventory is null)
            await AssistApplication.Current.CurrentUser.Inventory.GetPlayerInventory();
        ValorantProfilePicture = $"https://content.assistapp.dev/playercards/{AssistApplication.Current.CurrentUser.Inventory!.CurrentInventory!.PlayerData.PlayerCardID}_SmallArt.png";
    }
}