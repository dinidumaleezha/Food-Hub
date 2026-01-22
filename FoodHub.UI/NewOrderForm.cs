using System.Drawing;
using System.Windows.Forms;
using FoodHub.Data;
using FoodHub.Models;
using FoodHub.Services;

namespace FoodHub.UI;

public class NewOrderForm : Form
{
    private readonly CustomerRepository _customerRepository;
    private readonly FoodItemRepository _foodItemRepository;
    private readonly OrderService _orderService;
    private readonly ComboBox _customerComboBox;
    private readonly DataGridView _foodGrid;
    private readonly DataGridView _cartGrid;
    private readonly TextBox _quantityTextBox;
    private readonly Label _totalLabel;
    private readonly RadioButton _cashRadio;
    private readonly RadioButton _cardRadio;
    private readonly RadioButton _onlineRadio;
    private readonly BindingSource _cartSource;
    private readonly List<OrderItemLine> _cartItems;

    public NewOrderForm()
    {
        _customerRepository = new CustomerRepository();
        _foodItemRepository = new FoodItemRepository();
        _orderService = new OrderService(new OrderRepository());
        _cartItems = new List<OrderItemLine>();
        _cartSource = new BindingSource();

        Text = "New Order";
        Width = 1100;
        Height = 750;
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

        Controls.Add(UiHelpers.BuildNavPanel(this));

        var headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            Padding = new Padding(15, 10, 15, 10)
        };

        var customerLabel = new Label { Text = "Customer", AutoSize = true };
        _customerComboBox = new ComboBox { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
        headerPanel.Controls.Add(customerLabel);
        customerLabel.Location = new Point(10, 15);
        _customerComboBox.Location = new Point(90, 12);
        headerPanel.Controls.Add(_customerComboBox);

        Controls.Add(headerPanel);

        var contentPanel = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 500,
            Padding = new Padding(10)
        };

        _foodGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        _foodGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Item", DataPropertyName = "ItemName", Width = 200 });
        _foodGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Price", DataPropertyName = "Price", Width = 80 });

        var leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(5) };
        var leftHeader = new Label { Text = "Food Items", Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 11F, FontStyle.Bold) };
        leftPanel.Controls.Add(_foodGrid);
        leftPanel.Controls.Add(leftHeader);

        var addPanel = new Panel { Dock = DockStyle.Bottom, Height = 60 };
        var qtyLabel = new Label { Text = "Qty", AutoSize = true, Location = new Point(10, 20) };
        _quantityTextBox = new TextBox { Width = 60, Location = new Point(50, 16) };
        var addButton = new Button { Text = "Add Item", Location = new Point(130, 14), Width = 100 };
        addButton.Click += (_, _) => AddSelectedItem();
        addPanel.Controls.Add(qtyLabel);
        addPanel.Controls.Add(_quantityTextBox);
        addPanel.Controls.Add(addButton);
        leftPanel.Controls.Add(addPanel);

        _cartGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Item", DataPropertyName = "ItemName", Width = 200 });
        _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Price", DataPropertyName = "Price", Width = 80 });
        _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Qty", DataPropertyName = "Quantity", Width = 50 });
        _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = "LineTotal", Width = 80 });

        _cartSource.DataSource = _cartItems;
        _cartGrid.DataSource = _cartSource;

        var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(5) };
        var rightHeader = new Label { Text = "Order Summary", Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 11F, FontStyle.Bold) };
        rightPanel.Controls.Add(_cartGrid);
        rightPanel.Controls.Add(rightHeader);

        var footerPanel = new Panel { Dock = DockStyle.Bottom, Height = 140 };
        _totalLabel = new Label { Text = "Total: 0.00", AutoSize = true, Location = new Point(10, 10), Font = new Font("Segoe UI", 11F, FontStyle.Bold) };
        _cashRadio = new RadioButton { Text = "Cash", Location = new Point(10, 45) };
        _cardRadio = new RadioButton { Text = "Card", Location = new Point(80, 45) };
        _onlineRadio = new RadioButton { Text = "Online", Location = new Point(150, 45) };
        var placeOrderButton = new Button { Text = "Place Order", Width = 120, Location = new Point(10, 80) };
        placeOrderButton.Click += (_, _) => PlaceOrder();
        footerPanel.Controls.Add(_totalLabel);
        footerPanel.Controls.Add(_cashRadio);
        footerPanel.Controls.Add(_cardRadio);
        footerPanel.Controls.Add(_onlineRadio);
        footerPanel.Controls.Add(placeOrderButton);
        rightPanel.Controls.Add(footerPanel);

        contentPanel.Panel1.Controls.Add(leftPanel);
        contentPanel.Panel2.Controls.Add(rightPanel);
        Controls.Add(contentPanel);

        Load += (_, _) => LoadData();
    }

    private void LoadData()
    {
        _customerComboBox.DataSource = _customerRepository.GetAll();
        _customerComboBox.DisplayMember = "Name";
        _customerComboBox.ValueMember = "CustomerId";

        _foodGrid.DataSource = _foodItemRepository.GetAll();
    }

    private void AddSelectedItem()
    {
        if (_foodGrid.CurrentRow?.DataBoundItem is not FoodItem item)
        {
            MessageBox.Show("Select a food item.", "Missing Item", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!int.TryParse(_quantityTextBox.Text.Trim(), out var qty) || qty <= 0)
        {
            MessageBox.Show("Enter a valid quantity.", "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var existing = _cartItems.FirstOrDefault(c => c.FoodItemId == item.FoodItemId);
        if (existing != null)
        {
            existing.Quantity += qty;
        }
        else
        {
            _cartItems.Add(new OrderItemLine
            {
                FoodItemId = item.FoodItemId,
                ItemName = item.ItemName,
                Price = item.Price,
                Quantity = qty
            });
        }

        _cartSource.ResetBindings(false);
        _quantityTextBox.Text = string.Empty;
        UpdateTotal();
    }

    private void UpdateTotal()
    {
        var total = _cartItems.Sum(i => i.LineTotal);
        _totalLabel.Text = $"Total: {total:C}";
    }

    private void PlaceOrder()
    {
        try
        {
            if (_customerComboBox.SelectedValue is not int customerId)
            {
                MessageBox.Show("Select a customer.", "Missing Customer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var paymentMethod = GetPaymentMethod();
            var orderId = _orderService.PlaceOrder(customerId, paymentMethod, _cartItems);
            MessageBox.Show($"Order #{orderId} placed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ResetOrder();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private string GetPaymentMethod()
    {
        if (_cashRadio.Checked)
        {
            return "Cash";
        }

        if (_cardRadio.Checked)
        {
            return "Card";
        }

        if (_onlineRadio.Checked)
        {
            return "Online";
        }

        throw new ArgumentException("Select a payment method.");
    }

    private void ResetOrder()
    {
        _cartItems.Clear();
        _cartSource.ResetBindings(false);
        _cashRadio.Checked = false;
        _cardRadio.Checked = false;
        _onlineRadio.Checked = false;
        _quantityTextBox.Text = string.Empty;
        UpdateTotal();
    }
}
