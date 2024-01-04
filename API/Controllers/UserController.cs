using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Common;
using Common.Enums;
using Common.Interfaces;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net.Http.Headers;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IUserQueries UserQueries { get; set; }
        IConfiguration configuration;
        private Auth0AccessTokenManager Auth0AccessTokenManager { get; set; }
        private IUserInfoUtil userInfoUtil { get; set; }
        public UserController(IUserQueries userQueries, IConfiguration configuration, Auth0AccessTokenManager auth0AccessTokenManager, IUserInfoUtil userInfoUtil) {
            UserQueries = userQueries;
            Auth0AccessTokenManager = auth0AccessTokenManager;
            this.userInfoUtil = userInfoUtil;
            this.configuration = configuration;
        }

        [HttpPost()]
        public async Task<IActionResult> Add(long boardId, string email, Role role)
        {
            var currentUser = await userInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], configuration.GetValue<string>("authURL"));

            if (currentUser == null)
                return NotFound();

            var requestingUser = await UserQueries.GetUser(boardId, currentUser.UserId);
            if (requestingUser.Role != Role.Owner)
            {
                return Unauthorized();
            }

            var token = await Auth0AccessTokenManager.Get();
            var client = new ManagementApiClient(token, new Uri(configuration.GetValue<string>("authURL").ToString()));
            var res = await client.Users.GetUsersByEmailAsync(email);

            if (res.Count < 1)
                return NotFound();
            
            return await UserQueries.AddUser(boardId, "", res.FirstOrDefault().UserId, role) == true ? Ok() : BadRequest();
            
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateRole(string userId, long boardId, Role role)
        {
            var currentUser = await userInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], configuration.GetValue<string>("authURL").ToString());

            if(currentUser == null) return Unauthorized("Token is not associated with a session");

            var requestingUser = await UserQueries.GetUser(boardId, currentUser.UserId);
            if(requestingUser == null)
            {
                return Unauthorized("Requestee not a member of this board");
            }

            if(requestingUser.Role != Role.Owner)
            {
                return Unauthorized("Insufficient rights");
            }
            return await UserQueries.ChangeUserRole(boardId, userId, role) == true ? Ok() : BadRequest("An error occured saving the change");
        }

        [HttpDelete("{boardId}/{userId}")]
        public async Task<IActionResult> Remove(long boardId, string userId)
        {
            var currentUser = await userInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], configuration.GetValue<string>("authURL").ToString());

            if (currentUser == null) return Unauthorized("Token is not associated with a session");

            var requestingUser = await UserQueries.GetUser(boardId, currentUser.UserId);
            if (requestingUser == null)
            {
                return Unauthorized("Requestee not a member of this board");
            }

            if (requestingUser.Role != Role.Owner)
            {
                return Unauthorized("Insufficient rights");
            }

            var result = await UserQueries.RemoveUser(boardId, userId);
            if (result == false)
                return BadRequest("An error occured saving the change");
            return Ok();
        }

        [HttpGet("{boardId}")]
        public async Task<IActionResult> GetAll(long boardId)
        {
            var currentUser = await userInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], configuration.GetValue<string>("authURL").ToString());

            if (currentUser == null) return Unauthorized("Token is not associated with a session");

            var requestingUser = await UserQueries.GetUser(boardId, currentUser.UserId);
            if (requestingUser == null)
            {
                return Unauthorized("Requestee not a member of this board");
            }

            var result = await UserQueries.GetUsers(boardId);
            return Ok(result);
        }
    }
}
