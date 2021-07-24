using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repos
{
 public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(m => m.Photos).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.Users.Include(m => m.Photos).FirstOrDefaultAsync(m => m.UserName == username.ToLower());
        }
        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<MemberDto> GetMemberByUserNameAsync(string username)
        {
            return await _context.Users
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(m => m.UserName == username.ToLower());
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userPrams)
        {
            var query =  _context.Users.AsQueryable();
            query = query.Where(u=>u.UserName!=userPrams.CurrentUsername);
            query = query.Where(u=>u.Gender==userPrams.Gender);

            var minDob = DateTime.Today.AddYears(-userPrams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userPrams.MinAge);
            query=query.Where(u=>u.DateOfBirth>=minDob && u.DateOfBirth<=maxDob);

            query = userPrams.OrderBy switch{
                "created"=>query.OrderByDescending(u=>u.CreatedAt),
                _=>query.OrderByDescending(u=>u.LastActive)
            };
            // .AsNoTracking();

            return await PagedList<MemberDto>.CreateAsync(query
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking()
            ,userPrams.PageNumber,userPrams.PageSize);
        }

        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users.Where(u=>u.UserName==username).Select(u=>u.Gender).FirstOrDefaultAsync();
        }
    }
}