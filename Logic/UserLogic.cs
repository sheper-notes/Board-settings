using Common.Enums;
using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class UserLogic : IUserLogic
    {
        private readonly IUserQueries userQueries;
        public UserLogic(IUserQueries _userQueries) {
            userQueries = _userQueries;
        }

        public async Task<IEnumerable<UserRoleRelation>> GetUsers(long boardId)
        {
            return await userQueries.GetUsers(boardId);
        }

        public async Task<UserRoleRelation> GetUser(long boardId, string userId)
        {
            return await userQueries.GetUser(boardId, userId);
        }

        public async Task ChangeUserRole(long boardId, string userId, Role role)
        {

            await userQueries.ChangeUserRole(boardId, userId, role);
        }

        public async Task<bool> RemoveUser(long boardId, string userId)
        {
            return await userQueries.RemoveUser(boardId, userId);
        }
    }
}
