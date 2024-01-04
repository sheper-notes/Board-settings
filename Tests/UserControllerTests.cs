using API.Controllers;
using Common.Models;
using Data;
using IdGen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Mock;

namespace Tests
{
    public class UserControllerTests
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

        private void MockUserAuthentication(UserController controller, string jwt)
        {
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.HttpContext.Request.Headers["Authorization"] = jwt;
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext,
            };
        }

        [Fact]
        public async void UpdateRole()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);
            var board = new Board() { Id = idGenerator.CreateId(), Name = "Test Name", SubscriptionType = Common.Enums.SubscriptionType.Enterprise };
            await boardQueries.CreateBoard(board);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "testID", Common.Enums.Role.Owner);

            var mockUserInfoUtil = new MockUserInfoUtil(new Auth0.AuthenticationApi.Models.UserInfo() { UserId = "testID" });
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var userController = new UserController(userQueries, configuration, null, mockUserInfoUtil);


            MockUserAuthentication(userController, "jwt");


            var result = await userController.UpdateRole("testID", board.Id, Common.Enums.Role.Viewer);

            var okResult = result as OkResult;
            Assert.Equal(okResult.StatusCode, 200);
        }

        [Fact]
        public async void UpdateRole_Invalid_Session()
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
            var userController = new UserController(userQueries, configuration, null, mockUserInfoUtil);


            MockUserAuthentication(userController, "jwt");


            var result = await userController.UpdateRole("testID", board.Id, Common.Enums.Role.Viewer);

            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.Equal(unauthorizedResult.StatusCode, 401);
            Assert.Equal(unauthorizedResult.Value, "Token is not associated with a session");
        }

        [Fact]
        public async void UpdateRole_Insufficient_Permissions()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);
            var board = new Board() { Id = idGenerator.CreateId(), Name = "Test Name", SubscriptionType = Common.Enums.SubscriptionType.Enterprise };
            await boardQueries.CreateBoard(board);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "testID", Common.Enums.Role.Owner);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "alternateTestID", Common.Enums.Role.Viewer);

            var mockUserInfoUtil = new MockUserInfoUtil(new Auth0.AuthenticationApi.Models.UserInfo() { UserId = "alternateTestID" });
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var userController = new UserController(userQueries, configuration, null, mockUserInfoUtil);


            MockUserAuthentication(userController, "jwt");


            var result = await userController.UpdateRole("testID", board.Id, Common.Enums.Role.Viewer);

            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.Equal(unauthorizedResult.StatusCode, 401);
            Assert.Equal(unauthorizedResult.Value, "Insufficient rights");

        }

        [Fact]
        public async void UpdateRole_Not_Member_Of_Board()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);
            var board = new Board() { Id = idGenerator.CreateId(), Name = "Test Name", SubscriptionType = Common.Enums.SubscriptionType.Enterprise };
            await boardQueries.CreateBoard(board);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "testID", Common.Enums.Role.Owner);

            var mockUserInfoUtil = new MockUserInfoUtil(new Auth0.AuthenticationApi.Models.UserInfo() { UserId = "alternateTestID" });
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var userController = new UserController(userQueries, configuration, null, mockUserInfoUtil);


            MockUserAuthentication(userController, "jwt");


            var result = await userController.UpdateRole("testID", board.Id, Common.Enums.Role.Viewer);

            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.Equal(unauthorizedResult.StatusCode, 401);
            Assert.Equal(unauthorizedResult.Value, "Requestee not a member of this board");

        }

        [Fact]
        public async void Remove()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);
            var board = new Board() { Id = idGenerator.CreateId(), Name = "Test Name", SubscriptionType = Common.Enums.SubscriptionType.Enterprise };
            await boardQueries.CreateBoard(board);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "testID", Common.Enums.Role.Owner);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "alternateTestID", Common.Enums.Role.Owner);

            var mockUserInfoUtil = new MockUserInfoUtil(new Auth0.AuthenticationApi.Models.UserInfo() { UserId = "testID" });
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var userController = new UserController(userQueries, configuration, null, mockUserInfoUtil);


            MockUserAuthentication(userController, "jwt");


            var result = await userController.Remove(board.Id, "alternateTestID");

            var okResult = result as OkResult;
            Assert.Equal(okResult.StatusCode, 200);
        }

        [Fact]
        public async void Remove_Requestee_Not_A_Member()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);
            var board = new Board() { Id = idGenerator.CreateId(), Name = "Test Name", SubscriptionType = Common.Enums.SubscriptionType.Enterprise };
            await boardQueries.CreateBoard(board);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "alternateID", Common.Enums.Role.Owner);


            var mockUserInfoUtil = new MockUserInfoUtil(new Auth0.AuthenticationApi.Models.UserInfo() { UserId = "testID" });
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var userController = new UserController(userQueries, configuration, null, mockUserInfoUtil);


            MockUserAuthentication(userController, "jwt");


            var result = await userController.Remove(board.Id, "alternateTestID");

            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.Equal(unauthorizedResult.StatusCode, 401);
            Assert.Equal(unauthorizedResult.Value, "Requestee not a member of this board");
        }

        [Fact]
        public async void Remove_Insufficient_Rights()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);
            var board = new Board() { Id = idGenerator.CreateId(), Name = "Test Name", SubscriptionType = Common.Enums.SubscriptionType.Enterprise };
            await boardQueries.CreateBoard(board);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "testID", Common.Enums.Role.Viewer);


            var mockUserInfoUtil = new MockUserInfoUtil(new Auth0.AuthenticationApi.Models.UserInfo() { UserId = "testID" });
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var userController = new UserController(userQueries, configuration, null, mockUserInfoUtil);


            MockUserAuthentication(userController, "jwt");


            var result = await userController.Remove(board.Id, "alternateTestID");

            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.Equal(unauthorizedResult.StatusCode, 401);
            Assert.Equal(unauthorizedResult.Value, "Insufficient rights");
        }

        [Fact]
        public async void Remove_Invalid_Session()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);
            var board = new Board() { Id = idGenerator.CreateId(), Name = "Test Name", SubscriptionType = Common.Enums.SubscriptionType.Enterprise };
            await boardQueries.CreateBoard(board);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "testID", Common.Enums.Role.Viewer);


            var mockUserInfoUtil = new MockUserInfoUtil(null);
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var userController = new UserController(userQueries, configuration, null, mockUserInfoUtil);


            MockUserAuthentication(userController, "jwt");


            var result = await userController.Remove(board.Id, "alternateTestID");

            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.Equal(unauthorizedResult.StatusCode, 401);
            Assert.Equal(unauthorizedResult.Value, "Token is not associated with a session");
        }

        [Fact]
        public async void GetAll()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);
            var board = new Board() { Id = idGenerator.CreateId(), Name = "Test Name", SubscriptionType = Common.Enums.SubscriptionType.Enterprise };
            await boardQueries.CreateBoard(board);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "testID", Common.Enums.Role.Owner);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "alternateTestID", Common.Enums.Role.Owner);

            var mockUserInfoUtil = new MockUserInfoUtil(new Auth0.AuthenticationApi.Models.UserInfo() { UserId = "testID" });
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var userController = new UserController(userQueries, configuration, null, mockUserInfoUtil);


            MockUserAuthentication(userController, "jwt");


            var result = await userController.GetAll(board.Id);

            var okResult = result as OkObjectResult;
            Assert.Equal(okResult.StatusCode, 200);
            Assert.Equal(((IEnumerable<UserRoleRelation>)okResult.Value).Count(), 2);
        }

        [Fact]
        public async void GetAll_Requestee_Not_A_Member()
        {
            var idGenerator = new IdGenerator(123);
            var db = GetDatabase(CreateNewContextOptions());
            var boardQueries = new BoardQueries(db);
            var userQueries = new UserQueries(db);
            var board = new Board() { Id = idGenerator.CreateId(), Name = "Test Name", SubscriptionType = Common.Enums.SubscriptionType.Enterprise };
            await boardQueries.CreateBoard(board);
            await userQueries.AddUser(board.Id, idGenerator.CreateId().ToString(), "testID", Common.Enums.Role.Owner);

            var mockUserInfoUtil = new MockUserInfoUtil(new Auth0.AuthenticationApi.Models.UserInfo() { UserId = "alternateTestID" });
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

            configuration["authURL"] = "https://TestURL.com";
            var userController = new UserController(userQueries, configuration, null, mockUserInfoUtil);


            MockUserAuthentication(userController, "jwt");


            var result = await userController.GetAll(board.Id);

            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.Equal(unauthorizedResult.StatusCode, 401);
            Assert.Equal(unauthorizedResult.Value, "Requestee not a member of this board");
        }

        [Fact]
        public async void GetAll_Invalid_Session()
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
            var userController = new UserController(userQueries, configuration, null, mockUserInfoUtil);


            MockUserAuthentication(userController, "jwt");


            var result = await userController.GetAll(board.Id);

            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.Equal(unauthorizedResult.StatusCode, 401);
            Assert.Equal(unauthorizedResult.Value, "Token is not associated with a session");
        }
    }
}
