using System.Drawing;
using System.Windows.Forms;
using FoodHub.Data;
using FoodHub.Models;
using FoodHub.Services;

namespace FoodHub.UI;

public class RiderAssignmentForm : Form
{
    private readonly OrderService _orderService;
    private readonly DeliveryService _deliveryService;
    private readonly RiderRepository _riderRepository;
    private readonly MotorbikeRepository _motorbikeRepository;
    private readonly ComboBox _orderComboBox;
    private readonly ComboBox _riderComboBox;
    private readonly ComboBox _bikeComboBox;
    private readonly DateTimePicker _assignmentPicker;
    private readonly TextBox _startMeterTextBox;
    private readonly TextBox _endMeterTextBox;

    public RiderAssignmentForm()
    {
        _orderService = new OrderService(new OrderRepository());
        _deliveryService = new DeliveryService(new RiderBikeAssignmentRepository(), new OrderRepository());
        _riderRepository = new RiderRepository();
        _motorbikeRepository = new MotorbikeRepository();

        Text = "Rider Assignment";
        Width = 700;
        Height = 500;
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

        Controls.Add(UiHelpers.BuildNavPanel(this));

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(30, 80, 30, 30),
            ColumnCount = 2,
            RowCount = 7
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));

        _orderComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 250 };
        _riderComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 250 };
        _bikeComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 250 };
        _assignmentPicker = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 250 };
        _startMeterTextBox = new TextBox { Width = 250 };
        _endMeterTextBox = new TextBox { Width = 250 };

        AddRow(panel, 0, "Order ID", _orderComboBox);
        AddRow(panel, 1, "Rider", _riderComboBox);
        AddRow(panel, 2, "Motorbike", _bikeComboBox);
        AddRow(panel, 3, "Assignment Date", _assignmentPicker);
        AddRow(panel, 4, "Start Meter", _startMeterTextBox);
        AddRow(panel, 5, "End Meter", _endMeterTextBox);

        var assignButton = new Button { Text = "Assign Rider", Width = 140, Height = 35 };
        assignButton.Click += (_, _) => AssignRider();
        panel.Controls.Add(assignButton, 1, 6);

        Controls.Add(panel);

        Load += (_, _) => LoadData();
    }

    private static void AddRow(TableLayoutPanel layout, int row, string labelText, Control control)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var label = new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left };
        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(control, 1, row);
    }

    private void LoadData()
    {
        _orderComboBox.DataSource = _orderService.GetOrdersForAssignment();

        var riders = _riderRepository.GetAll();
        _riderComboBox.DataSource = riders;
        _riderComboBox.DisplayMember = "FullName";
        _riderComboBox.ValueMember = "RiderId";

        var bikes = _motorbikeRepository.GetAll();
        _bikeComboBox.DataSource = bikes;
        _bikeComboBox.DisplayMember = "DisplayName";
        _bikeComboBox.ValueMember = "BikeRegNo";
    }

    private void AssignRider()
    {
        try
        {
            if (_orderComboBox.SelectedItem is not int orderId)
            {
                MessageBox.Show("Select an order.", "Missing Order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_riderComboBox.SelectedValue is not int riderId)
            {
                MessageBox.Show("Select a rider.", "Missing Rider", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_bikeComboBox.SelectedValue is not string bikeRegNo)
            {
                MessageBox.Show("Select a motorbike.", "Missing Bike", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(_startMeterTextBox.Text.Trim(), out var startMeter))
            {
                MessageBox.Show("Enter a valid start meter.", "Invalid Meter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int? endMeter = null;
            if (!string.IsNullOrWhiteSpace(_endMeterTextBox.Text))
            {
                if (!int.TryParse(_endMeterTextBox.Text.Trim(), out var parsedEnd))
                {
                    MessageBox.Show("Enter a valid end meter.", "Invalid Meter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                endMeter = parsedEnd;
            }

            var assignment = new RiderBikeAssignment
            {
                RiderId = riderId,
                BikeRegNo = bikeRegNo,
                AssignmentDate = _assignmentPicker.Value.Date,
                StartMeter = startMeter,
                EndMeter = endMeter
            };

            _deliveryService.AssignRider(orderId, assignment);
            MessageBox.Show("Rider assigned successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ResetForm();
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ResetForm()
    {
        _startMeterTextBox.Text = string.Empty;
        _endMeterTextBox.Text = string.Empty;
    }
}
