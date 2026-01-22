using System.Drawing;
using System.Windows.Forms;

namespace FoodHub.UI;

internal static class UiHelpers
{
    public static Panel BuildNavPanel(Form parent)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(32, 64, 96)
        };

        var font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);

        var customersButton = CreateNavButton("CUSTOMERS", 10, font, () => new CustomerForm());
        var ordersButton = CreateNavButton("ORDERS", 130, font, () => new NewOrderForm());
        var deliveryButton = CreateNavButton("DELIVERY", 250, font, () => new RiderAssignmentForm());
        var statusButton = CreateNavButton("STATUS", 370, font, () => new OrderStatusForm());

        panel.Controls.Add(customersButton);
        panel.Controls.Add(ordersButton);
        panel.Controls.Add(deliveryButton);
        panel.Controls.Add(statusButton);

        return panel;
    }

    private static Button CreateNavButton(string text, int left, Font font, Func<Form> formFactory)
    {
        var button = new Button
        {
            Text = text,
            Left = left,
            Top = 10,
            Width = 110,
            Height = 30,
            BackColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = font
        };

        button.FlatAppearance.BorderColor = Color.White;
        button.Click += (_, _) =>
        {
            var form = formFactory();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        };

        return button;
    }
}
