using Assist.MVVM.Model;
using System.Threading.Tasks;
using ValNet.Objects.Store;

namespace Assist.MVVM.ViewModel
{
    internal class AssistNightMarketViewModel
    {
        public string SkinName { get; set; }
        public string DiscountPercentage { get; set; }
        public string ValorantPointCost { get; set; }
        public SkinObj skinData { get; set; }

        public async Task LoadSkin(NightMarket.NightMarketOffer skinOffer)
        {
            skinData = await AssistApplication.AppInstance.AssistApiController.GetSkinObj(skinOffer.Offer.OfferID);

            SkinName = skinData.displayName;
            DiscountPercentage = $"-{skinOffer.DiscountPercent}%";
            ValorantPointCost = $"{string.Format("{0:n0}", skinOffer.DiscountCosts.ValorantPointCost)}";

        }

    }
}
