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
        public static TransitioningContentControl ContentControl = new TransitioningContentControl();
        public static void SpawnPopup(PopupSettings settings)
        {
            var popup = new BasicPopup();

            if (ContentControl != null)
            {
                Log.Information("Spawning popup on Main Window");
                ContentControl.Content = (popup);
            }
        }

        public static async void KillPopups()
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (PopupController != null)
                {
                    Log.Information("Killing popups on popupcontrioller on Main Window");
                    PopupController.Children.Clear();
                }
                
                
                if (ContentControl != null)
                {
                    Log.Information("Killing popup on Main Window");
                    ContentControl.Content = null;
                }
            });

        }
        public static void cha(UserControl control)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                control.BeginInit();
                PopupController.Children.Add(control);
            });
        }
        
        public static void SpawnCustomPopup(UserControl c) => ContentControl.Content = c;
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
