using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Photos")] // change the name of the table
public class Photo
{
    public int Id { get; set; }

    public required string Url { get; set; }
    public bool IsMain { get; set; }
    public bool IsApproved { get; set; }
    public string? PublicId { get; set; }

    // Nav properties -------------------------------------------------
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;
    // ----------------------------------------------------------------
}