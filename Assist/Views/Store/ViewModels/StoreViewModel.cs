using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Controls.Store;
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
        
        private string _accountKc = "";

        public string AccountKC
        {
            get => _accountKc;
            set => this.RaiseAndSetIfChanged(ref _accountKc, value);
        }
        
        private string _accountRp = "";

        public string AccountRP
        {
            get => _accountRp;
            set => this.RaiseAndSetIfChanged(ref _accountRp, value);
        }

        private ObservableCollection<SkinStoreOfferV2> _skinOffers = new ObservableCollection<SkinStoreOfferV2>();

        public ObservableCollection<SkinStoreOfferV2> SkinOffers
        {
            get => _skinOffers;
            set => this.RaiseAndSetIfChanged(ref _skinOffers, value);
        }
        
        private ObservableCollection<BonusMarketControl> _bonusSkinOffers = new ObservableCollection<BonusMarketControl>();

        public ObservableCollection<BonusMarketControl> BonusSkinOffers
        {
            get => _bonusSkinOffers;
            set => this.RaiseAndSetIfChanged(ref _bonusSkinOffers, value);
        }

        private string _bundleName = "";

        public string BundleName
        {
            get => _bundleName;
            set => this.RaiseAndSetIfChanged(ref _bundleName, value);
        }

        private string _bundlePrice = "";

        public string BundlePrice
        {
            get => _bundlePrice;
            set => this.RaiseAndSetIfChanged(ref _bundlePrice, value);
        }
        
        private string _bundleImage = "";

        public string BundleImage
        {
            get => _bundleImage;
            set => this.RaiseAndSetIfChanged(ref _bundleImage, value);
        }
        
        public static Dictionary<string, ValUserStore> _UserStores = new Dictionary<string, ValUserStore>();
        public static Dictionary<string, ValWallet> _UserWallets = new Dictionary<string, ValWallet>();
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
                    AccountKC = $"{_UserWallets[AssistApplication.Current.CurrentUser.UserData.sub].Balances.KingdomCredits:n0}";    
                }
                
                NightMarketEnabled = _UserStores[AssistApplication.Current.CurrentUser.UserData.sub].BonusStore is not null;
                
                return _UserStores[AssistApplication.Current.CurrentUser.UserData.sub];
            }

            var r = await AssistApplication.Current.CurrentUser.Store.GetPlayerStore();

            if (r == null)
                return null;

            NightMarketEnabled = r.BonusStore is not null;

           
            if (!_UserWallets.ContainsKey(AssistApplication.Current.CurrentUser.UserData.sub))
            {
                var t = await AssistApplication.Current.CurrentUser.Store.GetPlayerWallet();
                _UserWallets.TryAdd(AssistApplication.Current.CurrentUser.UserData.sub,t);
            }
            
            
            if (_UserWallets.ContainsKey(AssistApplication.Current.CurrentUser.UserData.sub))
            {
                AccountVP = $"{_UserWallets[AssistApplication.Current.CurrentUser.UserData.sub].Balances.ValorantPoints:n0}";
                AccountRP = $"{_UserWallets[AssistApplication.Current.CurrentUser.UserData.sub].Balances.RadianitePoints:n0}";
                AccountKC = $"{_UserWallets[AssistApplication.Current.CurrentUser.UserData.sub].Balances.KingdomCredits:n0}";    
            }
            
            _UserStores.TryAdd(AssistApplication.Current.CurrentUser.UserData.sub, r);

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

        public async Task Setup()
        {
            var store = await GetPlayerStore();
            SetupBundle(store);
            CreateSkinOfferControls(store);
        }

        public async Task GetPlayerWallet()
        {
            try
            {
                if (_UserWallets.ContainsKey(AssistApplication.Current.CurrentUser.UserData.sub))
                {
                    AccountVP = $"{_UserWallets[AssistApplication.Current.CurrentUser.UserData.sub].Balances.ValorantPoints:n0}";
                    AccountRP = $"{_UserWallets[AssistApplication.Current.CurrentUser.UserData.sub].Balances.RadianitePoints:n0}";  
                    AccountKC = $"{_UserWallets[AssistApplication.Current.CurrentUser.UserData.sub].Balances.KingdomCredits:n0}";  
                    return;
                }
                var t = await AssistApplication.Current.CurrentUser.Store.GetPlayerWallet();
                AccountVP = $"{t.Balances.ValorantPoints:n0}";
                AccountRP = $"{t.Balances.RadianitePoints:n0}";  
                AccountKC = $"{t.Balances.KingdomCredits:n0}";    
            }
            catch (Exception e)
            {
                
            }
        }

        private async void SetupBundle(ValUserStore? store)
        {
            if(store.FeaturedBundle.Bundle.DataAssetID == null)
                return;

            var bData = await AssistApplication.ApiService.GetBundleAsync(store.FeaturedBundle.Bundle.DataAssetID);

            BundleName = bData.Name.ToUpper();
            BundleImage = bData.DisplayIcon;
            var price = store.FeaturedBundle.Bundle.Items.Sum(x => x.DiscountedPrice);
            BundlePrice = $"{price:n0}";
        }
        
        private async void CreateSkinOfferControls(ValUserStore? store)
        {
            for (int i = 0; i < store.SkinsPanelLayout.SingleItemOffers.Count; i++)
            {
                var OfferId = store.SkinsPanelLayout.SingleItemOffers[i];
                var sData = await AssistApplication.ApiService.GetWeaponSkinAsync(OfferId);
                var price = await AssistApplication.ApiService.GetWeaponSkinPriceAsync(OfferId);
                var skinOfferControl = new SkinStoreOfferV2()
                {
                    SkinId = OfferId,
                    SkinImage = sData.DisplayIcon,
                    SkinName = sData.DisplayName,
                    SkinCost = price 
                };

               SkinOffers.Add(skinOfferControl);
            }
        }

        public async Task NightMarketSetup()
        {
            var store = await GetPlayerStore();
            CreateNightMarketOfferControls(store);
        }

        private async void CreateNightMarketOfferControls(ValUserStore? store)
        {
            for (int i = 0; i < store.BonusStore.NightMarketOffers.Count; i++)
            {
                var offer = store.BonusStore.NightMarketOffers[i];
                
                var sData = await AssistApplication.ApiService.GetWeaponSkinAsync(offer.Offer.OfferID);
                
                var skinOfferControl = new BonusMarketControl()
                {
                    SkinId = offer.Offer.OfferID,
                    SkinImage = sData.DisplayIcon,
                    SkinName = sData.DisplayName,
                    SkinCost = $"{offer.DiscountCosts.ValorantPointCost:n0}",
                    SkinDiscountPercentage = $"{offer.DiscountPercent}%",
                    SkinOriginalCost = $"{offer.Offer.Cost.ValorantPointCost:n0}",
                };

                BonusSkinOffers.Add(skinOfferControl);
            }
        }
    }
}
