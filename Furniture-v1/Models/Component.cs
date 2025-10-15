namespace Furniture_v1.Models
{
  public class Component
  {
    public int Id { get; set; } 
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!; 
    public decimal UnitPrice { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; } 

    public ICollection<ComponentCompatibility>? CompatibleComponents { get; set; }
  }

}
