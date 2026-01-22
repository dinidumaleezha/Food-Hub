namespace FoodHub.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Nic { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string ContactNo { get; set; } = string.Empty;
    public string LocationNo { get; set; } = string.Empty;
    public string Lane { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
