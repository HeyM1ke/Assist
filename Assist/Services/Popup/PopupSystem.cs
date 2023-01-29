using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.X11;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Controls.Global.Popup;
using Avalonia.Threading;

namespace Assist.Services.Popup
{
    public class PopupSystem
    {
        public static PopupMaster PopupController;

        public static void SpawnPopup(PopupSettings settings)
        {
            var popup = new BasicPopup();

            if (PopupController != null)
            {
                Log.Information("Spawning popup on Main Window");
                PopupController.Children.Add(popup);
            }
        }

        public static void KillPopups()
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (PopupController != null)
                {
                    Log.Information("Killing popup on Main Window");
                    PopupController.Children.Clear();
                }
            });

        }

        public static void ModifyCurrentPopup(PopupSettings settings)
        {
            
        }

        public static void SpawnCustomPopup(UserControl control)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                PopupController.Children.Add(control);
            });
        }
    }

    public class PopupSettings
    {
        public string PopupTitle { get; set; }
        public string PopupDescription { get; set; }

        public PopupType PopupType { get; set; }
    }

    public enum PopupType
    {
        OK,
        LOADING,
        ERROR
    }
}
