using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(DataContext context)
    {
        if(await context.Users.AnyAsync()) return; // check if there are any users
        var userDate = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var users = JsonSerializer.Deserialize<List<AppUser>>(userDate, options);

        if(users == null) return;

        foreach(var user in users)
        {
            using var hmax = new HMACSHA512();
            user.UserName = user.UserName.ToLower();
            user.PasswordHash = hmax.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
            user.PasswordSalt = hmax.Key;

            context.Users.Add(user);
        }

        await context.SaveChangesAsync();

    }
}