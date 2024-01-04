using API.Controllers;
using Common.Models;
using Data;
using IdGen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tests.Mock;

namespace Tests
{
    public class BoardControllerTests
    {
        private DbContextOptions<Database> CreateNewContextOptions()
        {
            // Create options for ApplicationDbContext
            var options = new DbContextOptionsBuilder<Database>()
                .UseInMemoryDatabase(databaseName: "InMemoryAppDatabase")
                .Options;

            return options;
        }

        private Database GetDatabase(DbContextOptions<Database> options)
        {
            return new Database(options);
        }

        private void MockUserAuthentication(BoardController controller, string jwt)
        {
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.HttpContext.Request.Headers["Authorization"] = jwt;
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext,
            };
        }

        [Fact]
        public async void GetBoards()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);
            var board = new Board() { Id= idGenerator.CreateId(), Name = "Test Name", SubscriptionType = Common.Enums.SubscriptionType.Enterprise };
            await boardQueries.CreateBoard(board);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "testID", Common.Enums.Role.Owner);

            var mockUserInfoUtil = new MockUserInfoUtil(new Auth0.AuthenticationApi.Models.UserInfo() { UserId = "testID"});
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var boardController = new BoardController(idGenerator, boardQueries, userQueries, mockUserInfoUtil, configuration);
  

            MockUserAuthentication(boardController, "jwt");


            var result = await boardController.GetBoards();

            var okResult = result as OkObjectResult;
            var data = (IEnumerable<Board>)okResult.Value;
            Assert.Equal(okResult.StatusCode, 200);
            Assert.Equal(data.FirstOrDefault().Name, "Test Name");
        }

        [Fact]
        public async void GetBoards_User_Does_Not_Exist()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);
            var board = new Board() { Id = idGenerator.CreateId(), Name = "Test Name", SubscriptionType = Common.Enums.SubscriptionType.Enterprise };
            await boardQueries.CreateBoard(board);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "testID", Common.Enums.Role.Owner);

            var mockUserInfoUtil = new MockUserInfoUtil(null);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var boardController = new BoardController(idGenerator, boardQueries, userQueries, mockUserInfoUtil, configuration);


            MockUserAuthentication(boardController, "jwt");


            var result = await boardController.GetBoards();

            var unauthorizedResult = result as UnauthorizedResult;

            Assert.Equal(401, unauthorizedResult.StatusCode);
        }


        [Fact]
        public async void GetBoards_Has_No_Boards()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);

            var mockUserInfoUtil = new MockUserInfoUtil(new Auth0.AuthenticationApi.Models.UserInfo() { UserId = "testID" });
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var boardController = new BoardController(idGenerator, boardQueries, userQueries, mockUserInfoUtil, configuration);


            MockUserAuthentication(boardController, "jwt");


            var result = await boardController.GetBoards();

            var okResult = result as OkObjectResult;
            var data = (IEnumerable<Board>)okResult.Value;
            Assert.Equal(okResult.StatusCode, 200);
            Assert.Equal(data.Count(), 0);
        }


        [Fact]
        public async void CreateBoard()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);
            var board = new Board() { Id = 0, Name = "Test Name", SubscriptionType = Common.Enums.SubscriptionType.Enterprise };

            var mockUserInfoUtil = new MockUserInfoUtil(new Auth0.AuthenticationApi.Models.UserInfo() { UserId = "testID" });
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var boardController = new BoardController(idGenerator, boardQueries, userQueries, mockUserInfoUtil, configuration);


            MockUserAuthentication(boardController, "jwt");


            var result = await boardController.CreateBoard(board);

            var okResult = result as OkObjectResult;
            var data = (long)okResult.Value;
            Assert.Equal(okResult.StatusCode, 200);
            Assert.NotEqual(data, 0);
        }

        [Fact]
        public async void CreateBoard_User_Does_Not_Exist()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);
            var board = new Board() { Id = 0, Name = "Test Name", SubscriptionType = Common.Enums.SubscriptionType.Enterprise };

            var mockUserInfoUtil = new MockUserInfoUtil(null);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var boardController = new BoardController(idGenerator, boardQueries, userQueries, mockUserInfoUtil, configuration);


            MockUserAuthentication(boardController, "jwt");


            var result = await boardController.CreateBoard(board);

            var unauthorizedResult = result as UnauthorizedResult;

            Assert.Equal(401, unauthorizedResult.StatusCode);
        }
    }
}
