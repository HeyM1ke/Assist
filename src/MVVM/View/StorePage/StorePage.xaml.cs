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
using System.Diagnostics;

namespace Assist.MVVM.View.StorePage
{
    /// <summary>
    /// Interaction logic for StorePage.xaml
    /// </summary>
    public partial class StorePage : Page
    {
        AssistApplication _viewModel = AssistApplication.AppInstance;
        public StorePage()
        {
            DataContext = _viewModel;
            InitializeComponent();
        }

        private async void storePage_Initialized(object sender, EventArgs e)
        {
            // Load Daily Items (Loaded by Storepage for now, need to make custom control for future performance)
            await _viewModel.StorePageViewModel.GetUserStore();
            Load_DailyStore();

        }


        private async void Load_DailyStore()
        {
            for (int i = 0; i < _viewModel.StorePageViewModel.playerStore.SkinsPanelLayout.SingleItemOffers.Count; i++)
            {
                if(i == _viewModel.StorePageViewModel.playerStore.SkinsPanelLayout.SingleItemOffers.Count - 1)
                {
                    var skinOffer = new AssistStoreItemControl(_viewModel.StorePageViewModel.playerStore.SkinsPanelLayout.SingleItemOffers[i]);
                    
                    DailyItemGrid.Children.Add(skinOffer);
                }
                else
                {
                    var skinOffer = new AssistStoreItemControl(_viewModel.StorePageViewModel.playerStore.SkinsPanelLayout.SingleItemOffers[i])
                    {
                        
                        Margin = new Thickness(0, 0, 10, 0)
                    };
                    DailyItemGrid.Children.Add(skinOffer);
                }

                
            }
        }
    }
}
