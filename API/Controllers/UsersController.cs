using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService): BaseAPIController    
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
    {
        userParams.CurrentUsername = User.GetUsername();
        var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);
        Response.AddPaginationHeader(users.CurrentPage, users);
        return Ok(users);
    }
    [HttpGet("{username}")] // api/users/username
    public async Task<ActionResult<MemberDTO>> GetUser(string username)
    {
        var user = await unitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser: User.GetUsername() == username);

        if (user == null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if(user == null)
        {
            return BadRequest("User not found");
        }

        mapper.Map(memberUpdateDTO, user); // ef automaticaly tracks the changes and updates accordingly

        if(await unitOfWork.Complete())
        {
            return NoContent();
        }
        else
        {
            return BadRequest("Failed to update user");
        }
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if(user == null)
        {
            return BadRequest("User not found");
        }

        var result = await photoService.AddPhotoAsync(file);

        if(result.Error != null)
        {
            return BadRequest(result.Error.Message);
        }

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        user.Photos.Add(photo);

        if(await unitOfWork.Complete())
        {
            return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, mapper.Map<PhotoDTO>(photo));
        }

        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if(user == null)
        {
            return BadRequest("User not found");
        }

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if(photo == null || photo.IsMain)
        {
            return NotFound("Cannot use this as main photo");
        }
        // get the current main photo
        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if(currentMain != null)
        {
            currentMain.IsMain = false;
        }

        photo.IsMain = true;


        if(await unitOfWork.Complete())
        {
            return NoContent();
        }

        return BadRequest("Failed to set main photo");
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await unitOfWork.UserRepository.GetUserByPhotoIdAsync(photoId);
        
        if(user == null)
        {
            return BadRequest("User not found");
        }

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if(photo == null || photo.IsMain)
        {
            return BadRequest("Cannot delete this photo");
        }

        if(photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
        }

        user.Photos.Remove(photo);
        if(await unitOfWork.Complete())
        {
            return Ok();
        }

        return BadRequest("Failed to delete photo");
    }
}
