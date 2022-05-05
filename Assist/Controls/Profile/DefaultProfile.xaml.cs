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
using Assist.MVVM.ViewModel;
using Assist.Settings;

namespace Assist.Controls.Profile
{
    /// <summary>
    /// Interaction logic for DefaultProfile.xaml
    /// </summary>
    public partial class DefaultProfile : UserControl
    {
        public DefaultProfile()
        {
            InitializeComponent();
        }

        private void DefaultAcc_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var Profile in AssistSettings.Current.Profiles)
            {
                AccountComboBox.Items.Add(new ComboBoxItem()
                {
                    Content = Profile.RiotId,
                    Tag = Profile.ProfileUuid
                });

                if (AssistSettings.Current.DefaultAccount == Profile.ProfileUuid)
                {
                    AccountComboBox.SelectedIndex = AccountComboBox.Items.Count - 1;
                }
            }

            AccountComboBox.SelectionChanged += AccountComboBoxOnSelectionChanged;
            if (string.IsNullOrEmpty(AssistSettings.Current.DefaultAccount))
            {
                AccountComboBox.SelectedIndex = AccountComboBox.Items.Count - 1;
            }
        }

        private void AccountComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currItem = AccountComboBox.Items[AccountComboBox.SelectedIndex];
            if (currItem != null)
            {
                var item = currItem as ComboBoxItem;
                if (AssistSettings.Current.DefaultAccount == item.Tag.ToString())
                    return;

                AssistSettings.Current.DefaultAccount = item.Tag.ToString();
            }
        }
    }
}
