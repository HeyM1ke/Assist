using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Assist.Modules.Popup
{
    public class PopupSystem
    {
        private static Controls.Extra.Popup popup;

        public static void SpawnPopup(PopupSettings settings)
        {
            popup = new Controls.Extra.Popup(settings);
            AssistMainWindow.PopupContainer.Children.Add(popup);
        }

        public static void KillPopups()
        {
            AssistMainWindow.PopupContainer.Children.Clear();
        }

        public static void ModifyCurrentPopup(PopupSettings settings)
        {
            popup._viewModel.PopupSettings = settings;
        }

        public static void SpawnCustomPopup(UserControl control)
        {
            AssistMainWindow.PopupContainer.Children.Add(control);
        }

    }
}
