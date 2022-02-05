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

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistStoreBundleContainer.xaml
    /// </summary>
    public partial class AssistStoreBundleContainer : UserControl
    {
        AssistApplication _viewModel => AssistApplication.AppInstance;
        List<AssistStoreBundleControl> bundles = new List<AssistStoreBundleControl>();
        public AssistStoreBundleContainer()
        {
            InitializeComponent();
        }

        private async void StoreBundleContainer_Initialized(object sender, EventArgs e)
        {
            bundles.Clear();
            await _viewModel.StorePageViewModel.GetUserStore();

            foreach (var bundle in _viewModel.StorePageViewModel.playerStore.FeaturedBundle.Bundles)
            {
                var bundleControl = new AssistStoreBundleControl(bundle)
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Height = 570,
                    Width = 1168
                };

                bundles.Add(bundleControl);
            }

            if(bundles.Count <= 1)
            {
                MainGrid.Children.Remove(ScrollContainer);
                MainGrid.Children.Add(bundles[0]);
            }
        }
    }
}
