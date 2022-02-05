using Assist.MVVM.ViewModel;
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

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistLaunchControl.xaml
    /// </summary>
    public partial class AssistLaunchControl : UserControl
    {
        AssistApplication _viewModel => AssistApplication.AppInstance;

        public AssistLaunchControl()
        {
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void PatchlineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.LaunchControlViewModel.currentSelectedPatchline = PatchlineComboBox.SelectedIndex;
        }

        private async void LaunchControl_Initialized(object sender, EventArgs e)
        {
            accNameLabel.Content = $"{_viewModel.currentAccount.Gamename}#{_viewModel.currentAccount.Tagline}";

            await _viewModel.LaunchControlViewModel.GetUserPatchlines();

            PatchlineComboBox.SelectedIndex = 0;

            foreach (var patchline in _viewModel.LaunchControlViewModel.entitledPatchlines)
            {
                PatchlineComboBox.Items.Add(new ComboBoxItem()
                {
                    Content = patchline.PatchlineName
                });
            }
            // Ask Viewmodel to get player entitlements 
            
            // Set entitlements to combo
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LaunchControlViewModel.LaunchClient();
        }
    }
}
