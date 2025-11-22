using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string? TimeZone { get; set; }
    public string? Bio { get; set; }


    public string? ProfileImageUrl { get; set; }
}