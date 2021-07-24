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
        private readonly IUnitOfWork _unitOfWork;
        public LikesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);
            var likedUser = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("you cann't like yourself!!");

            var userLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("you already liked this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);
            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("you failed to like user");

        }
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikeParams likeParams)
        {
            likeParams.UserId = User.GetUserId();
            var users = await _unitOfWork.LikesRepository.GetUserLikes(likeParams);

            Response.AddPaginationHeader(likeParams.PageNumber, likeParams.PageSize,
            users.TotalCount, users.TotalPages);

            return Ok(users);
        }
    }
}