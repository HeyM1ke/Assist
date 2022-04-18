using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Assist.Controls.Extra;
using Assist.Modules.Popup;

namespace Assist.MVVM.View.Dashboard
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        private const string discordUrl = "https://discord.gg/B43EndmEgW";
        public Dashboard()
        {
            InitializeComponent();
        }

        private void Support_Btn_Click(object sender, RoutedEventArgs e)
        {
            PopupSystem.SpawnCustomPopup(new SupportPopup());

        }

        private void Discord_Btn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = discordUrl,
                UseShellExecute = true
            });
        }
    }
}
