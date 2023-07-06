using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Assist.Controls.Progression;
using Assist.Objects.AssistApi.Valorant;
using Assist.ViewModels;
using Assist.Views.Store.ViewModels;
using DynamicData;
using ReactiveUI;
using ValNet.Objects.Contracts;
using ValNet.Objects.Exceptions;

namespace Assist.Controls.Dashboard.ViewModels;

public class ProgressionOverviewViewModel : ViewModelBase
{
    private ObservableCollection<MissionControl> _weeklyMissionControls = new ObservableCollection<MissionControl>();

    public ObservableCollection<MissionControl> WeeklyMissionControls
    {
        get => _weeklyMissionControls;
        set => this.RaiseAndSetIfChanged(ref _weeklyMissionControls, value);
    }
    
    private ObservableCollection<MissionControl> _dailyMissionControls = new ObservableCollection<MissionControl>();
    
    public ObservableCollection<MissionControl> DailyMissionControls
    {
        get => _dailyMissionControls;
        set => this.RaiseAndSetIfChanged(ref _dailyMissionControls, value);
    }

    private string _kcAmount = "0 KC";
    public string KCAmount
    {
        get => _kcAmount;
        set => this.RaiseAndSetIfChanged(ref _kcAmount, value);
    }
    
    private static List<Mission> allMissions = null;
    private static ContactsFetchObj _userContacts = null;
    public static DailyTicketObj UserTicket = null;
    
    public async Task Setup()
    {
        var d = await GetDailyMissions();
        var w = await GetWeeklyMissions();

        DailyMissionControls.AddRange(d);
        WeeklyMissionControls.AddRange(w);

        await SetupKCCredits();
    }
    
    public async Task<IEnumerable<MissionControl>> GetDailyMissions()
        {
            
            if(_userContacts is null)
                _userContacts = await AssistApplication.Current.CurrentUser.Contracts.GetAllContracts();

            if (allMissions is null)
                allMissions = await AssistApplication.ApiService.GetAllMissions();

            var date = DateTime.Now.AddDays(1);


            var dailyMissions = _userContacts.Missions.FindAll(_mission => (_mission.ExpirationTime.Day == date.Day) || (_mission.ExpirationTime.Day == DateTime.Now.Day));

            List<MissionControl> controls = new List<MissionControl>();

            for (int i = 0; i < dailyMissions.Count; i++)
            {
                var missionData = allMissions.Find(_m => dailyMissions[i].ID == _m.Uuid);

                if (missionData == null) { continue; } // Sanity check, got annoyed at the warning. --Shiick

                controls.Add(new MissionControl()
                {
                    Height = 30,
                    Title = missionData.Title,
                    CurrentProgress = dailyMissions[i].Objectives.First().Value,
                    MaxProgress = missionData.ProgressToComplete,
                    XpGrantAmount = $"{missionData.XpGrant}XP",
                    PreviewText = $"{dailyMissions[i].Objectives.First().Value}/{missionData.ProgressToComplete}"
                });
            }

            return controls;
        }

    public async Task<IEnumerable<MissionControl>> GetWeeklyMissions()
        {
            if (_userContacts is null)
                _userContacts = await AssistApplication.Current.CurrentUser.Contracts.GetAllContracts();

            if (allMissions is null)
                allMissions = await AssistApplication.ApiService.GetAllMissions();

            var date = DateTime.Now.AddDays(1);

            var weeklyMissions = _userContacts.Missions.FindAll(_mission => (_mission.ExpirationTime.Day != date.Day) || (_mission.ExpirationTime.Day != DateTime.Now.Day));
            // Changed var name for clarity --Shiick

            List<MissionControl> controls = new List<MissionControl>();


            for (int i = 0; i < weeklyMissions.Count; i++)
            {
                var missionData = allMissions.Find(_m => weeklyMissions[i].ID == _m.Uuid);

                if (missionData == null) { continue; } // Sanity check, got annoyed at the warning. --Shiick

                if (missionData.XpGrant == 2000) { continue; }// Dirty fix but it does what it's supposed to do... --Shiick

                controls.Add(new MissionControl()
                {
                    Height = 30,
                    Title = missionData.Title,
                    CurrentProgress = weeklyMissions[i].Objectives.First().Value,
                    MaxProgress = missionData.ProgressToComplete,
                    XpGrantAmount = $"{missionData.XpGrant}XP",
                    PreviewText = $"{weeklyMissions[i].Objectives.First().Value}/{missionData.ProgressToComplete}"
                });
            }

            return controls;
        }

    public async Task SetupKCCredits()
    {
        if (StoreViewModel._UserWallets.ContainsKey(AssistApplication.Current.CurrentUser.UserData.sub))
        {
            KCAmount = $"{StoreViewModel._UserWallets[AssistApplication.Current.CurrentUser.UserData.sub].Balances.KingdomCredits:n0} KC";
            return;
        }
        try
        {
            var t = await AssistApplication.Current.CurrentUser.Store.GetPlayerWallet();
            StoreViewModel._UserWallets.Add(AssistApplication.Current.CurrentUser.UserData.sub,t);
            KCAmount = $"{StoreViewModel._UserWallets[AssistApplication.Current.CurrentUser.UserData.sub].Balances.KingdomCredits:n0} KC";
        }
        catch (RequestException e)
        {
            Console.WriteLine("Failed to Get Wallet");
            Console.WriteLine(e.Content);
            Console.WriteLine(e.StatusCode);
            return;
        }
    }
    public async Task HandleDailyTicket()
    {
        try
        {
            if (UserTicket is null)
                UserTicket = await AssistApplication.Current.CurrentUser.Contracts.GetDailyTicket();
        }
        catch (RequestException e)
        {
            Console.WriteLine("Failed to Get Daily Ticket");
            Console.WriteLine(e.Content);
            Console.WriteLine(e.StatusCode);
            return;
        }
        
    }
    
    
}