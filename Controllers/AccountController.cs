
using Microsoft.AspNetCore.Mvc;
using MyMvcAuthProject.Models;
using MyMvcAuthProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
public class AccountController : Controller
{

    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    // [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginAccount(LoginViewModel model)
    {
if (model == null || !ModelState.IsValid)
        {
            TempData["LoginError"] = "Please correct the form errors and try again.";
            TempData["Email"] = model?.Email;
            return RedirectToAction("Login");
        }
        // try to find user and sign in using SignInManager (handles lockout/2fa)
        var user = await _userManager.FindByEmailAsync(model.Email);
         if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
try{

            await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
                _logger.LogInformation("User logged in.");
}catch(Exception ex)
{
                TempData["LoginError"] = "Invalid email or password.";
}
        }
        else
        {
            // don't reveal whether the email exists
            TempData["LoginError"] = "Invalid email or password.";
        }

        // preserve entered email so login view can re-populate the field
        TempData["Email"] = model.Email;
        return RedirectToAction("Login");
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAccount(RegisterViewModel model)
    {
        if (model == null)
        {
            _logger.LogWarning("RegisterAccount called with null model.");
            ModelState.AddModelError(string.Empty, "Invalid registration data.");
            TempData["RegisterError"] = "Invalid registration data.";
            return View("Register", model);
        }

        if (!ModelState.IsValid)
        {
            TempData["RegisterError"] = "Please correct the form errors and try again.";
            return View("Register", model);
        }

        try
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User logged in after registration.");
                return RedirectToAction("Index", "Home");
            }

            // add identity errors to ModelState so view shows them
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            TempData["RegisterError"] = "Registration failed. See errors below.";
            return View("Register", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during registration.");
            ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
            TempData["RegisterError"] = "An unexpected error occurred. Please try again later.";
            return View("Register", model);
        }
        }

        // // If we got this far, something failed, redisplay form
        // return View(model);
       
    // }
}