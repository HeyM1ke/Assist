using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.MVVM.Model;
using RestSharp;

namespace Assist.MVVM.ViewModel
{
    internal class AssistStoreItemViewModel
    {
       
       
        public async Task<SkinObj> GetSkinInformation(string skinId)
        {
            return await AssistApplication.AppInstance.AssistApiController.GetSkinObj(skinId);
        }

        public async Task<string> GetSkinPrice(string skinId)
        {
            return await AssistApplication.AppInstance.AssistApiController.GetSkinPricing(skinId);
        }
    }
}
