using Common.Enums;
using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Mock
{
    public class MockBoardQueries : IBoardQueries
    {
        private List<Board> boards;
        private bool CreateBoardSuccesful;
        public MockBoardQueries(List<Board> boards) {
            this.boards = boards;
        }

        public async Task<Board> GetBoardById(long Id)
        {
            return boards.Where(x => x.Id == Id).FirstOrDefault();
        }

        public async Task UpdateBoardName(long Id, string NewName)
        {
            boards.Where(x => x.Id == Id).FirstOrDefault().Name = NewName;
        }
        public async Task<IEnumerable<Board>> GetBoardsForUser(string Id)
        {
            return boards.Where(x => x.Users.Any(y => y.UserId == Id)).ToList();

        }
        public async Task<bool> CreateBoard(Board board)
        {
            if (CreateBoardSuccesful == true) {
                boards.Add(board);
                return true;
            }
            return false;
        }

        public async Task UpdateSubscriptionType(long Id, SubscriptionType subscriptionType)
        {

        }
    }
}
