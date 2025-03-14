using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser : IdentityUser<int>
{
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
    public ICollection<Photo> Photos { get; set; } = [];
    public List<UserLike> LikedUsers { get; set; } = [];
    public List<UserLike> LikedByUsers { get; set; } = [];
    public List<Message> Messages { get; set; } = [];
    public List<Message> MessagesRecieved { get; set; } = [];
    public ICollection<AppUserRole> UserRoles { get; set; } = [];

}
