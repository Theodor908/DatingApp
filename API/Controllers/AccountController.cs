using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using API.Data;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using AutoMapper;
using API.Entities;
namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService, IMapper mapper) : BaseAPIController
{
    [HttpPost("register")] // account/register
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {

        if(await UserExists(registerDTO.Username)) return BadRequest("Username is taken");

        using var hmax = new HMACSHA512();

        var user = mapper.Map<AppUser>(registerDTO);

        user.UserName = registerDTO.Username.ToLower();
        user.PasswordHash = hmax.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
        user.PasswordSalt = hmax.Key;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDTO
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
    }

    [HttpPost("login")] // account/login

    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {
        var user = await context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == loginDTO.Username.ToLower());
        
        if(user == null)
        {
            return Unauthorized("Invalid username");
        }

        using var hmax = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmax.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

        for(int i = 0; i < computedHash.Length; i++)
        {
            if(computedHash[i] != user.PasswordHash[i])
            {
                return Unauthorized("Invalid password");
            }
        }

        return new UserDTO
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };

    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}