using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repos
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
        {
            var users = _context.Users.OrderBy(o => o.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (likeParams.Predicate == "liked")
            {
                likes = likes.Where(w => w.SourceUserId == likeParams.UserId);
                users = likes.Select(s => s.LikedUser);
            }


            if (likeParams.Predicate == "likedBy")
            {
                likes = likes.Where(w => w.LikedUserId == likeParams.UserId);
                users = likes.Select(s => s.SourceUser);
            }

            var likedUsers =  users.Select(s => new LikeDto
            {
                Id = s.Id,
                Username = s.UserName,
                KnownAs = s.KnownAs,
                PhotoUrl = s.Photos.FirstOrDefault(f => f.IsMain).Url,
                Age = s.DateOfBirth.CalculateAge(),
                City = s.City
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likeParams.PageNumber, 
            likeParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users.Include(i => i.LikedUsers)
            .FirstOrDefaultAsync(f => f.Id == userId);
        }

    }
}