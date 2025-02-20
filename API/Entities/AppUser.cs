using System;

namespace API.Entities;
using API.Extensions;

public class AppUser
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public byte[] PasswordHash {get; set; } = [];
    public byte[] PasswordSalt {get; set; } = [];
    public DateOnly DateOfBirth { get; set; }
    public required string KnownAs { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public required string Gender { get; set; }
    public required string? Introduction { get; set; }
    public required string? Interests { get; set; }
    public required string? LookingFor { get; set; } // new parameter
    public required string City { get; set; }
    public required string Country { get; set; }
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();

}
