using Assist.MVVM.ViewModel;
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

namespace Assist.MVVM.View.ProgressionPage
{
    /// <summary>
    /// Interaction logic for ProgressionPage.xaml
    /// </summary>
    public partial class ProgressionPage : Page
    {
        public ProgressionPage()
        {
            InitializeComponent();
        }

        private void Battlepass_Btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MainWindowInstance.mainContentFrame.Navigate(new Uri("/MVVM/View/ProgressionPage/Sections/Progression_Battlepass.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
