using Microsoft.AspNetCore.Mvc.Filters;
using API.Extensions;
using API.Interfaces;
namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if(context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            return;
        }
        var userId = resultContext.HttpContext.User.GetUserId();
        var userRepository = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        var user = await userRepository.GetUserByIdAsync(userId);
        if(user == null) return;
        user.LastActive = DateTime.UtcNow;
        await userRepository.SaveAllAsync();
    }
}