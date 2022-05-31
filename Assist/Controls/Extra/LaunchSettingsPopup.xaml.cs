using System;
using System.Collections.Generic;
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
using Assist.Modules.Popup;
using Assist.MVVM.ViewModel;
using Assist.Settings;
using Serilog;
using ValNet.Objects.Authentication;

namespace Assist.Controls.Extra
{
    /// <summary>
    /// Interaction logic for LaunchSettingsPopup.xaml
    /// </summary>
    public partial class LaunchSettingsPopup : UserControl
    {
        List<PatchlineObj> patchlines = new List<PatchlineObj>();
        public LaunchSettingsPopup()
        {
            InitializeComponent();
            DiscordCheckBox.IsChecked = AssistSettings.Current.LaunchSettings.ValDscRpcEnabled;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DiscordCheckBox.IsChecked is null || DiscordCheckBox.IsChecked is false)
                AssistSettings.Current.LaunchSettings.ValDscRpcEnabled = false;
            else
                AssistSettings.Current.LaunchSettings.ValDscRpcEnabled = true;

            PopupSystem.KillPopups();
        }

        private void PatchlineComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var patchline in AssistApplication.AppInstance.CurrentProfile.entitlements)
            {
                PatchlineComboBox.Items.Add(new ComboBoxItem()
                {
                    Content = patchline.PatchlineName
                });
            }
            PatchlineComboBox.SelectedIndex = 0;
        }

        private void PatchlineComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AssistSettings.Current.LaunchSettings.ValPatchline = AssistApplication.AppInstance.CurrentProfile
                .entitlements[PatchlineComboBox.SelectedIndex].PatchlinePath;
        }
    }
}
