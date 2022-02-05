using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using RestSharp;
using Assist.MVVM.Model;

namespace Assist.MVVM.ViewModel
{
    internal class  AssistFeaturedViewModel
    {
        public List<News>? newsList = new List<News>();

        public async Task GetFeaturedNews()
        {
            newsList = await AssistApplication.AppInstance.AssistApiController.GetAssistNews();

        }

    }
}
