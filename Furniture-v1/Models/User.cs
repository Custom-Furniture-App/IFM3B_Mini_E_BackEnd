namespace Furniture_v1.Models
{
  public class User
  {
    public int Id { get; set; } 
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = null!; 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }

}
