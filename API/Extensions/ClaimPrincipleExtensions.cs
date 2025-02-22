using System;
using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(username == null)
        {
            throw new Exception("Username not found");
        }
        else
        {
            return username;
        }
    }
}
