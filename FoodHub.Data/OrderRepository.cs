using System.Data.SqlClient;
using FoodHub.Models;

namespace FoodHub.Data;

public class OrderRepository
{
    public int CreateOrderWithItems(Order order, List<OrderItemLine> items)
    {
        const string orderSql = """
            INSERT INTO Orders (CustomerID, RiderID, OrderDate, OrderTime, Status, PaymentMethod, DispatchTime, OrderAmount)
            VALUES (@CustomerID, @RiderID, @OrderDate, @OrderTime, @Status, @PaymentMethod, @DispatchTime, @OrderAmount);
            SELECT CAST(SCOPE_IDENTITY() AS int);
            """;

        const string itemSql = """
            INSERT INTO OrderItem (OrderID, ItemID, Quantity)
            VALUES (@OrderID, @ItemID, @Quantity);
            """;

        using var connection = DbHelper.GetConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            using var orderCommand = new SqlCommand(orderSql, connection, transaction);
            orderCommand.Parameters.AddWithValue("@CustomerID", order.CustomerId);
            orderCommand.Parameters.AddWithValue("@RiderID", (object?)order.RiderId ?? DBNull.Value);
            orderCommand.Parameters.AddWithValue("@OrderDate", order.OrderDate);
            orderCommand.Parameters.AddWithValue("@OrderTime", order.OrderTime);
            orderCommand.Parameters.AddWithValue("@Status", order.Status);
            orderCommand.Parameters.AddWithValue("@PaymentMethod", order.PaymentMethod);
            orderCommand.Parameters.AddWithValue("@DispatchTime", (object?)order.DispatchTime ?? DBNull.Value);
            orderCommand.Parameters.AddWithValue("@OrderAmount", order.OrderAmount);

            var orderId = (int)orderCommand.ExecuteScalar();

            foreach (var item in items)
            {
                using var itemCommand = new SqlCommand(itemSql, connection, transaction);
                itemCommand.Parameters.AddWithValue("@OrderID", orderId);
                itemCommand.Parameters.AddWithValue("@ItemID", item.FoodItemId);
                itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                itemCommand.ExecuteNonQuery();
            }

            transaction.Commit();
            return orderId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public List<int> GetOrdersForAssignment()
    {
        const string sql = """
            SELECT OrderID
            FROM Orders
            WHERE RiderID IS NULL OR Status = 'Pending'
            ORDER BY OrderDate DESC, OrderTime DESC;
            """;

        var orders = new List<int>();
        using var connection = DbHelper.GetConnection();
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            orders.Add(reader.GetInt32(0));
        }

        return orders;
    }

    public List<OrderStatusResult> SearchOrders(string query)
    {
        const string sql = """
            SELECT o.OrderID,
                   c.Name AS CustomerName,
                   COALESCE(r.FirstName + ' ' + r.LastName, '') AS RiderName,
                   o.OrderDate,
                   o.OrderAmount,
                   o.Status,
                   o.PaymentMethod,
                   o.DispatchTime
            FROM Orders o
            INNER JOIN Customer c ON o.CustomerID = c.CustomerID
            LEFT JOIN Rider r ON o.RiderID = r.RiderID
            WHERE (@OrderId IS NOT NULL AND o.OrderID = @OrderId)
               OR (@OrderId IS NULL AND c.Name LIKE @CustomerName)
            ORDER BY o.OrderDate DESC, o.OrderTime DESC;
            """;

        int? orderId = null;
        if (int.TryParse(query, out var parsed))
        {
            orderId = parsed;
        }

        var results = new List<OrderStatusResult>();
        using var connection = DbHelper.GetConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@OrderId", (object?)orderId ?? DBNull.Value);
        command.Parameters.AddWithValue("@CustomerName", $"%{query}%");
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(new OrderStatusResult
            {
                OrderId = reader.GetInt32(0),
                CustomerName = reader.GetString(1),
                RiderName = reader.GetString(2),
                OrderDate = reader.GetDateTime(3),
                OrderAmount = reader.GetDecimal(4),
                Status = reader.GetString(5),
                PaymentMethod = reader.GetString(6),
                DispatchTime = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            });
        }

        return results;
    }

    public List<OrderItemDetail> GetOrderItems(int orderId)
    {
        const string sql = """
            SELECT f.ItemName,
                   f.Price,
                   oi.Quantity
            FROM OrderItem oi
            INNER JOIN FoodItem f ON oi.ItemID = f.FoodItemID
            WHERE oi.OrderID = @OrderID;
            """;

        var items = new List<OrderItemDetail>();
        using var connection = DbHelper.GetConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@OrderID", orderId);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new OrderItemDetail
            {
                ItemName = reader.GetString(0),
                Price = reader.GetDecimal(1),
                Quantity = reader.GetInt32(2)
            });
        }

        return items;
    }

    public void UpdateOrderAssignment(int orderId, int riderId)
    {
        const string sql = """
            UPDATE Orders
            SET RiderID = @RiderID,
                Status = @Status,
                DispatchTime = @DispatchTime
            WHERE OrderID = @OrderID;
            """;

        using var connection = DbHelper.GetConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@OrderID", orderId);
        command.Parameters.AddWithValue("@RiderID", riderId);
        command.Parameters.AddWithValue("@Status", "Dispatched");
        command.Parameters.AddWithValue("@DispatchTime", DateTime.Now);
        connection.Open();
        command.ExecuteNonQuery();
    }
}
