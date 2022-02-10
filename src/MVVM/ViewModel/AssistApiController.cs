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

namespace Assist.MVVM.ViewModel
{
    internal class AssistApiController
    {
        private const string updateUrl = "https://assist.rumblemike.com/Update";
        private const string statusUrl = "https://assist.rumblemike.com/Status";
        private const string newsUrl = "https://assist.rumblemike.com/News/FeaturedNews";
        private const string bundleUrl = "https://assist.rumblemike.com/Bundle/";
        private const string skinUrl = "https://assist.rumblemike.com/Skins/";
        private const string offerUrl = "https://assist.rumblemike.com/Offers/";
        private const string maintUrl = "https://assist.rumblemike.com/prod/maintenance/status";
        internal bool bIsUpdate = false;
        public RestClient client = new RestClient();
        public void CheckForAssistUpdates()
        {
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
                    CurrentVersion = updateData.version,
                    ChangelogURL = updateData.changelog,
                    DownloadURL = updateData.url,
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
            Trace.WriteLine("ran?");
        }

        public async Task<List<News>> GetAssistNews()
        {
            var resp = await client.ExecuteAsync(new RestRequest(newsUrl), Method.Get);

            if (resp.IsSuccessful)
            {
                return JsonSerializer.Deserialize<List<News>>(resp.Content);
            }
            else
            {
                var defaultError = new News
                {
                    title = "Assist: Error Getting Articles",
                    description = "Yea.. so articles werent found.",
                    featureimage = "https://i.kym-cdn.com/entries/icons/original/000/037/349/Screenshot_14.jpg",
                    newslink = "google.com"
                };

                return new List<News>()
                {
                    defaultError
                };
            }
        }
        public async Task<List<StatusMsg>> GetStatusMessages()
        {
            var resp = await client.ExecuteAsync(new RestRequest(statusUrl), Method.Get);

            if (resp.IsSuccessful)
            {
                return JsonSerializer.Deserialize<List<StatusMsg>>(resp.Content);
            }
            else
            {
                return new List<StatusMsg>
                {
                    new StatusMsg
                    {
                        statusMessage = "Could Not Reach Status API"
                    }
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
                    bundleDisplayName = "Could Not Find Bundle on Server",
                    bundleDescription = "Please Contact Mike to fix this issue.",
                    bundleDisplayIcon = "https://cdn.rumblemike.com/Bundles/2116a38e-4b71-f169-0d16-ce9289af4bfa_DisplayIcon.png"
                };
            }
        }
        public async Task<SkinObj> GetSkinObj (string dataAssetId)
        {
            var resp = await client.ExecuteAsync<SkinObj>(new RestRequest(skinUrl + dataAssetId), Method.Get);

            if (resp.IsSuccessful)
                return JsonSerializer.Deserialize<SkinObj>(resp.Content);
            else
                return new()
                {
                    displayName = "Could Not Find Skin on Server",
                    displayIcon = "https://cdn.rumblemike.com/Bundles/2116a38e-4b71-f169-0d16-ce9289af4bfa_DisplayIcon.png"
                };
        }
        public async Task<string> GetSkinPricing(string dataAssetId)
        {
            var resp = await client.ExecuteAsync(new RestRequest(offerUrl + dataAssetId), Method.Get);

            if (resp.IsSuccessful)
                return string.Format("{0:n0}", JsonSerializer.Deserialize<OfferObj>(resp.Content).cost.valorantPointCost);
            else
                return string.Format("{0:n0}", 99999);
        }
        public async Task<AssistMaintenanceObj> GetMaintenanceStatus()
        {
            var resp = await client.ExecuteAsync(new RestRequest(maintUrl), Method.Get);

            if (resp.IsSuccessful)
                return JsonSerializer.Deserialize<AssistMaintenanceObj>(resp.Content);
            else
                return new() { bDownForMaintenance = false };
        }
    }
}
