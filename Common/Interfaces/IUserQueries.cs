using Common.Enums;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IUserQueries
    {
        public Task<UserRoleRelation> GetUser(long boardId, string userId);
        public Task<IEnumerable<UserRoleRelation>> GetUsers(long boardId);
        public Task<bool> AddUser(long boardId, string Id, string userId, Role role);
        public Task<bool> ChangeUserRole(long boardId, string userId, Role role);
        public Task<bool> RemoveUser(long boardId, string userId);

    }
}
