using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.ViewModels;
using ValNet.Core.Store;
using ValNet.Objects.Store;

namespace Assist.Views.Store.ViewModels
{
    internal class StoreViewModel : ViewModelBase
    {
        static Dictionary<string, ValUserStore> _UserStores = new Dictionary<string, ValUserStore>();
        /// <summary>
        /// Get's the current RiotUser's Store
        /// </summary>
        /// <returns>ValUserStore Obj</returns>
        public async Task<ValUserStore> GetPlayerStore()
        {
            if (_UserStores.ContainsKey(AssistApplication.Current.CurrentUser.UserData.sub))
            {
                return _UserStores[AssistApplication.Current.CurrentUser.UserData.sub];
            }

            var r = await AssistApplication.Current.CurrentUser.Store.GetPlayerStore();

            if (r == null)
                return null;

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
