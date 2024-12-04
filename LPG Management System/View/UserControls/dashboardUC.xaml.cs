using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LPG_Management_System.View.UserControls
{
    /// <summary>
    /// Interaction logic for dashboarUC.xaml
    /// </summary>
    public partial class dashboardUC : UserControl
    {
        public dashboardUC()
        {
            InitializeComponent();

            double[] sales = { 150, 200, 180, 250, 300, 350, 400, 380, 420, 450, 470, 500 };

            // Days of the month (example labels)
            string[] days = { "1", "5", "10", "15", "20", "25", "30" };

            // Configuring the Chart
            SalesChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Sales",
                    Values = new ChartValues<double>(sales)
                }
            };

            // Adding Labels
            SalesChart.AxisX.Add(new Axis
            {
                Title = "Days",
                Labels = days
            });

            SalesChart.AxisY.Add(new Axis
            {
                Title = "Sales ($)",
                LabelFormatter = value => value.ToString("C") // Format as currency
            });

            SalesChart.LegendLocation = LegendLocation.Right;
        }
    }
}
