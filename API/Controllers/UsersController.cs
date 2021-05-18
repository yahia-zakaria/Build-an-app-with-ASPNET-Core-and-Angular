using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public IPhotoService _photoService { get; }

        public UsersController(DataContext context, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _context = context;
            _mapper = mapper;
        }
        //  api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> Get()
        {
            var users = await _context.Users
            .Include(a => a.Photos)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
            return Ok(_mapper.Map<IEnumerable<MemberDto>>(users));
        }
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> Get(string username)
        {
            var user = await _context.Users
            .Include(a => a.Photos)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(a => a.UserName.ToLower().Equals(username.ToLower()));
            return _mapper.Map<MemberDto>(user);

        }

        [HttpPost]
        public async Task<ActionResult> Update(UpdateMemberDto member)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var id = Convert.ToInt32(userId);
                var user = await _context.Users.FindAsync(id);

                _mapper.Map(member, user);

                if (await _context.SaveChangesAsync() > 0)
                {
                    return NoContent();
                }
                return BadRequest("Failed to update user");
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _context.Users.Include(i => i.Photos).FirstOrDefaultAsync(async => async.UserName == User.GetUserName());
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);
            if (await _context.SaveChangesAsync() > 0)
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));

            return BadRequest("There is problem when adding the photo");

        }

        [HttpPost("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _context.Users.Include(i => i.Photos).FirstOrDefaultAsync(async => async.UserName == User.GetUserName());
            var photo = user.Photos.FirstOrDefault(a => a.Id == photoId);

            if (photo.IsMain)
                return BadRequest("The chosen photo is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(a => a.IsMain);
            currentMain.IsMain = false;

            photo.IsMain = true;

            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }

            return BadRequest("something went wrong");
        }
        [HttpPost("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _context.Users.Include(i => i.Photos).FirstOrDefaultAsync(async => async.UserName == User.GetUserName());
            var photo = user.Photos.FirstOrDefault(a => a.Id == photoId);

            if (photo == null)
                return NotFound();

            if (photo.IsMain)
                return BadRequest("You cannot remove the main photo");

            if (!string.IsNullOrEmpty(photo.PublicId))
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                    return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            if (await _context.SaveChangesAsync() > 0)
                return NoContent();

            return BadRequest("something went wrong");
        }


    }
}