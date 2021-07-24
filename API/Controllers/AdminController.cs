

using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;

        }
        [HttpGet("users-with-roles")]
        [Authorize(Policy = "RequiredAdminRole")]
        public async Task<ActionResult> GetUsersWithroles()
        {
            var users = await _userManager.Users.Include(i => i.UserRoles).ThenInclude(i => i.Role)
            .OrderBy(o => o.UserName)
            .Select(s => new
            {
                s.Id,
                Username = s.UserName,
                Roles = s.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(',');
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound();

            var existingRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(existingRoles));

            if (!result.Succeeded) return BadRequest(result.Errors);

            result = await _userManager.RemoveFromRolesAsync(user, existingRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [HttpGet("photos-to-Moderate")]
        [Authorize(Policy = "ModeratePhotoRole")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins or moderators can see this");
        }
    }
}