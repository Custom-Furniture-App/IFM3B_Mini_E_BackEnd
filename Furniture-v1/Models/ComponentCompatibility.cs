namespace Furniture_v1.Models
{
  public class ComponentCompatibility
  {
    public int Id { get; set; }

    public int ComponentId { get; set; }
    public Component Component { get; set; } = null!;

    public int CompatibleWithId { get; set; }
    public Component CompatibleWith { get; set; } = null!;
  }

}
