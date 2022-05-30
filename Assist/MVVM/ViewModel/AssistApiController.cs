using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using AutoUpdaterDotNET;
using RestSharp;
using Assist.MVVM.Model;
using System.IO;
using System.Net;
using System.Diagnostics;
using Serilog;

namespace Assist.MVVM.ViewModel
{
    internal class AssistApiController
    {
        private const string baseUrl = "https://api.assistapp.dev";
        private const string valUrl = $"{baseUrl}/valorant/";
        private const string dataUrl = $"{baseUrl}/data/";
        private const string updateUrl = $"{dataUrl}update";
        private const string newsUrl = $"{valUrl}news";
        private const string bundleUrl = $"{valUrl}bundles/";
        private const string offerUrl = $"{valUrl}offers/";
        private const string maintUrl = $"{dataUrl}status/maintenance";
        private const string battlepassUrl = $"{valUrl}battlepass/";

        private const string bgclientData = $"{dataUrl}bgclient/data";

        public const string currentBattlepassId = "d80f3ef5-44f5-8d70-6935-f2840b2d3882";
        internal bool bIsUpdate = false;
        public RestClient client = new RestClient();


        public async Task CheckForAssistUpdates()
        {
            Log.Information("Checking for Assist Updates");
            AutoUpdater.ParseUpdateInfoEvent += ParseUpdateData;
            AutoUpdater.CheckForUpdateEvent += CheckForUpdate;
            try
            {
               AutoUpdater.Start(updateUrl);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            
        }
        private void CheckForUpdate(UpdateInfoEventArgs args)
        {
            if (args.Error is WebException)
            {
                Log.Information("Error on WebException");
                return;
            }

            var currentVersion = new Version(args.CurrentVersion);

            if (currentVersion == args.InstalledVersion)
                //Update isnt needed as Versions of both the latest from update api and local version are the same.
                return;


            if (args.InstalledVersion > currentVersion)
                return;

            OpenUpdateScreen(args);
        }
        private void ParseUpdateData(ParseUpdateInfoEventArgs args)
        {
            var updateData = JsonSerializer.Deserialize<UpdateData>(args.RemoteData);
            if(updateData != null)
            {
                args.UpdateInfo = new UpdateInfoEventArgs
                {
                    CurrentVersion = updateData.updateVersion,
                    ChangelogURL = updateData.updateChangelog,
                    DownloadURL = updateData.updateUrl,
                    Mandatory =
                    {
                        Value = updateData.mandatory.value,
                        MinimumVersion = updateData.mandatory.minVersion,
                        UpdateMode = Mode.Forced
                    }
                };
            }
        }
        private void OpenUpdateScreen(UpdateInfoEventArgs args)
        {
            bIsUpdate = true;
            var temp = Application.Current.MainWindow;
            temp.Visibility = Visibility.Hidden;
            new View.Extra.UpdateWindow(args).ShowDialog();
            temp.Close();
        }

        public async Task<List<AssistNewsObj>> GetAssistNews()
        {
            var resp = await client.ExecuteAsync(new RestRequest(newsUrl), Method.Get);

            if (resp.IsSuccessful)
            {
                return JsonSerializer.Deserialize<List<AssistNewsObj>>(resp.Content);
            }
            else
            {
                var defaultError = new AssistNewsObj()
                {
                    NewsTitle = "Assist: Error Getting Articles",
                    NewsDescription = "Yea.. so articles werent found.",
                    NewsImage = "https://i.kym-cdn.com/entries/icons/original/000/037/349/Screenshot_14.jpg",
                    NewsUrl = "assistapp.dev"
                };

                return new List<AssistNewsObj>()
                {
                    defaultError
                };
            }
        }
        public async Task<AssistBundleObj> GetBundleObj(string dataAssetId)
        {
            var resp = await client.ExecuteAsync<AssistBundleObj>(new RestRequest(bundleUrl + dataAssetId), Method.Get);

            if (resp.IsSuccessful)
            {
                 return JsonSerializer.Deserialize<AssistBundleObj>(resp.Content);
            }
            else
            {
                return new()
                {
                    BundleName = "Could Not Find Bundle on Server",
                    Description = "Please Contact Mike to fix this issue.",
                    DisplayIcon = "https://cdn.rumblemike.com/Bundles/2116a38e-4b71-f169-0d16-ce9289af4bfa_DisplayIcon.png"
                };
            }
        }
        public async Task<AssistSkin> GetSkinObj (string dataAssetId)
        {
            var resp = await client.ExecuteAsync<AssistSkin>(new RestRequest(valUrl + "skins/" + dataAssetId), Method.Get);

            if (resp.IsSuccessful)
                return JsonSerializer.Deserialize<AssistSkin>(resp.Content);
            else
                return new()
                {
                    DisplayName = "Could Not Find Skin on Server",
                    DisplayIcon = "https://cdn.rumblemike.com/Skins/2116a38e-4b71-f169-0d16-ce9289af4bfa_DisplayIcon.png"
                };
        }
        public async Task<string> GetSkinPricing(string dataAssetId)
        {
            var resp = await client.ExecuteAsync(new RestRequest(offerUrl + dataAssetId), Method.Get);

            if (resp.IsSuccessful)
                return string.Format("{0:n0}", JsonSerializer.Deserialize<OfferObj>(resp.Content).Cost.VpCost);
            else
                return string.Format("{0:n0}", 99999);
        }
        public async Task<AssistMaintenanceObj> GetMaintenanceStatus()
        {
            Log.Information("Checking for Maintenance");
            var resp = await client.ExecuteAsync(new RestRequest(maintUrl), Method.Get);

            if (resp.IsSuccessful)
                return JsonSerializer.Deserialize<AssistMaintenanceObj>(resp.Content);
            else
                return new() { DownForMaintenance = false, DownForMaintenanceMessage = "Assist is currently down for Maintenance. Please come back later. Check out the discord for information regarding the Maintenance."};
        }
        public async Task<BattlePassObj> GetBattlepassData()
        {
            var resp = await new RestClient().ExecuteAsync(new RestRequest(battlepassUrl + currentBattlepassId, Method.Get));

            if (resp.IsSuccessful)
                return JsonSerializer.Deserialize<BattlePassObj>(resp.Content);
            else
                return null;
        }

        public async Task<BgClientObj> GetBgClientData()
        {
            var resp = await new RestClient().ExecuteAsync(new RestRequest(bgclientData, Method.Get));

            if (resp.IsSuccessful)
                return JsonSerializer.Deserialize<BgClientObj>(resp.Content);
            else
                return null;
        }
    }
}
