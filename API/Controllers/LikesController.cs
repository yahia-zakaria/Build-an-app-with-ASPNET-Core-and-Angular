using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly ILikesRepository _repo;
        private readonly DataContext _context;
        public LikesController(ILikesRepository repo, DataContext context)
        {
            _context = context;
            _repo = repo;
        }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var sourceUser = await _repo.GetUserWithLikes(sourceUserId);
            var likedUser = await _context.Users.FirstOrDefaultAsync(f => f.UserName == username);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("you cann't like yourself!!");

            var userLike = await _repo.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("you already liked this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);
            if (await _context.SaveChangesAsync() > 0) return Ok();

            return BadRequest("you failed to like user");

        }
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikeParams likeParams)
        {
            likeParams.UserId = User.GetUserId();
            var users = await _repo.GetUserLikes(likeParams);

            Response.AddPaginationHeader(likeParams.PageNumber, likeParams.PageSize,
            users.TotalCount, users.TotalPages);

            return Ok(users);
        }
    }
}