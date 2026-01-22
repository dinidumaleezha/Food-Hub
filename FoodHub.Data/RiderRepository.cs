using System.Data.SqlClient;
using FoodHub.Models;

namespace FoodHub.Data;

public class RiderRepository
{
    public List<Rider> GetAll()
    {
        const string sql = """
            SELECT RiderID, FirstName, LastName
            FROM Rider
            ORDER BY FirstName, LastName;
            """;

        var riders = new List<Rider>();
        using var connection = DbHelper.GetConnection();
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            riders.Add(new Rider
            {
                RiderId = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2)
            });
        }

        return riders;
    }
}
