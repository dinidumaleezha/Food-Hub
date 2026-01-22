namespace FoodHub.Models;

public class FoodItem
{
    public int FoodItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
