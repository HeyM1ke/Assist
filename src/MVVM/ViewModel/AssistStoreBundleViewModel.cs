using System;
using Assist.MVVM.Model;
using RestSharp;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ValNet.Objects.Store;

namespace Assist.MVVM.ViewModel
{
    internal class AssistStoreBundleViewModel
    {
        

        public async Task<AssistBundleObj> GetBundleData(string dataAssetId)
        {
            return await AssistApplication.AppInstance.AssistApiController.GetBundleObj(dataAssetId);
        }

        public async Task<string> GetBundlePrice(Bundle bundle)
        {
            int price = 0;

            foreach (var item in bundle.Items)
            {
                price += item.DiscountedPrice;
            }

            return string.Format("{0:n0}", price);
        }
    }
}
