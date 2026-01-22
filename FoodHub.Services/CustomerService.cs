using System.Data.SqlClient;
using FoodHub.Data;
using FoodHub.Models;

namespace FoodHub.Services;

public class CustomerService
{
    private readonly CustomerRepository _repository;

    public CustomerService(CustomerRepository repository)
    {
        _repository = repository;
    }

    public List<Customer> GetAllCustomers() => _repository.GetAll();

    public void AddCustomer(Customer customer)
    {
        ValidateCustomer(customer);
        _repository.Insert(customer);
    }

    public void UpdateCustomer(Customer customer)
    {
        if (customer.CustomerId <= 0)
        {
            throw new ArgumentException("Select a customer to update.");
        }

        ValidateCustomer(customer);
        _repository.Update(customer);
    }

    public void DeleteCustomer(int customerId)
    {
        if (customerId <= 0)
        {
            throw new ArgumentException("Select a customer to delete.");
        }

        _repository.Delete(customerId);
    }

    private static void ValidateCustomer(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Name))
        {
            throw new ArgumentException("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(customer.Nic))
        {
            throw new ArgumentException("NIC is required.");
        }

        if (customer.DateOfBirth == default)
        {
            throw new ArgumentException("Date of birth is required.");
        }

        if (string.IsNullOrWhiteSpace(customer.ContactNo))
        {
            throw new ArgumentException("Contact number is required.");
        }

        if (string.IsNullOrWhiteSpace(customer.LocationNo))
        {
            throw new ArgumentException("Location number is required.");
        }

        if (string.IsNullOrWhiteSpace(customer.Lane) || string.IsNullOrWhiteSpace(customer.Street) || string.IsNullOrWhiteSpace(customer.City))
        {
            throw new ArgumentException("Lane, Street, and City are required.");
        }
    }
}
