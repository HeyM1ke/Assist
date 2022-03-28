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
using Assist.MVVM.Model;
using Assist.MVVM.ViewModel;
using ValNet.Objects.Store;

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistStoreBundleControl.xaml
    /// </summary>
    public partial class AssistStoreBundleControl : UserControl
    {
        private AssistApplication _viewModel => AssistApplication.AppInstance;
        private Bundle _bundleControlBundle;
        private AssistBundleObj _bundleData;
        public AssistStoreBundleControl(Bundle bundle)
        {
            _bundleControlBundle = bundle;
            DataContext = _viewModel;
            InitializeComponent();
            

        }

        private async void BundleControl_Initialized(object sender, EventArgs e)
        {
            _bundleData =  await _viewModel.StoreBundleViewModel.GetBundleData(_bundleControlBundle.DataAssetID);
            loadImage(_bundleData);

            bundleName = _bundleData.bundleDisplayName.ToUpper();
            bundleDescription = _bundleData.bundleExtraDescription;
            bundlePrice = await _viewModel.StoreBundleViewModel.GetBundlePrice(_bundleControlBundle);
        }




        private async void loadImage(AssistBundleObj bundleObj)
        {
            // Allows the image to be loaded with the resolution it is intended to be used for.
            // Because the program is a solo resolution that doesnt change res, this is fine.

            var image = new BitmapImage();
            image.BeginInit();
            image.DecodePixelWidth = 974;
            image.DecodePixelHeight = 474;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(bundleObj.bundleDisplayIcon, UriKind.Absolute);
            image.EndInit();

            bundleImageContainer.Source = image;
        }


        #region Control Properties
        public string bundleName
        {
            get { return (string)GetValue(bundleNameProperty); }
            set { SetValue(bundleNameProperty, value); }
        }

        public static readonly DependencyProperty bundleNameProperty = DependencyProperty.Register(
            "bundleName", typeof(string), typeof(AssistStoreBundleControl));




        public string bundleDescription
        {
            get { return (string)GetValue(bundleDescriptionProperty); }
            set { SetValue(bundleDescriptionProperty, value); }
        }

        public static readonly DependencyProperty bundleDescriptionProperty = DependencyProperty.Register(
            "bundleDescription", typeof(string), typeof(AssistStoreBundleControl));

        public static readonly DependencyProperty bundlePriceProperty = DependencyProperty.Register(
            "bundlePrice", typeof(string), typeof(AssistStoreBundleControl), new PropertyMetadata(default(string)));

        public string bundlePrice
        {
            get { return (string)GetValue(bundlePriceProperty); }
            set { SetValue(bundlePriceProperty, value); }
        }

        #endregion
    }

}
