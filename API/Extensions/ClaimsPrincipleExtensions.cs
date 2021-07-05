using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            var username = user.FindFirst("name").Value;
            return username;
        }
         public static int GetUserId(this ClaimsPrincipal user)
        {
            var Id = user.FindFirst(ClaimTypes.NameIdentifier).Value;
            return int.Parse(Id);
        }
    }
}