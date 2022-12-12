using System;
using System.Linq;
using Assist.Controls.Store.ViewModels;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Store
{
    public partial class OfferContainer : UserControl
    {

        private UniformGrid _offerGrid;
        private readonly OfferContainerViewModel _viewModel;

        public OfferContainer()
        {
            DataContext = _viewModel = new OfferContainerViewModel();
            InitializeComponent();
            _offerGrid = this.FindControl<UniformGrid>("OffersGrid");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        private async void OfferContainer_OnInitialized(object? sender, EventArgs e)
        {
            // On Load
            // Request RiotUser Store if User is not Null
            if (Design.IsDesignMode)
                return;

            if (AssistApplication.Current.CurrentUser != null)
                await AssistApplication.Current.CurrentUser.Store.GetPlayerStore();

            

            await _viewModel.Setup();

            foreach (var c in _viewModel.OfferControls)
                _offerGrid.Children.Add(c);
        }
    }
}
