using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _context = context;

        }
        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login(RegisterViewModel model)
        {
            var user = await _context.Users
            .Include(a => a.Photos)
            .FirstOrDefaultAsync(a => a.UserName.ToLower() == model.Username.ToLower());
            if (user == null)
                return Unauthorized("Invalid username or password");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var loginPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));

            for (var i = 0; i < loginPasswordHash.Length; i++)
                if (loginPasswordHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid username or password");

            return new UserToken()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(a => a.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserToken>> Register(RegisterViewModel model)
        {
            if (await UserExists(model.Username))
                return BadRequest("The username is already exists!!");

            var user = _mapper.Map<AppUser>(model);

            using var hmac = new HMACSHA512();

            user.UserName = model.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
            user.PasswordSalt = hmac.Key;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserToken()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };

        }

        public async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(a => a.UserName.ToLower() == username.ToLower());
        }
    }
}