using Microsoft.EntityFrameworkCore;
using MyMvcAuthProject.Data;
using MyMvcAuthProject.Models;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// --- Start of change ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("IdentityDb")); // You can name the in-memory database anything
// --- End of change ---

// 3. Add Razor Pages support (Identity UI is built on Razor Pages)
builder.Services.AddRazorPages();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//     .AddEntityFrameworkStores<ApplicationDbContext>();
    builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.ConfigureApplicationCookie(options =>
{
    // change these to your custom actions
    options.LoginPath = "/Account/Login";           // redirect here when not authenticated
    options.AccessDeniedPath = "/Account/AccessDenied"; // redirect here when access denied
    // optional:
    // options.ReturnUrlParameter = "returnUrl";
    // options.Cookie.Name = "MyMvcAuthProject.Auth";
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    for (int i = 1; i <= 10; i++)
    {
        var property = new Property
        {
            Title = $"Property {i}",
            Description = $"Description for Property {i}",
            PropertyType = "Apartment",
            PricePerMonth = 1000 + i * 10,
            Bedrooms = 2,
            Bathrooms = 1,
            SquareFeet = 800 + i * 5,
            AvailableDate = DateTime.Now.AddDays(i),
            StreetAddress = $"{i} Main St",
            City = "Sample City",
            State = "State",
            ZipCode = "12345",
            Country = "Country",
            UserId = "sample-user-id" // Replace with actual user ID if needed
        };

        // Add some amenities
        property.Amenities.Add(new Amenity { Name = "WiFi", Property = property });
        property.Amenities.Add(new Amenity { Name = "Air Conditioning", Property = property });

        // Add some images
        if(i % 2 == 0)
        {
        property.PropertyImages.Add(new PropertyImage { ImageUrl = $"https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=800&q=80", Property = property });
                property.PropertyImages.Add(new PropertyImage { ImageUrl = $"https://images.unsplash.com/photo-1575517111839-3a3843ee7f5d?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=800&q=80", Property = property });
     
          property.PropertyImages.Add(new PropertyImage { ImageUrl = $"https://images.unsplash.com/photo-1600585154340-be6161a56a0c?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=800&q=80", Property = property });
              property.PropertyImages.Add(new PropertyImage { ImageUrl = $"https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=800&q=80", Property = property });

        }else{
               property.PropertyImages.Add(new PropertyImage { ImageUrl = $"https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=800&q=80", Property = property });
                property.PropertyImages.Add(new PropertyImage { ImageUrl = $"https://images.unsplash.com/photo-1575517111839-3a3843ee7f5d?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=800&q=80", Property = property });
                 property.PropertyImages.Add(new PropertyImage { ImageUrl = $"https://images.unsplash.com/photo-1600585154340-be6161a56a0c?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=800&q=80", Property = property });
                 property.PropertyImages.Add(new PropertyImage { ImageUrl = $"https://images.unsplash.com/photo-1600585154340-be6161a56a0c?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=800&q=80", Property = property });

        }

        property.CreatedAt = DateTime.UtcNow;
     
        db.Properties.Add(property);
    }
    
     db.SaveChanges();

     var propertiesWithImages = db.Properties
        .Include(p => p.PropertyImages)
        .ToList();
        foreach (var prop in propertiesWithImages)
        {
            var favorite = new Favorite
            {
                PropertyId = prop.Id,
                UserId = "user "+prop.Id 
            };
            db.Favorites.Add(favorite);
        }
        db.SaveChanges();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
