using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UsersController(DataContext context, IMapper mapper)
        {
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
        [HttpGet("{usersname}")]
        public async Task<ActionResult<MemberDto>> Get(string usersname)
        {
            var user = await _context.Users
            .Include(a => a.Photos)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(a => a.UserName.ToLower().Equals(usersname.ToLower()));
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
    }
}