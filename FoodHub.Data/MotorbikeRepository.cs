using System.Data.SqlClient;
using FoodHub.Models;

namespace FoodHub.Data;

public class MotorbikeRepository
{
    public List<Motorbike> GetAll()
    {
        const string sql = """
            SELECT BikeRegNo, Brand, Model
            FROM Motorbike
            ORDER BY BikeRegNo;
            """;

        var bikes = new List<Motorbike>();
        using var connection = DbHelper.GetConnection();
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            bikes.Add(new Motorbike
            {
                BikeRegNo = reader.GetString(0),
                Brand = reader.GetString(1),
                Model = reader.GetString(2)
            });
        }

        return bikes;
    }
}
