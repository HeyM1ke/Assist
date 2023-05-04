using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.ViewModels;
using ReactiveUI;
using ValNet.Core.Store;
using ValNet.Objects.Store;

namespace Assist.Views.Store.ViewModels
{
    internal class StoreViewModel : ViewModelBase
    {
        private bool _nightMarketEnabled = false;

        public bool NightMarketEnabled
        {
            get => _nightMarketEnabled;
            set => this.RaiseAndSetIfChanged(ref _nightMarketEnabled, value);
        }
        
        private string _accountVp = "";

        public string AccountVP
        {
            get => _accountVp;
            set => this.RaiseAndSetIfChanged(ref _accountVp, value);
        }
        
        private string _accountRp = "";

        public string AccountRP
        {
            get => _accountRp;
            set => this.RaiseAndSetIfChanged(ref _accountRp, value);
        }
        
        
        static Dictionary<string, ValUserStore> _UserStores = new Dictionary<string, ValUserStore>();
        static Dictionary<string, ValWallet> _UserWallets = new Dictionary<string, ValWallet>();
        /// <summary>
        /// Get's the current RiotUser's Store
        /// </summary>
        /// <returns>ValUserStore Obj</returns>
        public async Task<ValUserStore> GetPlayerStore()
        {
            if (_UserStores.ContainsKey(AssistApplication.Current.CurrentUser.UserData.sub))
            {

                if (_UserWallets.ContainsKey(AssistApplication.Current.CurrentUser.UserData.sub))
                {
                    AccountVP = $"{_UserWallets[AssistApplication.Current.CurrentUser.UserData.sub].Balances.ValorantPoints:n0}";
                    AccountRP = $"{_UserWallets[AssistApplication.Current.CurrentUser.UserData.sub].Balances.RadianitePoints:n0}";    
                }
                
                NightMarketEnabled = _UserStores[AssistApplication.Current.CurrentUser.UserData.sub].BonusStore is not null;
                
                return _UserStores[AssistApplication.Current.CurrentUser.UserData.sub];
            }

            var r = await AssistApplication.Current.CurrentUser.Store.GetPlayerStore();

            if (r == null)
                return null;

            NightMarketEnabled = r.BonusStore is not null;
            
            var t = await AssistApplication.Current.CurrentUser.Store.GetPlayerWallet();
            AccountVP = $"{t.Balances.ValorantPoints:n0}";
            AccountRP = $"{t.Balances.RadianitePoints:n0}";
            
            _UserWallets.Add(AssistApplication.Current.CurrentUser.UserData.sub,t);
            _UserStores.Add(AssistApplication.Current.CurrentUser.UserData.sub, r);

            return r;
        }

        /// <summary>
        /// Creates NightMarket Controls
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public async Task<List<Controls.Store.NightMarketOffer>> CreateMarketControls(ValUserStore store)
        {
            var thisList = new List<Controls.Store.NightMarketOffer>();

            for (int i = 0; i < store.BonusStore.NightMarketOffers.Count; i++)
            {
                var offer = store.BonusStore.NightMarketOffers[i];

                var skinInfo = await AssistApplication.ApiService.GetWeaponSkinAsync(offer.Offer.OfferID);

                thisList.Add(new Controls.Store.NightMarketOffer()
                {
                    SkinName = skinInfo.DisplayName,
                    SkinImage = skinInfo.Levels[0].DisplayIcon,
                    SkinDiscount = $"{offer.DiscountPercent}%",
                    OriginalPrice = $"{offer.Offer.Cost.ValorantPointCost}",
                    DiscountedPrice = $"{offer.DiscountCosts.ValorantPointCost}"
                });
            }

            return thisList;
        }

        public async Task<List<Controls.Store.BundleItem>> CreateBundleControls(ValUserStore store)
        {
            var thisList = new List<Controls.Store.BundleItem>();

            for (int i = 0; i < store.FeaturedBundle.Bundles.Count; i++)
            {
                var offer = store.FeaturedBundle.Bundles[i];

                thisList.Add(new Controls.Store.BundleItem(offer)
                {
                    Width = 810,
                    Height = 395,
                });
            }

            return thisList;
        }
    }
}
