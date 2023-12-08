using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Common;
using Common.Interfaces;
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
        private IBoardQueries BoardQueries { get; set; }
        private IMemoryCache Cache { get; set; }
        public BoardController(IBoardQueries boardQueries, IMemoryCache cache) {
            BoardQueries = boardQueries;
            Cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetBoards()
        {
            var currentUser = await UserInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], Cache.Get("authURL").ToString());

            if (currentUser == null) return NotFound();

            return Ok(await BoardQueries.GetBoardsForUser(currentUser.UserId));
        }
    }
}
