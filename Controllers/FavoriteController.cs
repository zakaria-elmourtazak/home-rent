using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMvcAuthProject.Data;
using MyMvcAuthProject.Models;

// [Authorize]
public class FavoriteController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public FavoriteController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }


    [HttpPost]
    public async Task<IActionResult> AddFavorite(int id)
    {
        // var user = await _userManager.GetUserAsync(User);
var userId = "user " + id;
        bool exists = _db.Favorites.Any(f => f.UserId == userId && f.PropertyId == id);
        if (exists)
            return RedirectToAction("Details", "Property", new { id });

        var favorite = new Favorite
        {
            UserId = userId,
            PropertyId = id
        };

        _db.Favorites.Add(favorite);
        await _db.SaveChangesAsync();

        return RedirectToAction("PropertyDetail", "Home", new { id });
    }


    [HttpPost]
    public async Task<IActionResult> RemoveFavorite(int id)
    {
        // var user = await _userManager.GetUserAsync(User);
var userId = "user " + id;
        var favorite = _db.Favorites.FirstOrDefault(f => f.UserId == userId && f.PropertyId == id);
        if (favorite != null)
        {
            _db.Favorites.Remove(favorite);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction("Saved", "Home");
    }


    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Delete(int id)
    // {
    //     // var user = await _userManager.GetUserAsync(User);
    //     var userId = "user" + id;
    //     var favorite = _db.Favorites.FirstOrDefault(f => f.UserId == userId && f.PropertyId == id);
    //     if (favorite != null)
    //     {
    //         _db.Favorites.Remove(favorite);
    //         await _db.SaveChangesAsync();
    //     }

    //     return RedirectToAction("Saved", "Home");
    // }
}