using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Models.Dodge;
using Assist.Settings;
using Serilog;

namespace Assist.Game.Services
{
    internal class DodgeService
    {
        public List<DodgeUser> UserList = new List<DodgeUser>() { };
        // Add 
        // Remove
        // Refresh
        // Update Usernames
        public static DodgeService Current;
        public static string DodgeFolderPath = Path.Combine(AssistSettings.SettingsFolderPath, "Game", "Modules", "Dodge");
        public static string DodgeSettingsPath = Path.Combine(DodgeFolderPath, "Settings.json");
        public Action<DodgeUser> DodgeUserAddedToList;
        public Action<DodgeUser> DodgeUserRemovedFromList;
        public DodgeService()
        {
            Current = this;
            Directory.CreateDirectory(DodgeFolderPath);
            LoadDodgeList();
        }

        private void LoadDodgeList()
        {

            try
            {
                var settingsContent = File.ReadAllText(DodgeSettingsPath);
                Current.UserList = JsonSerializer.Deserialize<List<DodgeUser>>(settingsContent);
                Log.Information("Successfully read the Dodge settings file");
            }
            catch
            {
                Log.Error("Dodge Settings File was not found or tampered with.");
            }
        }

        public void AddUser(DodgeUser user)
        {
            UserList.Add(user);
            DodgeUserAddedToList?.Invoke(user);
            SaveSettings();
        }

        public static void SaveSettings()
        {
            File.WriteAllText(DodgeSettingsPath, JsonSerializer.Serialize(Current.UserList, new JsonSerializerOptions() { WriteIndented = true }), Encoding.UTF8);
        }

        public void RemoveUser(DodgeUser user)
        {
            UserList.Remove(user);
            DodgeUserRemovedFromList?.Invoke(user);
            SaveSettings(); 
        }

    }
}
