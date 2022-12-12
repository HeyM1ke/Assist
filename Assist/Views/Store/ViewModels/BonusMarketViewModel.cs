using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.ViewModels;
using ValNet.Objects.Store;
using NightMarketOffer = Assist.Controls.Store.NightMarketOffer;

namespace Assist.Views.Store.ViewModels
{
    internal class BonusMarketViewModel
    {
        Dictionary<string,ValUserStore> _UserStores = new Dictionary<string,ValUserStore>();
        public async Task<ValUserStore> GetNightMarket()
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

        public async Task<List<NightMarketOffer>> CreateMarketControls(ValUserStore store)
        {
            var thisList = new List<NightMarketOffer>();

            for (int i = 0; i < store.BonusStore.NightMarketOffers.Count; i++)
            {
                var offer = store.BonusStore.NightMarketOffers[i];

                var skinInfo = await AssistApplication.ApiService.GetWeaponSkinAsync(offer.Offer.OfferID);

                thisList.Add(new NightMarketOffer()
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
    }
}
