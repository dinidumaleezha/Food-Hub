namespace FoodHub.Models;

public class RiderBikeAssignment
{
    public int RiderId { get; set; }
    public string BikeRegNo { get; set; } = string.Empty;
    public DateTime AssignmentDate { get; set; }
    public int StartMeter { get; set; }
    public int? EndMeter { get; set; }
}
