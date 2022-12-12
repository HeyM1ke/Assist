using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.Services
{
    internal class SettingsViewNavigationController
    {
        public static SettingsPages CurrentPage;
        public static TransitioningContentControl ContentControl = new TransitioningContentControl();
        public static void Change(UserControl c) => ContentControl.Content = c;
    }

    enum SettingsPages
    {
        UNKNOWN,
        GENERAL,
        ACCOUNTS,
        ASSISTSET,
        INFORMATION,
    }
}
