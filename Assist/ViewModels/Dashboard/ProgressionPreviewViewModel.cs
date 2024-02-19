using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ValNet.Objects.Contracts;

namespace Assist.ViewModels.Dashboard;

public partial class ProgressionPreviewViewModel : ViewModelBase
{
    public Dictionary<string, object> ProgressionProgress = new Dictionary<string, object>();
    private static List<ContactsFetchObj.Mission> allMissions = null;
    private static ContactsFetchObj _userContacts = null;
    public static DailyTicketObj UserTicket = null;

    [ObservableProperty] private bool _weeklyMissionsCompleted = false;
    
    
    [ObservableProperty] private string _playerRankIcon = "";
    [ObservableProperty] private string _rankName = "";
    [ObservableProperty] private string _playerRR = "";
    
    
    public async Task Setup()
    {
       
        var w = await GetWeeklyMissions();
        await SetupCompetitiveDetails();
    }

    private async Task SetupCompetitiveDetails()
    {
            
    }

    private async Task<object> GetWeeklyMissions()
    {
        return null;
    }
}