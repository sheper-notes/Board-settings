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
        public IMemoryCache Cache { get; set; }
        private Auth0AccessTokenManager Auth0AccessTokenManager { get; set; }
        public UserController(IUserQueries userQueries, IMemoryCache memoryCache, Auth0AccessTokenManager auth0AccessTokenManager) {
            UserQueries = userQueries;
            Cache = memoryCache;
            Auth0AccessTokenManager = auth0AccessTokenManager;
        }

        [HttpPost()]
        public async Task<IActionResult> Add(long boardId, string email, Role role)
        {
            var currentUser = await UserInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], Cache.Get("authURL").ToString());

            if (currentUser == null)
                return NotFound();

            var requestingUser = await UserQueries.GetUser(boardId, currentUser.UserId);
            if (requestingUser.Role != Role.Owner)
            {
                return Unauthorized();
            }

            var token = await Auth0AccessTokenManager.Get();
            var client = new ManagementApiClient(token, new Uri(Cache.Get("authURL").ToString()));
            var res = await client.Users.GetUsersByEmailAsync(email);

            if (res.Count < 1)
                return NotFound();
            
            return await UserQueries.AddUser(boardId, "", res.FirstOrDefault().UserId, role) == true ? Ok() : BadRequest();
            
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateRole(string userId, long boardId, Role role)
        {
            var currentUser = await UserInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], Cache.Get("authURL").ToString());

            if(currentUser == null) return NotFound();

            var requestingUser = await UserQueries.GetUser(boardId, currentUser.UserId);
            if(requestingUser.Role != Role.Owner)
            {
                return Unauthorized();
            }
            return await UserQueries.ChangeUserRole(boardId, userId, role) == true ? Ok() : BadRequest();
        }

        [HttpDelete("{boardId}/{userId}")]
        public async Task<IActionResult> Remove(long boardId, string userId)
        {
            var currentUser = await UserInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], Cache.Get("authURL").ToString());

            if (currentUser == null) return NotFound();

            var requestingUser = await UserQueries.GetUser(boardId, currentUser.UserId);
            if (requestingUser.Role != Role.Owner)
            {
                return Unauthorized();
            }

            var result = await UserQueries.RemoveUser(boardId, userId);
            if (result)
                return BadRequest();
            return Ok();
        }

        [HttpGet("{boardId}")]
        public async Task<IActionResult> GetAll(long boardId)
        {
            var currentUser = await UserInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], Cache.Get("authURL").ToString());

            if (currentUser == null) return NotFound();

            var requestingUser = await UserQueries.GetUser(boardId, currentUser.UserId);
            if (requestingUser == null)
            {
                return Unauthorized();
            }

            var result = await UserQueries.GetUsers(boardId);
            if (result.Count() == 0)
                return NotFound();
            return Ok();
        }

        [HttpGet("{boardId}/{userId}")]
        public async Task<IActionResult> GetUser(long boardId, string userId)
        {
            var currentUser = await UserInfoUtil.GetUserInfo(HttpContext.Request.Headers["Authorization"], Cache.Get("authURL").ToString());

            if (currentUser == null)
            {
                return NotFound();
            }

            var requestingUser = await UserQueries.GetUser(boardId, currentUser.UserId);
            if (requestingUser == null)
            {

                return Unauthorized();
            }

            if (requestingUser.Role != Role.Owner)
            {
                return Unauthorized();
            }

            var result = await UserQueries.GetUser(boardId, userId);
            if (result == null)
            {
                return NotFound();
            }
                
            return Ok(result);
        }
    }
}
