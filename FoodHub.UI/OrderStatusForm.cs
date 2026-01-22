using System.Drawing;
using System.Windows.Forms;
using FoodHub.Data;
using FoodHub.Models;
using FoodHub.Services;

namespace FoodHub.UI;

public class OrderStatusForm : Form
{
    private readonly OrderService _orderService;
    private readonly TextBox _searchTextBox;
    private readonly DataGridView _ordersGrid;
    private readonly DataGridView _itemsGrid;

    public OrderStatusForm()
    {
        _orderService = new OrderService(new OrderRepository());

        Text = "Order Status";
        Width = 1100;
        Height = 700;
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

        Controls.Add(UiHelpers.BuildNavPanel(this));

        var searchPanel = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(15, 10, 15, 10) };
        var searchLabel = new Label { Text = "Order ID / Customer", AutoSize = true };
        _searchTextBox = new TextBox { Width = 250, Location = new Point(160, 12) };
        var searchButton = new Button { Text = "Search", Width = 90, Location = new Point(430, 10) };
        searchButton.Click += (_, _) => SearchOrders();
        searchPanel.Controls.Add(searchLabel);
        searchLabel.Location = new Point(10, 15);
        searchPanel.Controls.Add(_searchTextBox);
        searchPanel.Controls.Add(searchButton);

        Controls.Add(searchPanel);

        var splitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            SplitterDistance = 300,
            Padding = new Padding(10)
        };

        _ordersGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Order ID", DataPropertyName = "OrderId", Width = 80 });
        _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Customer", DataPropertyName = "CustomerName", Width = 180 });
        _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Rider", DataPropertyName = "RiderName", Width = 140 });
        _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Order Date", DataPropertyName = "OrderDate", Width = 130 });
        _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Amount", DataPropertyName = "OrderAmount", Width = 100 });
        _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 100 });
        _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Payment", DataPropertyName = "PaymentMethod", Width = 90 });
        _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Dispatch", DataPropertyName = "DispatchTime", Width = 140 });
        _ordersGrid.SelectionChanged += (_, _) => LoadOrderItems();

        _itemsGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Item", DataPropertyName = "ItemName", Width = 200 });
        _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Price", DataPropertyName = "Price", Width = 80 });
        _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Qty", DataPropertyName = "Quantity", Width = 60 });
        _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Line Total", DataPropertyName = "LineTotal", Width = 90 });

        splitContainer.Panel1.Controls.Add(_ordersGrid);
        splitContainer.Panel2.Controls.Add(_itemsGrid);

        Controls.Add(splitContainer);
    }

    private void SearchOrders()
    {
        try
        {
            var results = _orderService.SearchOrders(_searchTextBox.Text.Trim());
            _ordersGrid.DataSource = results;
            _itemsGrid.DataSource = null;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadOrderItems()
    {
        if (_ordersGrid.CurrentRow?.DataBoundItem is not OrderStatusResult selected)
        {
            return;
        }

        _itemsGrid.DataSource = _orderService.GetOrderItems(selected.OrderId);
    }
}
