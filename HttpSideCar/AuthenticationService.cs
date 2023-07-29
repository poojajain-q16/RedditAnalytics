using System;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Models;
using Models.Authentication;
using Newtonsoft.Json;

namespace HttpSideCar
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;

        private string _accessToken;
        private DateTimeOffset _tokenExpiration;

        private readonly RedditApiCredentials credentials;

        public AuthenticationService(IOptions<RedditApiCredentials> credentialsOptions)
        {

            _httpClient = new HttpClient();
            this.credentials = credentialsOptions.Value;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (_accessToken != null && DateTimeOffset.Now < _tokenExpiration)
            {
                return _accessToken;
            }

            _accessToken = string.Empty;

            //add parameters on request
            var body = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{credentials.ClientId}:{credentials.ClientSecret}")));

            // var request = new HttpRequestMessage(HttpMethod.Post, credentials.RedditAuthUrl);
            // request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{credentials.ClientId}:{credentials.ClientSecret}")));
            // request.Content = new FormUrlEncodedContent(body);

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Postman");
            var response = await _httpClient.PostAsync(credentials.RedditAuthUrl, new FormUrlEncodedContent(body));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<AuthenticationModel>(content);

            _accessToken = json.access_token;
            _tokenExpiration = DateTimeOffset.Now.AddSeconds(json.expires_in);

            return _accessToken;
        }
    }

}

