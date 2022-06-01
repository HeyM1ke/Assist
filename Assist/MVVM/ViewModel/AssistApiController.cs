using Assist.MVVM.Model;

using AutoUpdaterDotNET;

using Serilog;

using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
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

        public void CheckForAssistUpdates()
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
            var exception = args.Error;
            if (exception is WebException) // todo: can other exceptions occur?
            {
                Log.Error(exception, "Exception while checking for application updates.");
                return;
            }

            var installedVersion = args.InstalledVersion;
            var currentVersion = new Version(args.CurrentVersion);
            Log.Information("Current version: {CurrentVersion}", currentVersion.ToString());
            Log.Information("Installed: {InstalledVersion}", installedVersion.ToString());

            var hasLatestVersion = installedVersion == currentVersion;
            var hasHigherVersion = installedVersion > currentVersion;
            if (hasLatestVersion || hasHigherVersion)
                return;

            OpenUpdateScreen(args);
        }

        private static void ParseUpdateData(ParseUpdateInfoEventArgs args)
        {
            var content = args.RemoteData;
            var data = JsonSerializer.Deserialize<UpdateData>(content);
            if (data == null)
            {
                Log.Error("Failed to deserialize the update data while checking for application updates. Content: {Content}", 
                    content);
                return;
            }

            args.UpdateInfo = new UpdateInfoEventArgs
            {
                CurrentVersion = data.updateVersion,
                ChangelogURL = data.updateChangelog,
                DownloadURL = data.updateUrl,
                Mandatory =
                {
                    Value = data.mandatory.value,
                    MinimumVersion = data.mandatory.minVersion,
                    UpdateMode = Mode.Forced
                }
            };
        }

        /*
         * internal class UpdateData
    {
        public string updateUrl { get; set; }
        public string updateVersion { get; set; }
        public string updateChangelog { get; set; }
        public Mandatory mandatory { get; set; }

        public class Mandatory
        {
            public bool value { get; set; }
            public string minVersion { get; set; }
            public int mode { get; set; }
        }
    }
         */

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
