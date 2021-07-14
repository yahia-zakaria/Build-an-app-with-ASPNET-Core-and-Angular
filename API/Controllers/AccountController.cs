using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper,
        UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _context = context;

        }
        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login(LoginViewModel model)
        {
            var user = await _context.Users
            .Include(a => a.Photos)
            .FirstOrDefaultAsync(a => a.UserName.ToLower() == model.Username.ToLower());
            if (user == null)
                return Unauthorized("Invalid username or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded) return Unauthorized();

            return new UserToken()
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(a => a.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserToken>> Register(RegisterViewModel model)
        {
            if (await UserExists(model.Username))
                return BadRequest("The username is already exists!!");

            var user = _mapper.Map<AppUser>(model);
            user.UserName = model.Username.ToLower();
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            result = await _userManager.AddToRoleAsync(user, "Member");

            if (!result.Succeeded) return BadRequest(result.Errors);

            return new UserToken()
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(a => a.UserName.ToLower() == username.ToLower());
        }
    }
}