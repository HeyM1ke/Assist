using System.Threading.Tasks;
using Assist.Game.Services.Leagues;
using Assist.ViewModels;
using ReactiveUI;

namespace Assist.Game.Views.Leagues.ViewModels;

public class InvitePlayerPartyViewModel : ViewModelBase
{

    private string _message = string.Empty;

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }
    
    
    public async Task InvitePlayer(string? textInputText)
    {
        var ptyId = LeagueService.Instance.CurrentPartyInfo.Id;

        var response = await AssistApplication.Current.AssistUser.Party.InviteUserToParty(ptyId, textInputText);

        Message = response.Message;
    }
}