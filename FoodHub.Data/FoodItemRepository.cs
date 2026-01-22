using System.Data.SqlClient;
using FoodHub.Models;

namespace FoodHub.Data;

public class FoodItemRepository
{
    public List<FoodItem> GetAll()
    {
        const string sql = """
            SELECT FoodItemID, ItemName, Price
            FROM FoodItem
            ORDER BY ItemName;
            """;

        var items = new List<FoodItem>();
        using var connection = DbHelper.GetConnection();
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new FoodItem
            {
                FoodItemId = reader.GetInt32(0),
                ItemName = reader.GetString(1),
                Price = reader.GetDecimal(2)
            });
        }

        return items;
    }
}
