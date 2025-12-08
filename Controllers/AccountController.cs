
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

        if (ModelState.IsValid)
        {
            // 1. Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);

            // 2. Check if user exists and password is correct
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {

                // 3. If all checks pass, sign the user in
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User logged in.");

                // 4. Redirect to the original page or home page
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // If login fails, tell the user
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }

        // If we got this far, something failed, redisplay form
        return View(model);
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
    public IActionResult RegisterAccount(RegisterViewModel model)
    {
        // if (ModelState.IsValid)
        // {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = _userManager.CreateAsync(user, model.Password).Result;

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                // Optionally sign the user in after registration
                _signInManager.SignInAsync(user, isPersistent: false).Wait();
                _logger.LogInformation("User logged in after registration.");

                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // // If we got this far, something failed, redisplay form
        // return View(model);
       
    // }
}