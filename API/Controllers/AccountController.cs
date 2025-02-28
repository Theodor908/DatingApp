using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using API.Data;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using AutoMapper;
using API.Entities;
using Microsoft.AspNetCore.Identity;
namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper) : BaseAPIController
{
    [HttpPost("register")] // account/register
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        if(registerDTO.Username == null)
        {
            return BadRequest("Username is required");
        }

        if(registerDTO.Password == null)
        {
            return BadRequest("Password is required");
        }

        if(await UserExists(registerDTO.Username)) return BadRequest("Username is taken");
        
        var user = mapper.Map<AppUser>(registerDTO);

        user.UserName = registerDTO.Username.ToLower();

        var result = await userManager.CreateAsync(user, registerDTO.Password);

        if(result.Succeeded == false) return BadRequest(result.Errors);

        return new UserDTO
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = await tokenService.CreateToken(user),
            Gender = user.Gender
        };
    }

    [HttpPost("login")] // account/login

    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {
        var user = await userManager.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.NormalizedUserName == loginDTO.Username.ToUpper());
        
        if(user == null || user.UserName == null)
        {
            return Unauthorized("Invalid username");
        }

        var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);

        if(result == false)
        {
            return Unauthorized("Invalid password");
        }

        return new UserDTO
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = await tokenService.CreateToken(user),
            Gender = user.Gender,
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };

    }

    private async Task<bool> UserExists(string username)
    {
        return await userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
    }
}