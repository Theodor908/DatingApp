using System.Security.Claims;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsersController(IUserRepository userRepository, IMapper mapper): BaseAPIController    
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
    {
        var users = await userRepository.GetMembersAsync();
        return Ok(users);
    }

    [HttpGet("{username}")] // api/users/3
    public async Task<ActionResult<MemberDTO>> GetUser(string username)
    {
        var user = await userRepository.GetMemberAsync(username);

        if (user == null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(username == null)
        {
            return BadRequest("Username not found");
        }

        var user = await userRepository.GetUserByUsernameAsync(username);

        if(user == null)
        {
            return BadRequest("User not found");
        }

        mapper.Map(memberUpdateDTO, user); // ef automaticaly tracks the changes and updates accordingly

        if(await userRepository.SaveAllAsync())
        {
            return NoContent();
        }
        else
        {
            return BadRequest("Failed to update user");
        }
    }
}
