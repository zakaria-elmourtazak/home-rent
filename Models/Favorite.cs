
using Microsoft.AspNetCore.Identity;
namespace MyMvcAuthProject.Models;
public class Favorite
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public ApplicationUser? User { get; set; }

    public int PropertyId { get; set; }
    public Property? Property { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
