using Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        [HttpGet("Status")]
        public async Task<IActionResult> GetSubscriptionStatus(string boardId)
        {
            return Ok();
        }

        [HttpPut("Change")]
        public async Task<IActionResult> ChangeSubscription(string boardId, SubscriptionType subscriptionType)
        {
            return Ok();
        }

        [HttpPost("Start")]
        public async Task<IActionResult> Test()
        {
            return Ok("Yeet");
        }
    }
}
