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
using Assist.MVVM.View.Progression.Sectors;

namespace Assist.MVVM.View.Progression
{
    /// <summary>
    /// Interaction logic for Progression.xaml
    /// </summary>
    public partial class Progression : Page
    {
        public Progression()
        {
            InitializeComponent();
        }

        private void BattlepassCurrentButton_Click(object sender, RoutedEventArgs e)
        {
            AssistMainWindow.Current.ContentFrame.Navigate(new ProgressionBattlepass());
        }
        private void BattlepassEP5ACT1Button_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            AssistMainWindow.Current.ContentFrame.Navigate(new ProgressionBattlepass("99ac9283-4dd3-5248-2e01-8baf778affb4"));
        }

        private void BattlepassEP4ACT3Button_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            AssistMainWindow.Current.ContentFrame.Navigate(new ProgressionBattlepass("d80f3ef5-44f5-8d70-6935-f2840b2d3882"));
        }
    }
}
