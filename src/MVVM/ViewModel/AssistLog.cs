using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.MVVM.ViewModel
{
    public class AssistLog
    {
        string logPath;
        public AssistLog()
        {
            // Create Log file
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assist", "Logs"));
            logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assist", "Logs", $"Assist_Log.txt");
            File.CreateText(logPath).Dispose();
        }

        public void Normal(string message)
        {
            WriteToLog("[NORMAL] " + message);
        }
        public void Error(string message)
        {
            WriteToLog("[ERROR] " + message);
        }

        public void Debug(string message)
        {
            WriteToLog("[DEBUG] " + message);
        }

        private void WriteToLog(string message)
        {
            using (StreamWriter sw = new StreamWriter(logPath, append: true))
            {
                sw.WriteLine($"[{DateTime.Now.ToString()}] : {message}");
            }
        }
    }
}
