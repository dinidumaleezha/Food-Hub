namespace FoodHub.Models;

public class OrderStatusResult
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string RiderName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal OrderAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime? DispatchTime { get; set; }
}
