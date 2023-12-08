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
            UserInfo currentUser;
            Uri uri = new Uri(authURL);
            try
            {
                currentUser = await new AuthenticationApiClient(uri).GetUserInfoAsync(token);
            }
            catch (Exception)
            {
                return null;
            }

            return currentUser;
        }
    }
}
