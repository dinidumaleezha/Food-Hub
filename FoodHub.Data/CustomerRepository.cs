using System.Data;
using System.Data.SqlClient;
using FoodHub.Models;

namespace FoodHub.Data;

public class CustomerRepository
{
    public int Insert(Customer customer)
    {
        const string sql = """
            INSERT INTO Customer (Name, NIC, DOB, ContactNo, LocationNo, Lane, Street, City)
            VALUES (@Name, @NIC, @DOB, @ContactNo, @LocationNo, @Lane, @Street, @City);
            SELECT CAST(SCOPE_IDENTITY() AS int);
            """;

        using var connection = DbHelper.GetConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", customer.Name);
        command.Parameters.AddWithValue("@NIC", customer.Nic);
        command.Parameters.AddWithValue("@DOB", customer.DateOfBirth);
        command.Parameters.AddWithValue("@ContactNo", customer.ContactNo);
        command.Parameters.AddWithValue("@LocationNo", customer.LocationNo);
        command.Parameters.AddWithValue("@Lane", customer.Lane);
        command.Parameters.AddWithValue("@Street", customer.Street);
        command.Parameters.AddWithValue("@City", customer.City);

        connection.Open();
        return (int)command.ExecuteScalar();
    }

    public void Update(Customer customer)
    {
        const string sql = """
            UPDATE Customer
            SET Name = @Name,
                NIC = @NIC,
                DOB = @DOB,
                ContactNo = @ContactNo,
                LocationNo = @LocationNo,
                Lane = @Lane,
                Street = @Street,
                City = @City
            WHERE CustomerID = @CustomerID;
            """;

        using var connection = DbHelper.GetConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@CustomerID", customer.CustomerId);
        command.Parameters.AddWithValue("@Name", customer.Name);
        command.Parameters.AddWithValue("@NIC", customer.Nic);
        command.Parameters.AddWithValue("@DOB", customer.DateOfBirth);
        command.Parameters.AddWithValue("@ContactNo", customer.ContactNo);
        command.Parameters.AddWithValue("@LocationNo", customer.LocationNo);
        command.Parameters.AddWithValue("@Lane", customer.Lane);
        command.Parameters.AddWithValue("@Street", customer.Street);
        command.Parameters.AddWithValue("@City", customer.City);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Delete(int customerId)
    {
        const string sql = "DELETE FROM Customer WHERE CustomerID = @CustomerID;";

        using var connection = DbHelper.GetConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@CustomerID", customerId);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public List<Customer> GetAll()
    {
        const string sql = """
            SELECT CustomerID, Name, NIC, DOB, ContactNo, LocationNo, Lane, Street, City
            FROM Customer
            ORDER BY CustomerID DESC;
            """;

        var customers = new List<Customer>();
        using var connection = DbHelper.GetConnection();
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            customers.Add(new Customer
            {
                CustomerId = reader.GetInt32(0),
                Name = reader.GetString(1),
                Nic = reader.GetString(2),
                DateOfBirth = reader.GetDateTime(3),
                ContactNo = reader.GetString(4),
                LocationNo = reader.GetString(5),
                Lane = reader.GetString(6),
                Street = reader.GetString(7),
                City = reader.GetString(8)
            });
        }

        return customers;
    }
}
