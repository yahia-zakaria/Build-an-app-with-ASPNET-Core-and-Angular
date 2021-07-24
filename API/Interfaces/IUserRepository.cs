using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUserNameAsync(string username);
        Task<MemberDto> GetMemberByUserNameAsync(string username);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userPrams);
        Task<string> GetUserGender(string username);
    }
}