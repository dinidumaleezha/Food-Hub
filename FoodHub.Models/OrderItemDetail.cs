namespace FoodHub.Models;

public class OrderItemDetail
{
    public string ItemName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => Price * Quantity;
}
