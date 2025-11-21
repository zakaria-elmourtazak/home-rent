namespace MyMvcAuthProject.Models;

public class ListingProperty
{
    public List<Property> Properties { get; set; } = new List<Property>();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
