using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace Assist.Game.Services
{
    internal class GameViewNavigationController
    {
        public static Page CurrentPage;
        public static TransitioningContentControl ContentControl = new TransitioningContentControl();

        public static void Change(UserControl c) => ContentControl.Content = c;
    }

    enum Page
    {
        DASHBOARD,
        LOBBIES,
        LIVE,
        MODULES,
        MATCH,
        LEAGUES,
        ENDORSE
    }
}
