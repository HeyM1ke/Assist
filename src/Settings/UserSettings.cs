using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Assist.MVVM.Model;
using System.Net;
using System.Diagnostics;

namespace Assist.Settings
{
    /// <summary>
    /// Class that contains UserSettings for Computer
    /// </summary>
    internal class UserSettings
    {
        private const int ACCOUNTLIMIT = 5;
        private const string _riotClientDownloadUrl = "https://valorant.secure.dyn.riotcdn.net/channels/public/x/installer/current/live.live.na.exe";

        public static string SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assist", "Settings.json");
        // Instance to get user settings from everywhere.
        public static UserSettings Instance { get; set; }
        // Stores RiotClient Install Path for PC
        public string RiotClientInstallPath { get; set; }
        public List<AccountSettings> Accounts {  get; set; }
        public LaunchSettings LaunchSettings { get; set; }
        public string DefaultAccount { get; set; }
        static UserSettings()
        {
            Instance = new UserSettings()
            {
                Accounts = new List<AccountSettings>(ACCOUNTLIMIT),
                LaunchSettings = new LaunchSettings()
            };
        }
        public void Save()
        {
            var settingsJson = JsonSerializer.Serialize(Instance);
            File.WriteAllText(SettingsFilePath, settingsJson);
        }
        internal async Task<string> FindRiotClientPath(){
            string path = "";

            string riotInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Riot Games/RiotClientInstalls.json"); ;

            if (File.Exists(riotInstallPath))
            {
                var config = JsonDocument.Parse(File.ReadAllText(riotInstallPath));

                

                if (config.RootElement.TryGetProperty("rc_live", out JsonElement rcLive))
                {
                    path = rcLive.GetString();
                }

                if (config.RootElement.TryGetProperty("rc_beta", out JsonElement rcBeta))
                {
                    path = rcBeta.GetString();
                }

                if (config.RootElement.TryGetProperty("rc_default", out JsonElement rcDefault))
                {
                    path = rcDefault.GetString();
                }
            }

            if(File.Exists(path))
                return path;
            else
                return null;
        }


        internal AccountSettings FindAccountById(string id)
        {
            var account = Instance.Accounts.Find(account => account.puuid == id);
            if (account == null)
                return null;
            
            return account;
        }
        internal AccountSettings FindAccountByGameNameTagLine(string gamename, string tagline)
        {
            var account = Instance.Accounts.Find(account => account.Gamename == gamename && account.Tagline == tagline);
            if (account == null)
                return null;

            return account;
        }
        internal AccountSettings FindAccountByGameNameTagLine(string gamenametag)
        {
            var account = Instance.Accounts.Find(account => $"{account.Gamename}#{account.Tagline}" == gamenametag);
            if (account == null)
                return null;

            return account;
        }
    }
}
