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
using Assist.Controls.Profile;
using Assist.Settings;

namespace Assist.MVVM.View.Profiles
{
    /// <summary>
    /// Interaction logic for Profiles.xaml
    /// </summary>
    public partial class Profiles : Page
    {
        public Profiles()
        {
            InitializeComponent();
        }

        private void Profiles_Loaded(object sender, RoutedEventArgs e)
        {


            foreach (var Profile in AssistSettings.Current.Profiles)
            {
                var ProfileShowcaseControl = new ProfileShowcase(Profile)
                {
                    Margin = new Thickness(5, 0, 0, 0)
                };
                ProfilePanel.Children.Add(ProfileShowcaseControl);
            }

            if (AssistSettings.Current.Profiles.Count < AssistSettings.maxAccountCount)
                ProfilePanel.Children.Add(new ProfileAdd()
                {
                    Margin = new Thickness(5,0,0,0)
                });
        }
    }
}
