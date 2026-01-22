using FoodHub.Data;
using FoodHub.Models;

namespace FoodHub.Services;

public class OrderService
{
    private readonly OrderRepository _orderRepository;

    public OrderService(OrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public int PlaceOrder(int customerId, string paymentMethod, List<OrderItemLine> items)
    {
        if (customerId <= 0)
        {
            throw new ArgumentException("Select a customer.");
        }

        if (string.IsNullOrWhiteSpace(paymentMethod))
        {
            throw new ArgumentException("Select a payment method.");
        }

        if (items.Count == 0)
        {
            throw new ArgumentException("Add at least one item to the order.");
        }

        foreach (var item in items)
        {
            if (item.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }
        }

        var order = new Order
        {
            CustomerId = customerId,
            RiderId = null,
            OrderDate = DateTime.Today,
            OrderTime = DateTime.Now,
            Status = "Pending",
            PaymentMethod = paymentMethod,
            DispatchTime = null,
            OrderAmount = items.Sum(i => i.LineTotal)
        };

        return _orderRepository.CreateOrderWithItems(order, items);
    }

    public List<int> GetOrdersForAssignment() => _orderRepository.GetOrdersForAssignment();

    public List<OrderStatusResult> SearchOrders(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<OrderStatusResult>();
        }

        return _orderRepository.SearchOrders(query.Trim());
    }

    public List<OrderItemDetail> GetOrderItems(int orderId)
    {
        if (orderId <= 0)
        {
            return new List<OrderItemDetail>();
        }

        return _orderRepository.GetOrderItems(orderId);
    }
}
