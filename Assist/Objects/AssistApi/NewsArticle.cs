using System.Text.Json.Serialization;

namespace Assist.Objects.AssistApi
{

    public class NewsArticle
    {

        public string Guid { get; set; }

        [JsonPropertyName("newsTitle")] public string Title { get; set; }

        [JsonPropertyName("newsDescription")] public string Description { get; set; }

        [JsonPropertyName("newsImage")] public string ImageUrl { get; set; }

        [JsonPropertyName("newsUrl")] public string Url { get; set; }

    }
}
