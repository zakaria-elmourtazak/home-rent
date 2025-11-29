namespace MyMvcAuthProject.Models;
public class ChangePasswordViewModel
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}