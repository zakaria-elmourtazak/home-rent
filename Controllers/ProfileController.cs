
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMvcAuthProject.Models;

public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // [Authorize]
    // public async Task<IActionResult> Profile()
    // {
    //     var user = await _userManager.GetUserAsync(User);

    //     var model = new ProfileViewModel
    //     {
    //         FullName = user.FullName,
    //         TimeZone = user.TimeZone,
    //         Bio = user.Bio,
    //         ProfileImageUrl = user.ProfileImageUrl
    //     };

    //     return View(model);
    // }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Edit(ProfileViewModel model, IFormFile? profileImage)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }
        if (profileImage != null)
        {
            var fileName = $"{user.Id}{Path.GetExtension(profileImage.FileName)}";
            var path = Path.Combine("wwwroot/images/profiles", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            user.ProfileImageUrl = $"/images/profiles/{fileName}";
        }

        user.UserName = model.UserName;
        user.TimeZone = model.TimeZone;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        user.Bio = model.Bio;
Console.WriteLine("here email "+model.Email);
        await _userManager.UpdateAsync(user);

        return RedirectToAction("Profile", "Home", new ProfileViewModel
        {
            UserName = user.UserName,
            TimeZone = user.TimeZone,
            Bio = user.Bio,
            ProfileImageUrl = user.ProfileImageUrl
        });
    }
}