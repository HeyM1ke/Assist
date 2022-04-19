using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.MVVM.Model;
using Assist.MVVM.ViewModel;

namespace Assist.Settings
{
    internal class AssistSettings : ViewModelBase
    {
        public const int MAX_ACCOUNT_COUNT = 5;
        public static AssistSettings Current { get; set; }
        public string RiotClientInstallPath { get; set; }
#if DEBUG
        public static string SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assist", "AssistSettings_DEBUG.json");
#else
        public static string SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assist", "AssistSettings.json");
#endif


        static AssistSettings()
        {
            Current = new();
        }
        private LaunchSettings _launchSettings = new();
        public LaunchSettings LaunchSettings
        {
            get => _launchSettings;
            set => SetProperty(ref _launchSettings, value);
        }
        private List<ProfileSetting> _profiles = new();
        public List<ProfileSetting> Profiles
        {
            get => _profiles;
            set => SetProperty(ref _profiles, value);
        }
        private string _defaultAccount;
        public string DefaultAccount
        {
            get => _defaultAccount;
            set => SetProperty(ref _defaultAccount, value);
        }
        private bool _bNewUser = true;
        public bool bNewUser
        {
            get => _bNewUser;
            set => SetProperty(ref _bNewUser, value);
        }

        private Enums.EWindowSize _resolution = Enums.EWindowSize.R576;
        public Enums.EWindowSize Resolution {
            get => _resolution;
            set => SetProperty(ref _resolution, value);
        }

        private Enums.ELanguage _language = 0;
        public Enums.ELanguage Language
        {
            get => _language;
            set => SetProperty(ref _language, value);
        }


        private double _soundVolume = 0.5;
        public double SoundVolume { get => _soundVolume; set => SetProperty(ref _soundVolume, value); }

        #region Settings Methods

        /// <summary>
        /// Saves Settings to PC.
        /// </summary>
        public static void Save()
        {
            File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(Current, new JsonSerializerOptions() { WriteIndented = true }));
        }

        /// <summary>
        /// Finds Riot Client on Local Machine
        /// </summary>
        /// <returns>Path to Client</returns>
        internal async Task<string> FindRiotClient()
        {
            List<string> clients = new();

            string riotInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Riot Games/RiotClientInstalls.json"); ;

            if (!File.Exists(riotInstallPath)) return null;

            JsonDocument config;
            try
            {
                config = JsonDocument.Parse(File.ReadAllText(riotInstallPath));
            }
            catch (Exception e)
            {
                AssistLog.Error("Riot Client Check: Could not properly parse json file");
                return null;
            }

                

            if (config.RootElement.TryGetProperty("rc_default", out JsonElement rcDefault)) { clients.Add(rcDefault.GetString()); }
            if (config.RootElement.TryGetProperty("rc_live", out JsonElement rcLive)) { clients.Add(rcLive.GetString()); }
            if (config.RootElement.TryGetProperty("rc_beta", out JsonElement rcBeta)) { clients.Add(rcBeta.GetString()); }
            

            foreach (var clientPath in clients)
            {
                if (File.Exists(clientPath))
                    return clientPath;
            }
            
            return null;
        }

        /// <summary>
        /// Finds Profile Setting by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A ProfileSetting Instance if found.</returns>
        internal ProfileSetting FindProfileById(string id)
        {
            var profile = Profiles.Find(p => p.ProfileUuid == id);
            if (profile == null)
                return null;

            return profile;
        }

        /// <summary>
        /// Finds Profile Setting by Gamename and Tagline Separated
        /// </summary>
        /// <param name="gamename"></param>
        /// <param name="tagline"></param>
        /// <returns>A ProfileSetting Instance if found.</returns>
        internal ProfileSetting FindAccountByGameNameTagLine(string gamename, string tagline)
        {
            var profile = Profiles.Find(p => p.Gamename == gamename && p.Tagline == tagline);
            if (profile == null)
                return null;

            return profile;
        }

        /// <summary>
        /// Finds Profile Setting by combined Gamename and Tagline
        /// </summary>
        /// <param name="gamenametag"></param>
        /// <returns>A ProfileSetting Instance if found.</returns>
        internal ProfileSetting FindAccountByGameNameTagLine(string gamenametag)
        {
            var profile = Profiles.Find(p => $"{p.Gamename}#{p.Tagline}" == gamenametag);
            if (profile == null)
                return null;

            return profile;
        }

        #endregion
    }
}
