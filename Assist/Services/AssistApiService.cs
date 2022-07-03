using System.Text.Json;
using Assist.Objects;
using Assist.Objects.Valorant;
using Assist.Objects.Valorant.Bp;
using Assist.Objects.Valorant.Offer;
using Assist.Objects.Valorant.Skin;

using RestSharp;

using Serilog;

using System;
using System.Threading.Tasks;
using Assist.MVVM.Model;
using Serilog;

namespace Assist.Services;

public class AssistApiService
{

    public const string BaseUrl = "https://api.assistapp.dev";
    public const string BattlepassId = "99ac9283-4dd3-5248-2e01-8baf778affb4";

    private const string FailedNewsArticleImageUrl =
        "https://i.kym-cdn.com/entries/icons/original/000/037/349/Screenshot_14.jpg";
    private const int MaintenanceTimeoutInSeconds = 5;

    //private readonly ILogger _logger;
    private readonly RestClient _client;

    public AssistApiService()
    {
        //_logger = Log.ForContext<AssistApiService>();
        _client = new RestClient(BaseUrl);
    }

    public async Task<BackgroundClientInfo> GetBackgroundApplicationAsync()
    {
        var request = new RestRequest("/data/bgclient/data/");
        var response = await _client.ExecuteAsync<BackgroundClientInfo>(request);
        if (!response.IsSuccessful)
            return null;

        return response.Data;
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

    private static NewsArticle CreateFailedNewsTab()
    {
        return new NewsArticle
        {
            Title = "Error getting articles",
            Description = "No articles were found.",
            Image = FailedNewsArticleImageUrl,
            Url = "assistapp.dev"
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

}
