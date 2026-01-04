using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMvcAuthProject.Data;
using MyMvcAuthProject.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Text.Json;

namespace MyMvcAuthProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _db = db;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var properties = _db.Properties
           .Include(p => p.Amenities)
           .Include(p => p.PropertyImages)
           .ToList();

        return View(properties);
    }
    [HttpPost]
    public IActionResult FilterListings(
        int pageNumber = 1,
        int pageSize = 9,
        string sortBy = "newest",
        Dictionary<string, string> filters = null)
    {
        Console.WriteLine("Filters received:", sortBy);
        if (filters != null)
        {
            TempData["filters"] = JsonConvert.SerializeObject(filters);
        }
        TempData["sortBy"] = sortBy;

        return RedirectToAction("Listings", new
        {
            pageNumber = pageNumber,
            pageSize = pageSize,
            sortBy = sortBy
        });
    }


    public IActionResult Listings(int pageNumber = 1, int pageSize = 9, string sortBy = "newest")
    {
        IQueryable<Property> query = _db.Properties
       .Include(p => p.Amenities)
       .Include(p => p.PropertyImages)
       .AsQueryable();

        Dictionary<string, string> filters = null;

        if (TempData["filters"] != null)
        {
            filters = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                TempData["filters"].ToString()
            );
        }

        // Apply filters
        if (filters != null)
        {
            foreach (var filter in filters)
            {
                var key = filter.Key;
                var value = filter.Value;

                if (string.IsNullOrEmpty(value))
                    continue;

                switch (key)
                {
                    case "Location":
                        query = query.Where(p => p.City.ToLower().Contains(value.ToLower())
                            || p.State.ToLower().Contains(value.ToLower())
                            || p.Country.ToLower().Contains(value.ToLower())
                            || p.StreetAddress.ToLower().Contains(value.ToLower()));
                        break;

                    case "PriceMin":
                        if (int.TryParse(value, out int min))
                            query = query.Where(p => p.PricePerMonth >= min);
                        break;

                    case "PriceMax":
                        if (int.TryParse(value, out int max))
                            query = query.Where(p => p.PricePerMonth <= max);
                        break;

                    case "Bedrooms":
                        if (value != "Any" && int.TryParse(value, out int bdr))
                            query = query.Where(p => p.Bedrooms == bdr);
                        break;

                    case "Bathrooms":
                        if (value != "Any" && int.TryParse(value, out int bth))
                            query = query.Where(p => p.Bathrooms == bth);
                        break;

                    case "PropertyType":
                        query = query.Where(p => p.PropertyType.ToLower() == value.ToLower());
                        break;
                }
            }
        }
        query = sortBy switch
        {
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            "oldest" => query.OrderBy(p => p.CreatedAt),
            "price_low" => query.OrderBy(p => p.PricePerMonth),
            "price_high" => query.OrderByDescending(p => p.PricePerMonth),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };


        int totalCount = query.Count();

        var properties = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var vm = new
        {
            Properties = properties,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            sortBy = sortBy,
        };

        return View(vm);
    }
    public IActionResult About()
    {
        return View();
    }

    [Authorize]
    public IActionResult AdminListings(int pageNumber = 1, int pageSize = 5)
    {
        var user = _userManager.GetUserAsync(User).Result;
        IQueryable<Property> query = _db.Properties
       .Include(p => p.Amenities)
       .Where(p => p.UserId == user.Id)
       .Include(p => p.PropertyImages)
       .AsQueryable();

        int totalCount = query.Count();
        query = query.OrderByDescending(p => p.CreatedAt);
        var properties = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var vm = new ListingProperty
        {
            Properties = properties,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
        return View(vm);
    }


    public IActionResult Contact()
    {
        return View();
    }


    public async Task<IActionResult> PropertyDetail(int id)
    {
        var property = _db.Properties
            .Include(p => p.Amenities)
            .Include(p => p.PropertyImages)
            // .Include(p => p.User)
            .FirstOrDefault(p => p.Id == id);

        if (property == null)
        {
            return NotFound();
        }
        bool isFavorite = false;
        var userId = "";
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            userId = user.Id;
        }

        isFavorite = await _db.Favorites
            .AnyAsync(f => f.UserId == userId && f.PropertyId == id);
        // }
        var vm = new PropertyDetailsViewModel
        {
            Property = property,
            IsFavorite = isFavorite,
            similarProperties = _db.Properties
                .Include(p => p.PropertyImages)
                .Where(p => p.Id != id && p.PropertyType == property.PropertyType)
                .Take(3)
                .ToList()
        };

        // use explicit map location field if present, otherwise address
        // var locationSource = $"{property.StreetAddress}, {property.City}, {property.State}, {property.ZipCode}";
        // var coords = await ResolveLocationAsync(locationSource);
        // if (coords.lat.HasValue && coords.lon.HasValue)
        // {
        //     vm.MapLatitude = coords.lat.Value;
        //     vm.MapLongitude = coords.lon.Value;
        // }

        return View(vm);
    }

    private async Task<(double? lat, double? lon)> ResolveLocationAsync(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            return (null, null);

        // try parse "lat,lon"
        var parts = location.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
        if (parts.Length >= 2
            && double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var lat)
            && double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
        {
            return (lat, lon);
        }

        // fallback: geocode using Nominatim
        try
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.ParseAdd("MyMvcAuthProject/1.0 (contact@example.com)");
            var url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(location)}&limit=1";
            var json = await http.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);
            var arr = doc.RootElement;
            if (arr.GetArrayLength() > 0)
            {
                var first = arr[0];
                if (first.TryGetProperty("lat", out var latEl) && first.TryGetProperty("lon", out var lonEl)
                    && double.TryParse(latEl.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var gLat)
                    && double.TryParse(lonEl.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var gLon))
                {
                    return (gLat, gLon);
                }
            }
        }
        catch
        {
            // ignore failures, return nulls
        }

        return (null, null);
    }

    [Authorize]

    public IActionResult Dashboard()
    {
        var user = _userManager.GetUserAsync(User).Result;
        if (user == null) return Challenge();

        // total properties owned by the user
        var totalProperties = _db.Properties.Count(p => p.UserId == user.Id);
Console.WriteLine("Total Properties: " + totalProperties);
        // total favorites for the user
        var totalFavorites = _db.Favorites.Count(f => f.UserId == user.Id);

        // total messages: try to be resilient if the DbSet name or message schema differs
        int totalMessages = 0;
        var messagesProp = _db.GetType().GetProperty("Messages");
        if (messagesProp != null)
        {
            var messagesEnumerable = messagesProp.GetValue(_db) as IEnumerable<object>;
            if (messagesEnumerable != null)
            {
                var messagesList = messagesEnumerable.ToList();
                var candidateNames = new[] { "RecipientId", "UserId", "ToUserId", "ToId", "ReceiverId" };
                // try common id property names first
                foreach (var name in candidateNames)
                {
                    var pi = messagesList.FirstOrDefault()?.GetType().GetProperty(name);
                    if (pi != null)
                    {
                        totalMessages = messagesList.Count(m =>
                        {
                            var val = pi.GetValue(m);
                            return val != null && val.ToString() == user.Id;
                        });
                        break;
                    }
                }

                // fallback: count items where any string property equals the user id
                if (totalMessages == 0 && messagesList.Count > 0)
                {
                    foreach (var m in messagesList)
                    {
                        var props = m.GetType().GetProperties();
                        foreach (var p in props)
                        {
                            if (p.PropertyType == typeof(string) && p.GetValue(m)?.ToString() == user.Id)
                            {
                                totalMessages++;
                                break;
                            }
                        }
                    }
                }
            }
        }

        // expose counts to the view
        ViewBag.TotalProperties = totalProperties;
        ViewBag.TotalFavorites = totalFavorites;
        ViewBag.TotalMessages = totalMessages;
        return View();
    }
    [Authorize]

    public async Task<IActionResult> Saved()
    {
        var user = await _userManager.GetUserAsync(User);

        var favorites = _db.Favorites
            .Where(f => f.UserId == user.Id)
            .Include(f => f.Property)
            .ThenInclude(p => p.PropertyImages)
            .ToList();

        return View(favorites);

    }
    [Authorize]

    public IActionResult Messages()
    {
        return View();
    }
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }
        var model = new ProfileViewModel
        {
            UserName = user.UserName,
            TimeZone = user.TimeZone,
            Bio = user.Bio,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfileImageUrl = user.ProfileImageUrl
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]

    // [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Property model)
    {

        if (!ModelState.IsValid) return RedirectToAction("AdminListings", "Home");

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        model.UserId = user.Id;
        model.User = null; // avoid tracking nav property

        _db.Properties.Add(model);
        await _db.SaveChangesAsync();

        return RedirectToAction("AdminListings", "Home");
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
