using Common.Enums;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IBoardQueries
    {
        public Task<Board> GetBoardById(long Id);
        public Task UpdateBoardName(long Id, string NewName);
        public Task<IEnumerable<Board>> GetBoardsForUser(string Id);
        public Task<bool> CreateBoard(Board board);

        public Task UpdateSubscriptionType(long Id, SubscriptionType subscriptionType);
    }
}
