using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Common;
using Common.Interfaces;
using Common.Models;
using IdGen;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private IdGenerator IdGenerator { get; set; }
        private IBoardQueries BoardQueries { get; set; }
        private IUserQueries UserQueries { get; set; }
        private IMemoryCache Cache { get; set; }
        public BoardController(IdGenerator _IdGen, IBoardQueries boardQueries, IMemoryCache cache, IUserQueries _userQueries) {
            BoardQueries = boardQueries;
            Cache = cache;
            IdGenerator = _IdGen;
            UserQueries = _userQueries;
        }

        [HttpGet]
        public async Task<IActionResult> GetBoards()
        {
            var currentUser = await UserInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], Cache.Get("authURL").ToString());

            if (currentUser == null) return NotFound();

            return Ok(await BoardQueries.GetBoardsForUser(currentUser.UserId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBoard(Board board)
        {
            var currentUser = await UserInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], Cache.Get("authURL").ToString());

            if (currentUser == null)
                return NotFound();

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
