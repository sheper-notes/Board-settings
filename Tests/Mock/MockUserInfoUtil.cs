using Auth0.AuthenticationApi.Models;
using Auth0.AuthenticationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Interfaces;

namespace Tests.Mock
{
    public class MockUserInfoUtil : IUserInfoUtil
    {
        private UserInfo userInfo;
        public async Task<UserInfo> GetUserInfo(string token, string authURL)
        {
            return userInfo;
        }

        public MockUserInfoUtil(UserInfo userInfo) {
            this.userInfo = userInfo;
        }
    }
}
