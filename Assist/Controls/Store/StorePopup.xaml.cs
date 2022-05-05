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
using Assist.Controls.Store.ViewModels;
using Assist.Modules.Popup;
using Assist.MVVM.Model;

namespace Assist.Controls.Store
{
    /// <summary>
    /// Interaction logic for StorePopup.xaml
    /// </summary>
    public partial class StorePopup : UserControl
    {
        private StorePopupViewModel _viewModel;
        public StorePopup(AssistSkin skin)
        {
            DataContext = _viewModel = new StorePopupViewModel();
            _viewModel.Skin = skin;
            InitializeComponent();
            Setup();
        }

        void Setup()
        {
            for (int i = 0; i < _viewModel.Skin.Levels.Count; i++)
            {
                if (_viewModel.Skin.Levels[i].StreamedVideoUrl is not null)
                {
                    var btn = new RadioButton()
                    {
                        Content = i+1
                    };

                    btn.Click += ChangeLVideo;

                    LevelContainer.Children.Add(btn);
                }
            }


            for (int i = 0; i < _viewModel.Skin.Chromas.Count; i++)
            {
                if (_viewModel.Skin.Chromas[i].StreamedVideoUrl is not null)
                {
                    var btn = new RadioButton()
                    {
                        Content = i + 1
                    };

                    btn.Click += ChangeCVideo;

                    ChromaContainer.Children.Add(btn);
                }
            }

            if (LevelContainer.Children.Count is not 0)
                _viewModel.ChangeChromaVideo(0);
            else if (ChromaContainer.Children.Count is not 0)
                _viewModel.ChangeChromaVideo(0);
            else
            {
                PopupSystem.KillPopups();
                PopupSystem.SpawnPopup(new PopupSettings()
                {
                    PopupTitle = "This Video Does not contain extra content.",
                    PopupType = PopupType.OK
                });
            }
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            preview.Source = null;
            PopupSystem.KillPopups();
        }


        void ChangeCVideo(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeChromaVideo(Convert.ToInt32(((RadioButton)sender).Content)-1);
        }

        void ChangeLVideo(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeLevelVideo(Convert.ToInt32(((RadioButton)sender).Content)-1);
        }
    }
}
