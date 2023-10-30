using Common.Enums;
using Common.Interfaces;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UserQueries : IUserQueries
    {
        private readonly Database db;
        public UserQueries(Database database)
        {
            db = database;
        }

        public async Task<UserRoleRelation> GetUser(long boardId, string userId)
        {
            return await db.Users.Where(x => x.BoardId == boardId && x.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserRoleRelation>> GetUsers(long boardId)
        {
            return await db.Users.Where(x => x.BoardId == boardId).ToListAsync();
        }


        public async Task ChangeUserRole(long boardId, string userId, Role role) 
        {
            var user = await db.Users.Where(x => x.BoardId == boardId && x.UserId == userId).FirstOrDefaultAsync();
            user.Role = role;
            await db.SaveChangesAsync();
        }

        public async Task<bool> RemoveUser(long boardId, string userId)
        {
            var user = await db.Users.Where(x => x.BoardId == boardId && x.UserId == userId).FirstOrDefaultAsync();
            var result = db.Users.Remove(user);
            return await db.SaveChangesAsync() > 0;
        }
    }
}
