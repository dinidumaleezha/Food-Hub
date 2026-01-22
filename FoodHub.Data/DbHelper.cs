using System.Configuration;
using System.Data.SqlClient;

namespace FoodHub.Data;

public static class DbHelper
{
    public static SqlConnection GetConnection()
    {
        var connectionString = ConfigurationManager.AppSettings["FoodHubDb"];
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("FoodHubDb connection string is missing from app.config.");
        }

        return new SqlConnection(connectionString);
    }
}
