using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IPhotoService photoService) : BaseAPIController
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
    public async Task<ActionResult> GetPhotosForModeration([FromQuery] UserParams userParams)
    {
        var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotos(userParams);
        Response.AddPaginationHeader(photos.CurrentPage, photos);
        return Ok(photos);
    }
    [Authorize(Policy = "ModeratePhotoRole")] 
    [HttpPost("approve-photo/{id:int}")]
    public async Task<ActionResult> ApprovePhoto(int id)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(id);
        if(photo == null)
        {
            return NotFound("Photo not found");
        }

        photo.IsApproved = true;
        var user = await unitOfWork.UserRepository.GetUserByPhotoIdAsync(photo.Id);

        if(user == null)
        {
            return NotFound("User not found");
        }

        if(user.Photos.Any(p => p.IsMain) == false)
        {
            photo.IsMain = true;
        }

        if(await unitOfWork.Complete())
        {
            return Ok();
        }
        else
        {
            return BadRequest("Failed to approve photo");
        }
    
    }

    [Authorize(Policy = "ModeratePhotoRole")] 
    [HttpPost("reject-photo/{id:int}")]
    public async Task<ActionResult> RejectPhoto(int id)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(id);
        if(photo == null)
        {
            return NotFound("Photo not found");
        }
        if(photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Result == "ok")
            {
                unitOfWork.PhotoRepository.RemovePhoto(photo);
            }
        }
        else
        {
            unitOfWork.PhotoRepository.RemovePhoto(photo);
        }

        if(await unitOfWork.Complete())
        {
            return Ok();
        }
        else
        {
            return BadRequest("Failed to reject photo");
        }
    }
    
}
