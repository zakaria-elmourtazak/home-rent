namespace MyMvcAuthProject.Models;

public class PropertyDetailsViewModel
{
    public Property Property { get; set; }
    public bool IsFavorite { get; set; }

    public double? MapLatitude { get; set; }
    public double? MapLongitude { get; set; }
    public List<Property> similarProperties { get; set; }
}