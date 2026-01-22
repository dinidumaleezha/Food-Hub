namespace FoodHub.Models;

public class Motorbike
{
    public string BikeRegNo { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;

    public string DisplayName => $"{BikeRegNo} - {Brand} {Model}".Trim();
}
