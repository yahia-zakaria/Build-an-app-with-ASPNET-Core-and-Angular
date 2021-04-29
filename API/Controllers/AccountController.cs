using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;

        }
        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login(RegisterViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.UserName == model.Username.ToLower());
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
                Token = _tokenService.CreateToken(user)
            };
        }
        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterViewModel model)
        {
            if (await UserExists(model.Username))
                return BadRequest("The username is already exists!!");

            using var hmac = new HMACSHA512();
            var user = new AppUser()
            {
                UserName = model.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;

        }

        public async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(a => a.UserName.ToLower() == username.ToLower());
        }
    }
}