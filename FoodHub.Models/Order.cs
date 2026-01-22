namespace FoodHub.Models;

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int? RiderId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime OrderTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime? DispatchTime { get; set; }
    public decimal OrderAmount { get; set; }
}
