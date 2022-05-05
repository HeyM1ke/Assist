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

        private void BattlepassButton_Click(object sender, RoutedEventArgs e)
        {
            AssistMainWindow.Current.ContentFrame.Navigate(new Uri("/MVVM/View/Progression/Sectors/ProgressionBattlepass.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
