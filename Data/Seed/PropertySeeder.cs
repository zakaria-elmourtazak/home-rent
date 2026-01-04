using System.Text.Json;
using MyMvcAuthProject.Data;
using MyMvcAuthProject.Models;

public static class PropertySeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, IWebHostEnvironment env)
    {
        if (context.Properties.Any())
            return;

        var user = new ApplicationUser
        {
            Id = "user1-id",
            UserName = "user1",
            Email = "user1@example.com"
        };
        context.Users.Add(user);
        context.Users.Add(new ApplicationUser
        {
            Id = "user2-id",
            UserName = "user2",
            Email = "user2@example.com"
        });

        var filePath = Path.Combine(env.ContentRootPath, "Data", "Seed", "properties.morocco.json");

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Seed file not found.", filePath);

        var json = await File.ReadAllTextAsync(filePath);

        var properties = JsonSerializer.Deserialize<List<Property>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (properties is null || properties.Count == 0)
            return;

        foreach (var property in properties)
        {
            context.Properties.Add(new Property
            {
                Title = property.Title,
                Description = property.Description,
                PropertyType = property.PropertyType,
                PricePerMonth = property.PricePerMonth,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                SquareFeet = property.SquareFeet,
                AvailableDate = property.AvailableDate,
                StreetAddress = property.StreetAddress,
                City = property.City,
                State = property.State,
                ZipCode = property.ZipCode,
                Country = property.Country,
                UserId = user.Id,
                User = user,
                CreatedAt = DateTime.UtcNow,
                Amenities = property.Amenities,
                PropertyImages = new List<PropertyImage>([
                      new PropertyImage { ImageUrl = property.ImageUrl[0], Property = property },
                     new PropertyImage { ImageUrl = property.ImageUrl[1], Property = property },
                     new PropertyImage { ImageUrl = property.ImageUrl[2], Property = property },
                     new PropertyImage { ImageUrl = property.ImageUrl[3], Property = property }
                    //  new PropertyImage { ImageUrl = property.ImageUrl[4], Property = property }
                ])
            });
        }

        // context.Properties.AddRange(properties);
        await context.SaveChangesAsync();
    }
}