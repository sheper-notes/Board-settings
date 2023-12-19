using API.Controllers;
using IdGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Mock;

namespace Tests
{
    public class BoardControllerTests
    {
        [Fact]
        public void GetBoards()
        {
            var idGenerator = new IdGenerator(123);
            var boardQueries = new MockBoardQueries();
            var boardController = new BoardController();
        }
    }
}
