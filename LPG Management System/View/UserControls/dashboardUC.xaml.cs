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
                var today = DateTime.Today;

                var todaysIncome = context.tbl_reports
                    .Where(r => r.Date.Date == today)
                    .Sum(r => r.TotalPrice);

                var totalIncome = context.tbl_reports.Sum(r => r.TotalPrice);

                var soldLPG = context.tbl_reports.Sum(r => r.Quantity);

                var stocks = context.tbl_inventory.Sum(s => s.Stocks);

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
                var currentMonth = DateTime.Today.Month;
                var currentYear = DateTime.Today.Year;

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

                var sales = salesData.Select(d => (double)d.TotalSales).ToArray();
                var days = salesData.Select(d => d.Day.ToString()).ToArray();

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

                MonthlySalesChart.AxisX.Clear();
                MonthlySalesChart.AxisX.Add(new Axis
                {
                    Title = "Days",
                    Labels = days
                });

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
                var productSalesData = new Dictionary<string, double>();

                var reports = context.tbl_reports.ToList();

                foreach (var report in reports)
                {
                    var products = report.ProductName.Split(',')
                        .Select(p => p.Trim())
                        .ToArray();

                    foreach (var product in products)
                    {
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

                SalesDistributionChart.Series = new SeriesCollection();

                foreach (var product in productSalesData)
                {
                    SalesDistributionChart.Series.Add(new PieSeries
                    {
                        Title = product.Key,
                        Values = new ChartValues<double> { product.Value }, 
                        DataLabels = true
                    });
                }
                SalesDistributionChart.LegendLocation = LegendLocation.Right;
            }
        }


    }
}
