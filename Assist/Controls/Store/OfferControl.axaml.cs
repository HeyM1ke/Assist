using System;
using Assist.Controls.Store.ViewModels;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Store
{
    public partial class OfferControl : UserControl
    {
        private readonly OfferControlViewModel _viewModel;

        public OfferControl()
        {
            DataContext = _viewModel = new OfferControlViewModel();
            InitializeComponent();
        }

        public OfferControl(string offerId)
        {
            DataContext = _viewModel = new OfferControlViewModel();
            _viewModel.OfferId = offerId;
            InitializeComponent();
        }



        private async void OfferControl_OnInitialized(object? sender, EventArgs e)
        {
            if(Design.IsDesignMode)
                return;
            await _viewModel.Setup();
        }
    }
}
