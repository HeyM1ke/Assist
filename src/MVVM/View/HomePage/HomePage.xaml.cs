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
using Assist.MVVM.ViewModel;
using Assist.Controls;
namespace Assist.MVVM.View.HomePage
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            LoadStatusTEMP();
        }

        private async void LoadStatusTEMP()
        {
            var statusMessages = await AssistApplication.AppInstance.AssistApiController.GetStatusMessages();
            foreach (var message in statusMessages)
            {
                StatusMessageStackPanel.Children.Add(new StatusMessageControl() { MessageText = message.statusMessage });
            }
        }
    }
}
