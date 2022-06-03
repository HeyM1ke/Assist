using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Assist.MVVM.View.Selector;
using Serilog;

namespace Assist.Modules.Popup
{
    public class PopupSystem
    {
        private static Controls.Extra.Popup popup;
        public static void SpawnPopup(PopupSettings settings)
        {
            popup = new Controls.Extra.Popup(settings);
            

            if (AssistMainWindow.PopupContainer != null)
            {
                Log.Information("Spawning popup on Main Window");
                AssistMainWindow.PopupContainer.Children.Add(popup);
            }

            if (Startup.Container != null)
            {
                Log.Information("Spawning popup on Startup");
                Startup.Container.Children.Add(popup);
            }
        }

        public static void KillPopups()
        {
            if (AssistMainWindow.PopupContainer != null)
            {
                Log.Information("Killing popup on Main Window");
                AssistMainWindow.PopupContainer.Children.Clear();
            }

            if (Startup.Container != null)
            {
                Log.Information("Killing popup on Startup");
                Startup.Container.Children.Clear();
            }
        }

        public static void ModifyCurrentPopup(PopupSettings settings)
        {
            popup._viewModel.UpdateSettings(settings);
        }

        public static void SpawnCustomPopup(UserControl control)
        {
            AssistMainWindow.PopupContainer.Children.Add(control);
        }

    }
}
