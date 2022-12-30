using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Objects.Discord;
using Assist.ViewModels;
using ReactiveUI;

namespace Assist.Settings
{
    public class GameSettings : ViewModelBase
    {
        public static string SettingsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AssistX", "Game");
        public static GameSettings Current { get; set; } = new GameSettings();
        public GameSettings()
        {
            Directory.CreateDirectory(SettingsFolderPath);
        }

        public static void Save()
        {
            File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(Current, new JsonSerializerOptions() { WriteIndented = true }), Encoding.UTF8);
        }

#if DEBUG
        public static string SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AssistX", "Game", "Settings_DEBUG.json");
#else
        public static string SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AssistX", "Game", "Settings.json");
#endif

        private bool _discordPresenceEnabled = false;

        public bool DiscordPresenceEnabled
        {
            get => _discordPresenceEnabled;
            set => this.RaiseAndSetIfChanged(ref _discordPresenceEnabled, value);
        }


        public RichPresenceModel RichPresenceSettings { get; set; } = new RichPresenceModel();

    }
}
