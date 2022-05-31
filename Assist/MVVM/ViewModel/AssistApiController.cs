using Assist.MVVM.Model;

using AutoUpdaterDotNET;

using Serilog;

using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Assist.MVVM.ViewModel
{
    // todo: move the other methods to a different class
    internal class AssistApiController
    {

        private const string BaseUrl = "https://api.assistapp.dev";
        private const string DataUrl = $"{BaseUrl}/data/";
        private const string UpdateUrl = $"{DataUrl}update";

        public const string currentBattlepassId = "d80f3ef5-44f5-8d70-6935-f2840b2d3882";
        internal bool bIsUpdate = false;

        public async Task CheckForAssistUpdates()
        {
            Log.Information("Checking for Assist Updates");

            AutoUpdater.ParseUpdateInfoEvent += ParseUpdateData;
            AutoUpdater.CheckForUpdateEvent += CheckForUpdate;

            try
            {
               AutoUpdater.Start(UpdateUrl);
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

    }
}
