// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers;
using Auth0.ManagementApi;

using Common;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class GDPRController : ControllerBase
{

    public IUserQueries UserQueries { get; set; }
    private IBoardQueries BoardQueries { get; set; }
    IConfiguration configuration;
    private Auth0AccessTokenManager Auth0AccessTokenManager { get; set; }
    private IUserInfoUtil userInfoUtil { get; set; }
    private ILogger Logger { get; set; }
    private IBoardDBService BoardDBService { get; set; }
    public GDPRController(IUserQueries userQueries, IConfiguration configuration, Auth0AccessTokenManager auth0AccessTokenManager, IUserInfoUtil userInfoUtil, ILogger logger, IBoardQueries boardQueries, IBoardDBService boardDBService)
    {
        UserQueries = userQueries;
        Auth0AccessTokenManager = auth0AccessTokenManager;
        this.userInfoUtil = userInfoUtil;
        this.configuration = configuration;
        Logger = logger;
        BoardQueries = boardQueries;
        BoardDBService = boardDBService;
    }
    // DELETE api/<GDPRController>/5
    [HttpDelete("all")]
    public async Task<IActionResult> Delete()
    {
        var currentUser = await userInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], configuration.GetValue<string>("authURL"));

        if (currentUser == null)
            return Unauthorized();

        var userBoards = await BoardQueries.GetBoardsForUser(currentUser.UserId);

        List<Task> tasks = new List<Task>();

        foreach (var board in userBoards)
        {
            var user = board.Users.Where(x=>x.UserId == currentUser.UserId).FirstOrDefault();

            tasks.Add(DeleteBoard(user, board.Id));
        }

        await Task.WhenAll(tasks);
        if(tasks.Count(x => x.IsFaulted) > 0)
        {
            return BadRequest("Not all boards were deleted");
        }
        return Ok();
    }

    [HttpDelete("account")]
    public async Task<IActionResult> DeleteAccount()
    {
        var currentUser = await userInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], configuration.GetValue<string>("authURL"));

        if (currentUser == null)
            return Unauthorized();

        var userBoards = await BoardQueries.GetBoardsForUser(currentUser.UserId);
        if(userBoards.Count() > 0)
            return BadRequest("User still has data which has to be dealt with.");

        var token = await Auth0AccessTokenManager.Get();
        var client = new ManagementApiClient(token, new Uri("https://sheper.eu.auth0.com/api/v2/"));//configuration.GetValue<string>("authURL").ToString()));
        try
        {
            await client.Users.DeleteAsync(currentUser.UserId);
        }
        catch (Exception)
        {
            return BadRequest("An error occurred deleting the account");
        }

        return Ok();
    }

    [HttpGet("account")]
    public async Task<IActionResult> GetAccount()
    {
        var currentUser = await userInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], configuration.GetValue<string>("authURL"));

        if (currentUser == null)
            return Unauthorized();

        return Ok(currentUser);
    }

    private async Task DeleteBoard(UserRoleRelation user, long boardId)
    {
        if(user.Role == Common.Enums.Role.Owner)
        {
            await BoardDBService.DeleteBoard(boardId, HttpContext.Request.Headers["Authorization"]);
            var board = await BoardQueries.GetBoardById(boardId);
            await BoardQueries.Delete(board);
        } else
        {
            var res = await UserQueries.RemoveUser(user.BoardId, user.UserId);
            if(res == false )
            {
                new Exception();
            }
        }

    }
}
