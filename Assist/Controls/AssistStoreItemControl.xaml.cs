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
using Assist.MVVM.Model;
using Assist.MVVM.ViewModel;

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistStoreControl.xaml
    /// </summary>
    public partial class AssistStoreItemControl : UserControl
    {
        public static readonly DependencyProperty skinNameProperty = DependencyProperty.Register("skinName", typeof(string), typeof(AssistStoreItemControl), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty skinImageUrlProperty = DependencyProperty.Register("skinImageUrl", typeof(string), typeof(AssistStoreItemControl), new PropertyMetadata(default(string)));

        private string skinId;
        private AssistApplication _viewmodel => AssistApplication.AppInstance;

        public AssistStoreItemControl()
        {
            DataContext = _viewmodel;
            skinId = "00s";
            InitializeComponent();
        }
        public AssistStoreItemControl(string skinid)
        {
            DataContext = _viewmodel;
            skinId = skinid;
            InitializeComponent();
        }

        private void loadImage(string url)
        {
            // Allows the image to be loaded with the resolution it is intended to be used for.
            // Because the program is a solo resolution that doesnt change res, this is fine.

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(url, UriKind.Absolute);
            image.EndInit();
            skinsImage.Source = image;



        }


        public string skinName
        {
            get { return (string) GetValue(skinNameProperty); }
            set { SetValue(skinNameProperty, value); }
        }

        public string skinImageUrl
        {
            get { return (string) GetValue(skinImageUrlProperty); }
            set { SetValue(skinImageUrlProperty, value); }
        }

        private async void _this_Initialized(object sender, EventArgs e)
        {
            var data = await _viewmodel.StoreItemViewModel.GetSkinInformation(skinId);
            LoadData(data);
            priceLabel.Content = await _viewmodel.StoreItemViewModel.GetSkinPrice(skinId);
        }

        private void LoadData(SkinObj data)
        {
            skinName = data.displayName;
            loadImage(data.levels[0].displayIcon);
        }
    }
}
