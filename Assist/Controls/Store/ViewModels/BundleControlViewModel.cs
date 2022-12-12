using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.ViewModels;
using ReactiveUI;

namespace Assist.Controls.Store.ViewModels
{
    internal class BundleControlViewModel : ViewModelBase
    {
        private IEnumerable<BundleItem> _bundles = new List<BundleItem>() {new BundleItem()};

        public IEnumerable<BundleItem> Bundles
        {
            get => _bundles;
            set => this.RaiseAndSetIfChanged(ref _bundles, value);
        }


        public async Task Setup()
        {
            
            if (AssistApplication.Current.CurrentUser == null)
            {
                Bundles = new List<BundleItem>()
                {
                    new BundleItem()
                };

                return;
            }

            var store = AssistApplication.Current.CurrentUser.Store.PlayerStore;

            Bundles = store.FeaturedBundle.Bundles.Select(b => new BundleItem(b));
        }
    }
}
