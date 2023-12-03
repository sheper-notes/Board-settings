using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Common;
using Common.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private IBoardQueries BoardQueries { get; set; }
        public BoardController(IBoardQueries boardQueries) {
            BoardQueries = boardQueries;
        }

        [HttpGet]
        public async Task<IActionResult> GetBoards()
        {
            var currentUser = await UserInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"]);

            if (currentUser == null) return NotFound();

            return Ok(await BoardQueries.GetBoardsForUser(currentUser.UserId));
        }
    }
}
