namespace FoodHub.Models;

public class OrderItemLine
{
    public int FoodItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => Price * Quantity;
}
