using Auth0.AuthenticationApi.Models;
using Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Common
{
    public class Auth0AccessTokenManager
    {
        public bool isUpdating;
        public string accessToken;

        public async Task<string> GetDataAsync()
        {
            while (isUpdating)
            {
                await Task.Delay(100); // Wait until the update is complete
            }

            return accessToken;
        }

        public async Task RenewToken()
        {
            try
            {
                isUpdating = true;

                // Simulating an HTTP POST request to update data
                using (var httpClient = new HttpClient())
                {
                    var clientId = "MBxJEyzwxPgrSuZNHSdSqxg7Vug0tFFz"; // Fill in your client ID
                    var clientSecret = ""; // Fill in your client secret
                    var audience = "https://sheper.eu.auth0.com/api/v2/"; // Fill in your audience
                    var requestBody = $"{{\"client_id\":\"{clientId}\",\"client_secret\":\"{clientSecret}\",\"audience\":\"{audience}\",\"grant_type\":\"client_credentials\"}}";

                    var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync("https://sheper.eu.auth0.com/oauth/token", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        // Parse the response and extract the token
                        // Assuming the response contains a JSON with an "access_token" field
                        // You should handle this based on the actual response structure
                        AuthResponse tokenResponse = JsonConvert.DeserializeObject<AuthResponse>(responseContent);
                        accessToken = tokenResponse.access_token;
                    }
                }
            }
            finally
            {
                isUpdating = false;
            }
        }

    }
}
