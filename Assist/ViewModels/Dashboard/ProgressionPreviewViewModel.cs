using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Assist.Controls.Progression;
using Assist.Core.Helpers;
using Assist.Objects.AssistApi.Valorant;
using Assist.Shared.Settings;
using Assist.Shared.Settings.Accounts;

using AssistUser.Lib.Base.Exceptions;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using ValNet.Objects.Contracts;

namespace Assist.ViewModels.Dashboard;

public partial class ProgressionPreviewViewModel : ViewModelBase
{
    public Dictionary<string, object> ProgressionProgress = new Dictionary<string, object>();
    private static List<Mission> allMissions = null;
    private static ContactsFetchObj _userContacts = null;
    public static DailyTicketObj UserTicket = null;
    private static string _currentUser;
    
    [ObservableProperty]
    private ObservableCollection<DailyTicketDiamond> _dailyDiamonds = new ObservableCollection<DailyTicketDiamond>();
    
    [ObservableProperty]
    private ObservableCollection<PreviewMissionControl> _weeklyMissions = new ObservableCollection<PreviewMissionControl>();
    
    [ObservableProperty] private bool _weeklyMissionsCompleted = false;
    
    [ObservableProperty] private string _seasonWins = "";
    [ObservableProperty] private string _playerRankIcon = "";
    [ObservableProperty] private string _rankName = "";
    [ObservableProperty] private string _playerRR = "";

    private bool _newUser = false;
    public async Task Setup()
    {
        await HandleDailyTicket();
        await SetupWeeklyMissions();
        await SetupCompetitiveDetails();
        _currentUser = AssistApplication.ActiveUser.UserData.sub;
        _newUser = false;
        
    }

    public async Task LoadedCheck()
    {
        if (!string.IsNullOrEmpty(_currentUser) && _currentUser != AssistApplication.ActiveUser.UserData.sub)
        {
            _newUser = true;
            DailyDiamonds.Clear();
            WeeklyMissions.Clear();
            await Setup();
        }
    }
    public async Task HandleDailyTicket()
    {
        try
        {
            await AssistApplication.ActiveUser.Contracts.RenewDailyTicket(); // God knows what this does. Writing this before it resets the daily ticket every time LMFAO
            UserTicket = await AssistApplication.ActiveUser.Contracts.GetDailyTicket();
        }
        catch (RequestException e)
        {
            Log.Information("Failed to Get Daily Ticket");
            Log.Information(e.Content);
            Log.Information($"Status Code: {e.StatusCode}");
            return;
        }

        CreateDailyTicketItems();

    }

    private void CreateDailyTicketItems()
    {
        if (UserTicket is null)
        {
            Log.Error("Player failed to get the user ticket.");
            return;
        }
        
        for (int i = 0; i < UserTicket.DailyRewards.Milestones.Count; i++)
        {
            var currMilestone = UserTicket.DailyRewards.Milestones[i];

            bool _prevCompleted = false;
            if (i > 0)
                _prevCompleted = UserTicket.DailyRewards.Milestones[i - 1].BonusApplied || UserTicket.DailyRewards.Milestones[i - 1].Progress == 4;
            else
                _prevCompleted = true;
            
            DailyDiamonds.Add(new DailyTicketDiamond()
            {
                DiamondNumber = $"{i+1}",
                IsCompleted = currMilestone.BonusApplied || currMilestone.Progress == 4,
                IsCurrent = _prevCompleted && currMilestone.Progress != 4,
                ProgressText = currMilestone.Progress != 4 && _prevCompleted ? $"{currMilestone.Progress}/4" : string.Empty
            });
        }
    }

    private async Task SetupCompetitiveDetails()
    {
        try
        {
            var playerMmr = await AssistApplication.ActiveUser.Player.GetPlayerMmr();

            int currentRankTier = 0;
            int currentRR = 0;
            var currentSeasonId = playerMmr.LatestCompetitiveUpdate.SeasonID;
            if (playerMmr.QueueSkills.competitive.SeasonalInfoBySeasonID != null)
            {
                currentRankTier = playerMmr.QueueSkills.competitive.SeasonalInfoBySeasonID[currentSeasonId].CompetitiveTier;
                currentRR = playerMmr.QueueSkills.competitive.SeasonalInfoBySeasonID[currentSeasonId].RankedRating;
                SeasonWins =
                    $"{playerMmr.QueueSkills.competitive.SeasonalInfoBySeasonID[currentSeasonId].NumberOfWins} Wins";
            }

            if (currentRankTier >= 24) PlayerRR = $"{currentRR}RR";
            else PlayerRR = $"{currentRR}/100 RR";
            PlayerRankIcon = $"https://cdn.assistval.com/ranks/TX_CompetitiveTier_Large_{currentRankTier}.png";
            var t = AccountSettings.Default.Accounts.Find(pfp => pfp.Id == AssistApplication.ActiveAccountProfile.Id);

            t.Personalization.ValRankTier = currentRankTier;
            await AccountSettings.Default.UpdateAccount(t);
            AssistApplication.ActiveAccountProfile = t;
                
            if (currentRankTier != null)
                RankName = ValorantHelper.RankNames[currentRankTier].ToUpper();
                
                
        }
        catch (Exception e)
        {
            if (e is RequestException)
            {
                var t =e as RequestException;
                Log.Error(t.Content);
                Log.Error($"{t.StatusCode}");
            }
            Log.Error(e.Message);
        }
    }

    public async Task SetupWeeklyMissions()
    {
        if (_userContacts is null || _newUser)
            _userContacts = await AssistApplication.ActiveUser.Contracts.GetAllContracts();

        if (allMissions is null)
            allMissions = await AssistApplication.AssistApiService.GetAllMissions();

        var date = DateTime.Now.AddDays(1);

        var weeklyMissions = _userContacts.Missions.FindAll(_mission => (_mission.ExpirationTime.Day != date.Day) || (_mission.ExpirationTime.Day != DateTime.Now.Day));
        
        for (int i = 0; i < weeklyMissions.Count; i++)
        {
            var missionData = allMissions.Find(_m => weeklyMissions[i].ID == _m.Uuid);
            if (missionData is null || missionData.XpGrant == 2000) { continue; }

            WeeklyMissions.Add(new PreviewMissionControl()
            {
                Height = 30,
                Title = missionData.Title,
                CurrentProgress = weeklyMissions[i].Objectives.First().Value,
                MaxProgress = missionData.ProgressToComplete,
                XpGrantAmount = $"{missionData.XpGrant}XP",
                PreviewText = $"{weeklyMissions[i].Objectives.First().Value}/{missionData.ProgressToComplete}"
            });
        }
        if (WeeklyMissions.Count == 0) WeeklyMissionsCompleted = true;
    }
}