using Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(string boardId) {
            
            return Ok();
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(string boardId, Settings settings)
        {
            return Ok();
        }
    }
}
