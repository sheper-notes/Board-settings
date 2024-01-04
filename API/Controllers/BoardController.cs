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

            return Ok(await BoardQueries.GetBoardsForUser(currentUser.UserId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBoard(Board board)
        {
            var currentUser = await userInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], configuration.GetValue<string>("authURL"));

            if (currentUser == null)
                return Unauthorized();
            board.Id = IdGenerator.CreateId();
            if(!await BoardQueries.CreateBoard(board))
            {
                return BadRequest();
            }

            if (!await UserQueries.AddUser(board.Id, IdGenerator.CreateId().ToString(), currentUser.UserId, Common.Enums.Role.Owner))
            {
                return BadRequest();
            }
            return Ok(board.Id);
        }
    }
}
