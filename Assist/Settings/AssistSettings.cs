using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Objects.Enums;
using Assist.ViewModels;
using Avalonia.Platform;
using DynamicData;
using ReactiveUI;
using Serilog;
using Serilog.Core;

namespace Assist.Settings
{
    internal class AssistSettings : INotifyPropertyChanged
    {
        
        public static string SettingsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AssistX");
        public static string ResourcesFolderPath = Path.Combine(SettingsFolderPath, "Resources");
        public AssistSettings()
        {
            Directory.CreateDirectory(ResourcesFolderPath);
        }

        public string ApplicationVersion => $"V{Process.GetCurrentProcess().MainModule.FileVersionInfo.FileVersion}";
        public const int maxAccountCount = 5;
        public static AssistSettings Current { get; set; } = new AssistSettings();
        public string RiotClientInstallPath { get; set; }

        private bool _languageSelected = false;

        public bool LanguageSelected
        {
            get => _languageSelected;
            set => this.SetProperty(ref _languageSelected, value);
        }



        private ELanguage _language;
        public ELanguage Language { get => _language; set => this.SetProperty(ref _language, value); }

        private EResolution _resolution = EResolution.R720;
        public EResolution SelectedResolution
        {
            get => _resolution;
            set => this.SetProperty(ref _resolution, value);
        }

        private List<ProfileSettings> _profiles = new List<ProfileSettings>();
        public List<ProfileSettings> Profiles
        {
            get => _profiles;
            set => this.SetProperty(ref _profiles, value);
        }

        private string _defaultAccount;
        public string DefaultAccount
        {
            get => _defaultAccount;
            set => this.SetProperty(ref _defaultAccount, value);
        }

        private string _assistUserCode;
        public string AssistUserCode
        {
            get => _assistUserCode;
            set => this.SetProperty(ref _assistUserCode, value);
        }

        internal async Task<string> FindRiotClient()
        {
            if (AssistApplication.Current.Platform.OperatingSystem != OperatingSystemType.WinNT)
                return null;

            List<string> clients = new List<string>();

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
                Log.Error("Riot Client Check: Could not properly parse json file");
                return null;
            }



            
            if (config.RootElement.TryGetProperty("rc_live", out JsonElement rcLive)) { clients.Add(rcLive.GetString()); }
            if (config.RootElement.TryGetProperty("rc_beta", out JsonElement rcBeta)) { clients.Add(rcBeta.GetString()); }
            if (config.RootElement.TryGetProperty("rc_default", out JsonElement rcDefault)) { clients.Add(rcDefault.GetString()); }

            foreach (var clientPath in clients)
            {
                if (File.Exists(clientPath))
                    return clientPath;
            }

            return null;
        }

        internal async Task SaveProfile(ProfileSettings profile)
        {
            var possProfile = Profiles.Find(p => p.ProfileUuid == profile.ProfileUuid);

            if (possProfile == null)
            {
                Log.Information("New Profile Found, Adding profile to Profiles");
                Profiles.Add(profile);
                return;
            }

            Log.Information("Previous Profile Found, Refreshing Profile");
            Profiles.Replace(possProfile, profile);
        }

        public static void Save()
        {
            File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(Current, new JsonSerializerOptions() { WriteIndented = true }), Encoding.UTF8);
        }

#if DEBUG
        public static string SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AssistX", "Settings_DEBUG.json");
#else
        public static string SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AssistX", "Settings.json");
#endif
        

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }

        private bool _gameModeEnabled = true;

        public bool GameModeEnabled
        {
            get => _gameModeEnabled;
            set => this.SetProperty(ref _gameModeEnabled, value);
        }
        public BackupsSettings Backups = new BackupsSettings();
    }
}
