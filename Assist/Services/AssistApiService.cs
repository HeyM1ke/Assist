using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assist.Objects.AssistApi;
using Assist.Objects.AssistApi.Valorant;
using Assist.Objects.AssistApi.Valorant.Battlepass;
using Assist.Objects.AssistApi.Valorant.Offer;
using Assist.Objects.AssistApi.Valorant.Skin;
using Assist.Objects.Enums;
using Assist.Services.Utils;
using Assist.Settings;
using RestSharp;
using Serilog;

namespace Assist.Services
{
    internal class AssistApiService
    {
        public const string BaseUrl = "https://api.assistapp.dev";
        public const string BattlepassId = "2b3a941d-4b85-a0df-5beb-8897224d290a";

        private const string FailedNewsArticleImageUrl =
            "https://images.contentstack.io/v3/assets/bltb6530b271fddd0b1/blta8463fc941226152/638a967c34be4631e02db299/12062022_eoy_2022_16x9_banner.jpg";
        private const int MaintenanceTimeoutInSeconds = 5;

        //private readonly ILogger _logger;
        private readonly RestClient _client;

        public AssistApiService()
        {
            //_logger = Log.ForContext<AssistApiService>();
            _client = new RestClient(BaseUrl);
        }

        // todo: handle unsuccessful response
        public async Task<WeaponSkin> GetWeaponSkinAsync(string uuid)
        {
            var request = new RestRequest($"/valorant/skins/{uuid}");
            var response = await _client.ExecuteAsync<WeaponSkin>(request);

            return response.Data!;
        }

        public async Task<string> GetWeaponSkinPriceAsync(string uuid)
        {
            var request = new RestRequest($"/valorant/offers/{uuid}");
            var response = await _client.ExecuteAsync<StoreOffer>(request);
            var isSuccess = response.IsSuccessful;

            var offer = response.Data;
            var cost = isSuccess ? offer!.Cost.Cost : 99999;

            return $"{cost:n0}";
        }

        public async Task<Battlepass> GetBattlepassAsync()
        {
            var request = new RestRequest($"/valorant/battlepass/{BattlepassId}");
            var response = await _client.ExecuteAsync<Battlepass>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data;
        }
        public async Task<Battlepass> GetBattlepassAsync(string bpId)
        {
            var request = new RestRequest($"/valorant/battlepass/{bpId}");
            var response = await _client.ExecuteAsync<Battlepass>(request);
            if (!response.IsSuccessful)
                return null;

            return response.Data;
        }

        public async Task<NewsArticle[]> GetNewsAsync()
        {
            var request = new RestRequest("/valorant/news");
            var response = await _client.ExecuteAsync<NewsArticle[]>(request);
            if (!response.IsSuccessful)
                return new[] { CreateFailedNewsTab() };

            return response.Data;
        }

        public async Task<NewsArticle[]> GetNewsAsyncByRegion()
        {
            var request = new RestRequest($"/valorant/news/{AssistSettings.Current.Language.GetAttribute<LanguageAttribute>().Code.ToLower()}");
            var response = await _client.ExecuteAsync<NewsArticle[]>(request);
            if (!response.IsSuccessful)
                return new[] { CreateFailedNewsTab() };

            return response.Data;
        }

        private static NewsArticle CreateFailedNewsTab()
        {
            return new NewsArticle
            {
                title = "VALORANT 2022 YEAR-END",
                description = "Celebrate the year with actual content! we know we havent given you any recently",
                imageUrl = FailedNewsArticleImageUrl,
                nodeUrl = "https://playvalorant.com/en-us/news/game-updates/valorant-2022-year-end/"
            };
        }

        public async Task<Bundle> GetBundleAsync(string id)
        {
            var request = new RestRequest($"/valorant/bundles/{id}");
            var response = await _client.ExecuteAsync<Bundle>(request);
            if (!response.IsSuccessful)
                return CreateFailedBundle();

            return response.Data;
        }

        private static Bundle CreateFailedBundle()
        {
            return new Bundle
            {
                Name = "Could not find bundle on server",
                Description = "Please contact Mike to fix this issue.",
                DisplayIcon = "https://cdn.rumblemike.com/Bundles/2116a38e-4b71-f169-0d16-ce9289af4bfa_DisplayIcon.png"
            };
        }

        public async Task<AssistMaintenance> GetMaintenanceStatus()
        {
            Log.Information("Checking for Maintenance");

            var client = new RestClient(new RestClientOptions
            {
                BaseUrl = new Uri(BaseUrl),
                ThrowOnAnyError = false,
                ThrowOnDeserializationError = false,
                Timeout = MaintenanceTimeoutInSeconds * 1000
            });

            var request = new RestRequest("/data/status/maintenance");
            var response = await client.ExecuteAsync<AssistMaintenance>(request);

            if (!response.IsSuccessful)
            {
                Log.Information("Failed to request the maintenance status.");
                return CreateDefaultMaintenanceMessage();
            }

            return response.Data;
        }

        public async Task<AssistAgent> GetAgent()
        {
            Log.Information("Getting Agent");

            var client = new RestClient(new RestClientOptions
            {
                BaseUrl = new Uri(BaseUrl),
                ThrowOnAnyError = false,
                ThrowOnDeserializationError = false,
                Timeout = 5000
            });

            var request = new RestRequest("/data/agent");
            var response = await client.ExecuteAsync<AssistAgent>(request);

            if (!response.IsSuccessful)
            {
                Log.Information("Failed to request agent.");
                return new AssistAgent()
                {
                    Agent = string.Empty
                };
            }

            return response.Data;
        }

        private static AssistMaintenance CreateDefaultMaintenanceMessage()
        {
            return new AssistMaintenance
            {
                DownForMaintenance = false,
                DownForMaintenanceMessage =
                    "Assist is currently down for Maintenance. Please come back later. Check out the discord for information regarding the Maintenance."
            };
        }

        public async Task<Mission> GetMission(string id)
        {
            var request = new RestRequest($"/valorant/missions/{id}");
            var response = await _client.ExecuteAsync<Mission>(request);
            if (!response.IsSuccessful)
                return CreateFailedMission();

            return response.Data;
        }

        public async Task<List<Mission>> GetAllMissions()
        {
            var request = new RestRequest($"/valorant/missions");
            var response = await _client.ExecuteAsync<List<Mission>>(request);
            if (!response.IsSuccessful)
                return new List<Mission>()
                {
                    CreateFailedMission()
                };

            return response.Data;
        }

        private static Mission CreateFailedMission()
        {
            return new Mission
            {
                Title = "Error",
                ProgressToComplete = 1,
                XpGrant = 1,
            };
        }
    }
}
