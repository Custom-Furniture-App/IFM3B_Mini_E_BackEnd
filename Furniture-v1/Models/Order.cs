namespace Furniture_v1.Models
{
  public class Order
  {
    public int Id { get; set; } 
    public string OrderNumber { get; set; } = null!;

    public int CustomerId { get; set; }
    public User Customer { get; set; } = null!;

    public string Status { get; set; } = "pending"; 
    public decimal TotalAmount { get; set; }

    public ICollection<OrderItem>? Items { get; set; }
  }

}
