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
            LoadMonthlySalesChart();
            LoadSalesByProductChart();
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
                var soldLPG = context.tbl_reports.Sum(r => r.Quantity);

                // Fetch stocks
                var stocks = context.tbl_inventory.Sum(s => s.Stocks);

                // Update UI elements
                lblTodaysIncome.Content = todaysIncome.ToString("C");
                lblTotalIncome.Content = totalIncome.ToString("C");
                lblSoldLPG.Content = soldLPG.ToString();
                lblStocks.Content = stocks.ToString();
            }
        }

        private void LoadMonthlySalesChart()
        {
            using (var context = new DataContext())
            {
                // Get the current month
                var currentMonth = DateTime.Today.Month;
                var currentYear = DateTime.Today.Year;

                // Fetch and group sales by day of the current month
                var salesData = context.tbl_reports
                    .Where(r => r.Date.Year == currentYear && r.Date.Month == currentMonth)
                    .GroupBy(r => r.Date.Day)
                    .Select(g => new
                    {
                        Day = g.Key,
                        TotalSales = g.Sum(r => (decimal?)r.TotalPrice) ?? 0
                    })
                    .OrderBy(g => g.Day)
                    .ToList();

                // Fill sales and labels
                var sales = salesData.Select(d => (double)d.TotalSales).ToArray();
                var days = salesData.Select(d => d.Day.ToString()).ToArray();

                // Configure the chart
                MonthlySalesChart.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Sales",
                        Values = new ChartValues<double>(sales),
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 10
                    }
                };

                // Update X-Axis labels
                MonthlySalesChart.AxisX.Clear();
                MonthlySalesChart.AxisX.Add(new Axis
                {
                    Title = "Days",
                    Labels = days
                });

                // Update Y-Axis labels
                MonthlySalesChart.AxisY.Clear();
                MonthlySalesChart.AxisY.Add(new Axis
                {
                    Title = "Sales ($)",
                    LabelFormatter = value => value.ToString("C")
                });

                MonthlySalesChart.LegendLocation = LegendLocation.Right;
            }
        }

        private void LoadSalesByProductChart()
        {
            using (var context = new DataContext())
            {
                // Dictionary to store aggregated product sales
                var productSalesData = new Dictionary<string, double>();

                // Fetch all reports
                var reports = context.tbl_reports.ToList();

                foreach (var report in reports)
                {
                    // Split the ProductName by comma and trim whitespace
                    var products = report.ProductName.Split(',')
                        .Select(p => p.Trim())
                        .ToArray();

                    foreach (var product in products)
                    {
                        // Add the quantity sold for each product
                        if (productSalesData.ContainsKey(product))
                        {
                            productSalesData[product] += report.Quantity;
                        }
                        else
                        {
                            productSalesData[product] = report.Quantity;
                        }
                    }
                }

                // Clear the current chart data
                SalesDistributionChart.Series = new SeriesCollection();

                // Add each product to the pie chart
                foreach (var product in productSalesData)
                {
                    SalesDistributionChart.Series.Add(new PieSeries
                    {
                        Title = product.Key, // Product Name
                        Values = new ChartValues<double> { product.Value }, // Quantity Sold
                        DataLabels = true // Display the data labels
                    });
                }

                // Optional: Set the chart's legend location
                SalesDistributionChart.LegendLocation = LegendLocation.Right;
            }
        }


    }
}
