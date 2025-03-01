using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager) : BaseAPIController
{
    [Authorize(Policy = "RequireAdminRole")]  // beware of evil typos
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users
            .OrderBy(user => user.UserName)
            .Select(user => new{
                user.Id,
                Username = user.UserName,
                Roles = user.UserRoles.Select(userRole => userRole.Role.Name).ToList()
            }).ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]  // beware of evil typos
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if(string.IsNullOrEmpty(roles))
        {
            return BadRequest("Role is required");
        }
        var selectedRoles = roles.Split(",").ToArray();
        var user = await userManager.FindByNameAsync(username);
        if(user == null)
        {
            return BadRequest("User not found");
        }

        var userRoles = await userManager.GetRolesAsync(user);
        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles)); // add roles that are not already in userRoles
        if(result.Succeeded == false)
        {
            return BadRequest("Failed to add to roles");
        }

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles)); // remove roles that are not in selectedRoles

        if(result.Succeeded == false)
        {
            return BadRequest("Failed to remove from roles");
        }

        return Ok(await userManager.GetRolesAsync(user));

    }

    [Authorize(Policy = "ModeratePhotoRole")]  // beware of evil typos
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Only moderators can see this");
    }
    
}
