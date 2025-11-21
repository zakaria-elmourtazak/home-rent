namespace MyMvcAuthProject.Models;

public class Amenity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Relationship
    public int PropertyId { get; set; }
    public Property Property { get; set; }
}