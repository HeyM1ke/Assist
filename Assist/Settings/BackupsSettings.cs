using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assist.Objects.RiotClient;
using Serilog;

namespace Assist.Settings
{
    public class BackupsSettings
    {
        public static string BackupsFolderPath = Path.Combine(AssistSettings.SettingsFolderPath, "Backups");
        public BackupsSettings()
        {
            Directory.CreateDirectory(BackupsFolderPath);
        }

        public static void SaveBackup(BackupModel model)
        {
            string[] folders = System.IO.Directory.GetDirectories(BackupsFolderPath,"*", System.IO.SearchOption.AllDirectories);

            var p = Path.Combine(BackupsFolderPath, model.PlayerUuid);
            Directory.CreateDirectory(p);
            
            CopyFiles(model.ConfigFolderPath, p); // Copies Riot Files to Backups Folder

        }

        private static void CopyFiles(string sDir, string nDir)
        {
            foreach(var file in Directory.GetFiles(sDir))
                File.Copy(file, Path.Combine(nDir, Path.GetFileName(file)), true);
        }

        private static void CopySettings(BackupModel model)
        {
            // Copy Settings to backup
            
        }

    }
}