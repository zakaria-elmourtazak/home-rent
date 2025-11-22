
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMvcAuthProject.Data;
using MyMvcAuthProject.Models;
using Microsoft.EntityFrameworkCore;

namespace MyMvcAuthProject.Controllers;

public class PropertyController : Controller
{

    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<HomeController> _logger;

    public PropertyController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger)
    {
        _logger = logger;
        _db = db;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Property model,IFormFile[] PropertyImages)
    {       
        Console.WriteLine("here");
         if (model == null) return BadRequest();

   var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(imagesDirectory))
            {
                Directory.CreateDirectory(imagesDirectory);
            }

            // Save images to the server
            if (PropertyImages != null && PropertyImages.Length > 0)
            {
                foreach (var image in PropertyImages)
                {
                    if (image.Length > 0)
                    {
                        var filePath = Path.Combine(imagesDirectory, image.FileName); // Adjust path as needed
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        // Add image path to the model
                        model.PropertyImages.Add(new PropertyImage { ImageUrl = $"/images/{image.FileName}" });
                    }
                }
            }


        // var user = await _userManager.GetUserAsync(User);
        // if (user == null) return Challenge();

        // model.UserId = user.Id;
        model.User = null; // avoid tracking nav property

        _db.Properties.Add(model);
        await _db.SaveChangesAsync();

        return RedirectToAction("AdminListings", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Properties
                              .Include(p => p.PropertyImages)
                              .FirstOrDefaultAsync(p => p.Id == id);

        if (entity == null) return NotFound();

        // remove image files from wwwroot if present
        if (entity.PropertyImages != null)
        {
            foreach (var img in entity.PropertyImages)
            {
                try
                {
                    var relative = (img?.ImageUrl ?? string.Empty).TrimStart('/');
                    if (string.IsNullOrWhiteSpace(relative)) continue;
                    var physical = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relative.Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(physical))
                    {
                        System.IO.File.Delete(physical);
                    }
                }
                catch
                {
                    // ignore file delete errors
                }
            }
        }

        _db.Properties.Remove(entity);
        await _db.SaveChangesAsync();

        // if AJAX call, return JSON success, otherwise redirect back
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Json(new { success = true });

        return RedirectToAction("AdminListings", "Home");
    }

     [HttpGet]
    public IActionResult Get(int id)
    {
        var prop = _db.Properties
            .Include(p => p.PropertyImages)
            .FirstOrDefault(p => p.Id == id);

        if (prop == null) return NotFound();

        return Json(new
        {
            prop.Id,
            prop.Title,
            prop.Description,
            prop.PropertyType,
            prop.PricePerMonth,
            prop.Bedrooms,
            prop.Bathrooms,
            prop.SquareFeet,
            AvailableDate = prop.AvailableDate.ToString("yyyy-MM-dd"),
            prop.StreetAddress,
            prop.City,
            prop.State,
            prop.ZipCode,
            prop.Country,
            Images = prop.PropertyImages?.Select(pi => pi.ImageUrl).ToArray() ?? Array.Empty<string>()
        });
    }
[HttpPost]
    [ValidateAntiForgeryToken]
    public  async Task<IActionResult> Edit(Property model, IFormFile[] PropertyImages)
    {
        if (model == null) return BadRequest();

        var entity = await _db.Properties
            .Include(p => p.PropertyImages)
            .FirstOrDefaultAsync(p => p.Id == model.Id);

        if (entity == null) return NotFound();

        // update scalar properties
        entity.Title = model.Title;
        entity.Description = model.Description;
        entity.PropertyType = model.PropertyType;
        entity.PricePerMonth = model.PricePerMonth;
        entity.Bedrooms = model.Bedrooms;
        entity.Bathrooms = model.Bathrooms;
        entity.SquareFeet = model.SquareFeet;
        entity.AvailableDate = model.AvailableDate;
        entity.StreetAddress = model.StreetAddress;
        entity.City = model.City;
        entity.State = model.State;
        entity.ZipCode = model.ZipCode;
        entity.Country = model.Country;

        // ensure images directory exists
        var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
        if (!Directory.Exists(imagesDirectory))
            Directory.CreateDirectory(imagesDirectory);

        // save new uploaded images and add to entity
        if (PropertyImages != null && PropertyImages.Length > 0)
        {
                            entity.PropertyImages = new List<PropertyImage>();

            foreach (var image in PropertyImages)
            {
                if (image == null || image.Length == 0) continue;
                var safeName = Path.GetFileName(image.FileName);
                var filePath = Path.Combine(imagesDirectory, safeName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                entity.PropertyImages.Add(new PropertyImage { ImageUrl = $"/images/{safeName}" });
            }
        }

        await _db.SaveChangesAsync();
        return RedirectToAction("AdminListings", "Home");
    }
}