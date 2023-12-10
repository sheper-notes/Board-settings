using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class UserInfoUtil
    {
        public static async Task<UserInfo> GetUserInfo(string token, string authURL) {
            return new UserInfo() { UserId = "testID" };

            UserInfo currentUser;
            Uri uri = new Uri(authURL);
            try
            {
                currentUser = await new AuthenticationApiClient("https://sheper.eu.auth0.com/").GetUserInfoAsync(token);
            }
            catch (Exception)
            {
                return null;
            }

            return currentUser;
        }
    }
}
