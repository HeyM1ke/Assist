using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace Assist.Services
{
    internal class MainWindowContentController
    {
        public static TransitioningContentControl ContentControl = new TransitioningContentControl();

        public static void Change(UserControl c) => ContentControl.Content = c;
    }
}
