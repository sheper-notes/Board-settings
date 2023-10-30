using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IUserLogic UserLogic { get; set; }
        public UserController(IUserLogic userLogic) { 
            UserLogic = userLogic;
        }

        [HttpPost()]
        public async Task<IActionResult> Add(string boardId, string userId, Role role)
        {
            return Ok();
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateRole(string userId, string boardId, Role role)
        {
            return Ok();
        }

        [HttpDelete("{boardId}/{userId}")]
        public async Task<IActionResult> Remove(long boardId, string userId)
        {
            var result = await UserLogic.RemoveUser(boardId, userId);
            if (result)
                return BadRequest();
            return Ok();
        }

        [HttpGet("{boardId}")]
        public async Task<IActionResult> GetAll(long boardId)
        {
            var result = await UserLogic.GetUsers(boardId);
            if (result.Count() == 0)
                return NotFound();
            return Ok();
        }

        [HttpGet("{boardId}/{userId}")]
        public async Task<IActionResult> GetUser(long boardId, string userId)
        {
            var result = await UserLogic.GetUser(boardId, userId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
