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
using Assist.Controls.Extra.ViewModels;
using Assist.Modules.Popup;

namespace Assist.Controls.Extra
{
    /// <summary>
    /// Interaction logic for Popup.xaml
    /// </summary>
    public partial class Popup : UserControl
    {
        public PopupViewModel _viewModel;
        public Popup()
        {
            InitializeComponent();
        }

        public Popup(PopupSettings Settings)
        {
            DataContext = _viewModel = new PopupViewModel();
            _viewModel.PopupSettings = Settings;
            InitializeComponent();
            
        }

        private void Popup_Loaded(object sender, RoutedEventArgs e)
        {


            switch (_viewModel.PopupSettings.PopupType)
            {
                case PopupType.LOADING:
                    Loading.Visibility = Visibility.Visible;
                    break;
                case PopupType.OK:
                    OK_BTN.Visibility = Visibility.Visible;
                    break;
                case PopupType.ERROR:
                    ERR_BTN.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
