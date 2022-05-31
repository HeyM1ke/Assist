using Assist.Objects;
using Assist.Objects.Valorant;
using Assist.Objects.Valorant.Bp;
using Assist.Objects.Valorant.Offer;
using Assist.Objects.Valorant.Skin;

using RestSharp;

using System.Threading.Tasks;

namespace Assist.Services;

public class AssistApiService
{

    private const string FailedNewsArticleImageUrl =
        "https://i.kym-cdn.com/entries/icons/original/000/037/349/Screenshot_14.jpg";
    private const string BaseUrl = "https://api.assistapp.dev";
    private const string BattlepassId = "d80f3ef5-44f5-8d70-6935-f2840b2d3882";

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
        if (response.IsSuccessful)
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
            return CreatedFailedBundle();

        return response.Data;
    }

    private static Bundle CreatedFailedBundle()
    {
        return new Bundle
        {
            Name = "Could not find bundle on server",
            Description = "Please contact Mike to fix this issue.",
            DisplayIcon = "https://cdn.rumblemike.com/Bundles/2116a38e-4b71-f169-0d16-ce9289af4bfa_DisplayIcon.png"
        };
    }

    /*
     * public async Task<AssistMaintenanceObj> GetMaintenanceStatus()
        {
            Log.Information("Checking for Maintenance");
            var resp = await client.ExecuteAsync(new RestRequest(maintUrl), Method.Get);

            if (resp.IsSuccessful)
                return JsonSerializer.Deserialize<AssistMaintenanceObj>(resp.Content);
            else
                return new() { DownForMaintenance = false, DownForMaintenanceMessage = "Assist is currently down for Maintenance. Please come back later. Check out the discord for information regarding the Maintenance."};
        }
     */

}
