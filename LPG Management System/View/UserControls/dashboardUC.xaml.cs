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
using LPG_Management_System.Models;

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
            LoadDashboardData();


            double[] sales = { 150, 200, 180, 250, 350, 450, 400, 380, 420, 450, 470, 500 };

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
        private void LoadDashboardData()
        {
            using (var context = new DataContext())
            {
                // Fetch today's income
                var today = DateTime.Today;

                var todaysIncome = context.tbl_reports
                    .Where(r => r.Date.Date == today)
                    .Sum(r => r.TotalPrice);

                // Fetch total income
                var totalIncome = context.tbl_reports.Sum(r => r.TotalPrice);

                // Fetch sold LPG
                var soldLPG = context.tbl_inventory
                    .Where(i => i.Date.Date == today)
                    .Sum(i => i.Quantity);

                // Fetch stocks
                var stocks = context.tbl_stocks.Sum(s => s.Quantity);

                // Update UI elements
                lblTodaysIncome.Content = todaysIncome.ToString("C");
                lblTotalIncome.Content = totalIncome.ToString("C");
                lblSoldLPG.Content = soldLPG.ToString();
                lblStocks.Content = stocks.ToString();
            }
        }
    }
}
