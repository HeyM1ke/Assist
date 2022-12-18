using System.Text.Json.Serialization;

namespace Assist.Objects.AssistApi
{

    public class NewsArticle
    {

        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
        public string nodeUrl { get; set; }

    }
}
