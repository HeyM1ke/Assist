using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicData;
using ValNet.Objects.Exceptions;

namespace Assist.Services
{
    // Temp Class for Assist User
    internal class AssistApiUser
    {
        private const string BaseUrl = "https://api.assistapp.dev{0}";
        private string AuthenticationUrl => String.Format(BaseUrl, "/authentication/auth");
        private string UserInfoUrl => String.Format(BaseUrl, "/user/userinfo");
        private string ChangeUsernameUrl => String.Format(BaseUrl, "/user/changeusername");


        private HttpClient userClient = new HttpClient();
        public AssistToken Tokens;
        public AssistUserInfo UserInfo;
        public async Task<AssistToken> AuthenticateWithRedirectCode(string code)
        {
            var authClient = new HttpClient();

            authClient.DefaultRequestHeaders.Add("code", code);

            var data = await authClient.PostAsync(AuthenticationUrl, null);
            var content = await data.Content.ReadAsStringAsync();
            if (data.IsSuccessStatusCode)
            {
                Tokens = JsonSerializer.Deserialize<AssistToken>(content);
                userClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Tokens.AccessToken}");
                return Tokens;
            }

            throw new RequestException(data.StatusCode, content, content);
        }

        public async Task<AssistToken> AuthenticateWithRefreshToken(string refreshToken)
        {
            var authClient = new HttpClient();

            authClient.DefaultRequestHeaders.Add("refreshToken", refreshToken);

            var data = await authClient.PostAsync(AuthenticationUrl, null);
            var content = await data.Content.ReadAsStringAsync();
            if (data.IsSuccessStatusCode)
            {
                Tokens = JsonSerializer.Deserialize<AssistToken>(content);
                userClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Tokens.AccessToken}");
                return Tokens;
            }

            throw new RequestException(data.StatusCode, content, content);
        }

        public async Task<AssistUserInfo> GetUserInfo()
        {
            var data = await userClient.GetAsync(UserInfoUrl);
            var content = await data.Content.ReadAsStringAsync();
            if (data.IsSuccessStatusCode)
            {
                UserInfo = JsonSerializer.Deserialize<AssistUserInfo>(content);
                return UserInfo;
            }

            throw new RequestException(data.StatusCode, content, content);
        }

        public async Task<bool> ChangeUsername(AssistChangeUsernameModel data)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var resp = await userClient.PostAsync(ChangeUsernameUrl, jsonContent);
            var content = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode)
            {
                return true;
            }

            throw new RequestException(resp.StatusCode, content, content);
        }
    }

    internal class AssistToken
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
        
    }

    internal class AssistUserInfo
    {
        public string id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public bool verified { get; set; }
        public string discordId { get; set; }
    }

    internal class AssistChangeUsernameModel
    {
        public string username { get; set; }
    }
}
