
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
  private IActionResult ReturnErrors(IEnumerable<string> errors)
    {
        var list = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList() ?? new List<string>();

        // Return JSON when request is AJAX
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return BadRequest(new { success = false, errors = list });
        }

        // otherwise, put the errors in TempData for display on the Profile view
        TempData["Errors"] = string.Join("|", list);
        return RedirectToAction("Profile", "Home");
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
        var result  = await _userManager.UpdateAsync(user);

         if (!result.Succeeded)
        {
            var errs = result.Errors.Select(e => e.Description);
            return ReturnErrors(errs);
        }

        TempData["Success"] = "Profile updated successfully";

        // return JSON for AJAX or redirect otherwise
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Ok(new { success = true });

        return RedirectToAction("Profile", "Home", new ProfileViewModel
        {
            UserName = user.UserName,
            TimeZone = user.TimeZone,
            Bio = user.Bio,
            ProfileImageUrl = user.ProfileImageUrl
        });
    }


 [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangeImage(IFormFile profileImage)
    {
        Console.WriteLine("ChangeImage called");
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
        var result  = await _userManager.UpdateAsync(user);

         if (!result.Succeeded)
        {
            var errs = result.Errors.Select(e => e.Description);
            return ReturnErrors(errs);
        }
    Console.WriteLine("Profile image updated:", user.ProfileImageUrl);

        TempData["Success"] = "Profile updated successfully";

        // return JSON for AJAX or redirect otherwise
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Ok(new { success = true });
        return RedirectToAction("Profile", "Home", new ProfileViewModel
        {
            UserName = user.UserName,
            TimeZone = user.TimeZone,
            Bio = user.Bio,
            ProfileImageUrl = user.ProfileImageUrl
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login");
        if (!ModelState.IsValid)
            return ReturnErrors(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            var errs = result.Errors.Select(e => e.Description);
            return ReturnErrors(errs);
        }

        TempData["Success"] = "Password updated successfully";
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Ok(new { success = true });

        return RedirectToAction("Profile", "Home", new ProfileViewModel
        {
            UserName = user.UserName,
            TimeZone = user.TimeZone,
            Bio = user.Bio,
            ProfileImageUrl = user.ProfileImageUrl
        });
    }
}