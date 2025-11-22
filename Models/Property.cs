using Microsoft.AspNetCore.Identity;

namespace MyMvcAuthProject.Models;

public class Property
{
    public int Id { get; set; }

    // Basic Information
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;

    // Pricing & Size
    public decimal PricePerMonth { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int SquareFeet { get; set; }

    // Availability
    public DateTime AvailableDate { get; set; }

    // Address
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    // MANY Amenities
    public List<Amenity> Amenities { get; set; } = new List<Amenity>();
    // Images (One-to-many)
    public List<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();

    // Owner (Identity)
    public string UserId { get; set; } = string.Empty;
    public  ApplicationUser User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
