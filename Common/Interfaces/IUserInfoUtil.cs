using Auth0.AuthenticationApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IUserInfoUtil
    {
        public Task<UserInfo> GetUserInfo(string token, string authURL);
    }
}
