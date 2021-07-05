using System;
using System.Threading.Tasks;
using API.Data;
using API.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.ActionFilters
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var id = resultContext.HttpContext.User.GetUserId();
            var ctx = resultContext.HttpContext.RequestServices.GetService<DataContext>();
            var user = await ctx.Users.FindAsync(id);
            
            user.LastActive = DateTime.Now;
            await ctx.SaveChangesAsync();
        }
    }
}