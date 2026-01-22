using System.Data.SqlClient;
using FoodHub.Models;

namespace FoodHub.Data;

public class RiderBikeAssignmentRepository
{
    public void Insert(RiderBikeAssignment assignment)
    {
        const string sql = """
            INSERT INTO RiderBikeAssignment (RiderID, BikeRegNo, AssignmentDate, StartMeter, EndMeter)
            VALUES (@RiderID, @BikeRegNo, @AssignmentDate, @StartMeter, @EndMeter);
            """;

        using var connection = DbHelper.GetConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@RiderID", assignment.RiderId);
        command.Parameters.AddWithValue("@BikeRegNo", assignment.BikeRegNo);
        command.Parameters.AddWithValue("@AssignmentDate", assignment.AssignmentDate);
        command.Parameters.AddWithValue("@StartMeter", assignment.StartMeter);
        command.Parameters.AddWithValue("@EndMeter", (object?)assignment.EndMeter ?? DBNull.Value);
        connection.Open();
        command.ExecuteNonQuery();
    }
}
