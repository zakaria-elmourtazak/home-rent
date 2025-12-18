using Microsoft.AspNetCore.Identity;
namespace MyMvcAuthProject.Models;

public class ApplicationUser : IdentityUser
{
    public string? TimeZone { get; set; }
    public string? Bio { get; set; }

    public string? ProfileImageUrl { get; set; }

    public ICollection<Property> Properties { get; set; } = new List<Property>();
}