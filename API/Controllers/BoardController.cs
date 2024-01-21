using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Common;
using Common.Interfaces;
using Common.Models;
using IdGen;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private IdGenerator IdGenerator { get; set; }
        private IBoardQueries BoardQueries { get; set; }
        private IUserQueries UserQueries { get; set; }
        private IUserInfoUtil userInfoUtil { get; set; }
        private IConfiguration configuration { get; set; }
        public BoardController(IdGenerator _IdGen, IBoardQueries boardQueries, IUserQueries _userQueries, IUserInfoUtil userInfoUtil, IConfiguration configuration) {
            BoardQueries = boardQueries;
            IdGenerator = _IdGen;
            UserQueries = _userQueries;
            this.userInfoUtil = userInfoUtil;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetBoards()
        {
            var currentUser = await userInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], configuration.GetValue<string>("authURL"));

            if (currentUser == null) return Unauthorized();
            var boards = await BoardQueries.GetBoardsForUser(currentUser.UserId);
            foreach (var board in boards)
            {
                board.Users.ForEach(us =>  us.Board = null);
            }
            return Ok(boards);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBoard(Board board)
        {
            var currentUser = new UserInfo() { UserId = "google-oauth2|112145192005862567996" }; //await userInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], configuration.GetValue<string>("authURL"));

            if (currentUser == null)
                return Unauthorized("User not found");
            board.Id = IdGenerator.CreateId();
            if(!await BoardQueries.CreateBoard(board))
            {
                return BadRequest("Error creating board");
            }

            if (!await UserQueries.AddUser(board.Id, IdGenerator.CreateId().ToString(), currentUser.UserId, Common.Enums.Role.Owner))
            {
                return BadRequest("Error adding user to board");
            }
            return Ok(board.Id);
        }
    }
}
