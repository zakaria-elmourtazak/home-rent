using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Username")]
    public required string Username { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public required string Email { get; set; }

    
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public required string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public required string ConfirmPassword { get; set; }
}