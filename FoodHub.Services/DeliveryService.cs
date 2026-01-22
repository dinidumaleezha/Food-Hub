using FoodHub.Data;
using FoodHub.Models;

namespace FoodHub.Services;

public class DeliveryService
{
    private readonly RiderBikeAssignmentRepository _assignmentRepository;
    private readonly OrderRepository _orderRepository;

    public DeliveryService(RiderBikeAssignmentRepository assignmentRepository, OrderRepository orderRepository)
    {
        _assignmentRepository = assignmentRepository;
        _orderRepository = orderRepository;
    }

    public void AssignRider(int orderId, RiderBikeAssignment assignment)
    {
        if (orderId <= 0)
        {
            throw new ArgumentException("Select an order to assign.");
        }

        if (assignment.RiderId <= 0)
        {
            throw new ArgumentException("Select a rider.");
        }

        if (string.IsNullOrWhiteSpace(assignment.BikeRegNo))
        {
            throw new ArgumentException("Select a motorbike.");
        }

        if (assignment.StartMeter < 0)
        {
            throw new ArgumentException("Start meter must be zero or greater.");
        }

        if (assignment.EndMeter.HasValue && assignment.EndMeter.Value < assignment.StartMeter)
        {
            throw new ArgumentException("End meter must be greater than or equal to start meter.");
        }

        _assignmentRepository.Insert(assignment);
        _orderRepository.UpdateOrderAssignment(orderId, assignment.RiderId);
    }
}
