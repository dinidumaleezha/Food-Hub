using System.Drawing;
using System.Windows.Forms;

namespace FoodHub.UI;

public class MainForm : Form
{
    public MainForm()
    {
        Text = "FoodHub Delivery Management System";
        Width = 900;
        Height = 600;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

        Controls.Add(UiHelpers.BuildNavPanel(this));

        var heroLabel = new Label
        {
            Text = "Welcome to FoodHub Delivery Management System",
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point)
        };

        Controls.Add(heroLabel);
    }
}
