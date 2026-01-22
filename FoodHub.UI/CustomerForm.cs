using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using FoodHub.Data;
using FoodHub.Models;
using FoodHub.Services;

namespace FoodHub.UI;

public class CustomerForm : Form
{
    private readonly CustomerService _customerService;
    private readonly DataGridView _grid;
    private readonly TextBox _nameTextBox;
    private readonly TextBox _nicTextBox;
    private readonly DateTimePicker _dobPicker;
    private readonly TextBox _contactTextBox;
    private readonly TextBox _locationTextBox;
    private readonly TextBox _laneTextBox;
    private readonly TextBox _streetTextBox;
    private readonly TextBox _cityTextBox;
    private int _selectedCustomerId;

    public CustomerForm()
    {
        _customerService = new CustomerService(new CustomerRepository());

        Text = "Customer Registration";
        Width = 1000;
        Height = 700;
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

        Controls.Add(UiHelpers.BuildNavPanel(this));

        var formPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 360,
            Padding = new Padding(20)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = 9,
            AutoSize = true
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

        _nameTextBox = CreateTextBox();
        _nicTextBox = CreateTextBox();
        _dobPicker = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 200 };
        _contactTextBox = CreateTextBox();
        _locationTextBox = CreateTextBox();
        _laneTextBox = CreateTextBox();
        _streetTextBox = CreateTextBox();
        _cityTextBox = CreateTextBox();

        AddRow(layout, 0, "Name", _nameTextBox);
        AddRow(layout, 1, "NIC", _nicTextBox);
        AddRow(layout, 2, "DOB", _dobPicker);
        AddRow(layout, 3, "Contact No", _contactTextBox);
        AddRow(layout, 4, "Location No", _locationTextBox);
        AddRow(layout, 5, "Lane", _laneTextBox);
        AddRow(layout, 6, "Street", _streetTextBox);
        AddRow(layout, 7, "City", _cityTextBox);

        var buttonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 15, 0, 0)
        };

        var saveButton = new Button { Text = "Save", Width = 80 };
        var updateButton = new Button { Text = "Update", Width = 80 };
        var deleteButton = new Button { Text = "Delete", Width = 80 };
        var resetButton = new Button { Text = "Reset", Width = 80 };

        saveButton.Click += (_, _) => SaveCustomer();
        updateButton.Click += (_, _) => UpdateCustomer();
        deleteButton.Click += (_, _) => DeleteCustomer();
        resetButton.Click += (_, _) => ResetForm();

        buttonsPanel.Controls.Add(saveButton);
        buttonsPanel.Controls.Add(updateButton);
        buttonsPanel.Controls.Add(deleteButton);
        buttonsPanel.Controls.Add(resetButton);

        formPanel.Controls.Add(layout);
        formPanel.Controls.Add(buttonsPanel);

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "CustomerId", Width = 60 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Width = 160 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "NIC", DataPropertyName = "Nic", Width = 120 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Contact", DataPropertyName = "ContactNo", Width = 120 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "City", DataPropertyName = "City", Width = 120 });
        _grid.CellClick += GridOnCellClick;

        var gridPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };
        gridPanel.Controls.Add(_grid);

        Controls.Add(gridPanel);
        Controls.Add(formPanel);

        Load += (_, _) => LoadCustomers();
    }

    private static TextBox CreateTextBox() => new TextBox { Width = 200 };

    private static void AddRow(TableLayoutPanel layout, int row, string labelText, Control control)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var label = new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left };
        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(control, 1, row);
    }

    private void LoadCustomers()
    {
        _grid.DataSource = _customerService.GetAllCustomers();
    }

    private void SaveCustomer()
    {
        try
        {
            var customer = GetCustomerFromForm();
            _customerService.AddCustomer(customer);
            MessageBox.Show("Customer saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ResetForm();
            LoadCustomers();
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            MessageBox.Show("NIC already exists. Please use a unique NIC.", "Duplicate NIC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdateCustomer()
    {
        try
        {
            var customer = GetCustomerFromForm();
            customer.CustomerId = _selectedCustomerId;
            _customerService.UpdateCustomer(customer);
            MessageBox.Show("Customer updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ResetForm();
            LoadCustomers();
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            MessageBox.Show("NIC already exists. Please use a unique NIC.", "Duplicate NIC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DeleteCustomer()
    {
        try
        {
            _customerService.DeleteCustomer(_selectedCustomerId);
            MessageBox.Show("Customer deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ResetForm();
            LoadCustomers();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Customer GetCustomerFromForm()
    {
        return new Customer
        {
            Name = _nameTextBox.Text.Trim(),
            Nic = _nicTextBox.Text.Trim(),
            DateOfBirth = _dobPicker.Value.Date,
            ContactNo = _contactTextBox.Text.Trim(),
            LocationNo = _locationTextBox.Text.Trim(),
            Lane = _laneTextBox.Text.Trim(),
            Street = _streetTextBox.Text.Trim(),
            City = _cityTextBox.Text.Trim()
        };
    }

    private void ResetForm()
    {
        _selectedCustomerId = 0;
        _nameTextBox.Text = string.Empty;
        _nicTextBox.Text = string.Empty;
        _dobPicker.Value = DateTime.Today;
        _contactTextBox.Text = string.Empty;
        _locationTextBox.Text = string.Empty;
        _laneTextBox.Text = string.Empty;
        _streetTextBox.Text = string.Empty;
        _cityTextBox.Text = string.Empty;
        _grid.ClearSelection();
    }

    private void GridOnCellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        if (_grid.Rows[e.RowIndex].DataBoundItem is Customer customer)
        {
            _selectedCustomerId = customer.CustomerId;
            _nameTextBox.Text = customer.Name;
            _nicTextBox.Text = customer.Nic;
            _dobPicker.Value = customer.DateOfBirth;
            _contactTextBox.Text = customer.ContactNo;
            _locationTextBox.Text = customer.LocationNo;
            _laneTextBox.Text = customer.Lane;
            _streetTextBox.Text = customer.Street;
            _cityTextBox.Text = customer.City;
        }
    }
}
