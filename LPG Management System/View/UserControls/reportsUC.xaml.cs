using LPG_Management_System.Models;
using LPG_Management_System.View.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;


namespace LPG_Management_System.View.UserControls
{
    public partial class reportsUC : UserControl
    {
        private int inventorySalesCurrentPage = 1;
        private int stockCheckCurrentPage = 1;
        private int salesByProductCurrentPage = 1;

        private const int itemsPerPage = 15;

        private int inventorySalesTotalPages;
        private int stockCheckTotalPages;
        private int salesByProductTotalPages;

        private List<ReportsTable> allInventorySalesReports;
        private List<InventoryTable> allStockCheckReports;
        private List<dynamic> allSalesByProductReports;

        public reportsUC()
        {
            InitializeComponent();
            LoadInventorySalesData();
            LoadStockCheckData();
            LoadSalesByProductData();
            InitializeCategoryComboBox();

            beginDatePicker.DisplayDateEnd = DateTime.Today;
            endDatePicker.DisplayDateEnd = DateTime.Today;
        }

        private void LoadInventorySalesData()
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    allInventorySalesReports = dbContext.tbl_reports.OrderByDescending(r => r.Date).ToList();
                    inventorySalesTotalPages = (int)Math.Ceiling((double)allInventorySalesReports.Count / itemsPerPage);
                    LoadPage("InventorySales", inventorySalesCurrentPage);
                }
            }
            catch (Exception ex)
            {   
                MessageBox.Show($"Error fetching inventory sales data: {ex.Message}");
            }
        }

        private void LoadStockCheckData()
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    var stockData = dbContext.tbl_inventory.ToList();
                    stockCheckDG.ItemsSource = stockData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading stock check data: {ex.Message}");
            }
        }

        private void LoadSalesByProductData()
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    var salesData = dbContext.tbl_reports
                        .GroupBy(report => report.ProductName)
                        .Select(group => new
                        {
                            ProductName = group.Key,
                            TotalQuantitySold = group.Sum(report => report.Quantity),
                            TotalSales = group.Sum(report => report.TotalPrice)
                        })
                        .ToList();
                    salesByProductDG.ItemsSource = salesData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sales by product data: {ex.Message}");
            }
        }


        private void LoadPage(string category, int page)
        {
            int startIndex = (page - 1) * itemsPerPage;

            switch (category)
            {
                case "InventorySales":
                    var inventoryPageData = allInventorySalesReports.Skip(startIndex).Take(itemsPerPage).ToList();
                    inventorySalesDG.ItemsSource = inventoryPageData;
                    pageIndicator.Text = $"Page {inventorySalesCurrentPage} of {inventorySalesTotalPages}";
                    break;

                case "StockCheck":
                    var stockPageData = allStockCheckReports.Skip(startIndex).Take(itemsPerPage).ToList();
                    stockCheckDG.ItemsSource = stockPageData;
                    break;

                case "SalesByProduct":
                    var salesPageData = allSalesByProductReports.Skip(startIndex).Take(itemsPerPage).ToList();
                    salesByProductDG.ItemsSource = salesPageData;
                    break;
            }
        }


        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (categoryComboBox.SelectedItem is string selectedCategory)
            {
                if (selectedCategory == "Inventory Sales Report" && inventorySalesCurrentPage > 1)
                {
                    inventorySalesCurrentPage--;
                    LoadPage("InventorySales", inventorySalesCurrentPage);
                }
                else if (selectedCategory == "Stock Check Sheets" && stockCheckCurrentPage > 1)
                {
                    stockCheckCurrentPage--;
                    LoadPage("StockCheck", stockCheckCurrentPage);
                }
                else if (selectedCategory == "Sales by Product" && salesByProductCurrentPage > 1)
                {
                    salesByProductCurrentPage--;
                    LoadPage("SalesByProduct", salesByProductCurrentPage);
                }
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (categoryComboBox.SelectedItem is string selectedCategory)
            {
                if (selectedCategory == "Inventory Sales Report" && inventorySalesCurrentPage < inventorySalesTotalPages)
                {
                    inventorySalesCurrentPage++;
                    LoadPage("InventorySales", inventorySalesCurrentPage);
                }
                else if (selectedCategory == "Stock Check Sheets" && stockCheckCurrentPage < stockCheckTotalPages)
                {
                    stockCheckCurrentPage++;
                    LoadPage("StockCheck", stockCheckCurrentPage);
                }
                else if (selectedCategory == "Sales by Product" && salesByProductCurrentPage < salesByProductTotalPages)
                {
                    salesByProductCurrentPage++;
                    LoadPage("SalesByProduct", salesByProductCurrentPage);
                }
            }
        }

        private void InitializeCategoryComboBox()
        {
            try
            {
                categoryComboBox.ItemsSource = new List<string>
                {
                    "Inventory Sales Report",
                    "Stock Check Sheets",
                    "Sales by Product"
                };
                categoryComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing categories: {ex.Message}");
            }
        }

        private void categoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoryComboBox.SelectedItem is string selectedCategory)
            {
                // Hide all DataGrids initially
                inventorySalesDG.Visibility = Visibility.Collapsed;
                stockCheckDG.Visibility = Visibility.Collapsed;
                salesByProductDG.Visibility = Visibility.Collapsed;

                switch (selectedCategory)
                {
                    case "Inventory Sales Report":
                        inventorySalesDG.Visibility = Visibility.Visible;
                        LoadInventorySalesData();
                        break;

                    case "Stock Check Sheets":
                        stockCheckDG.Visibility = Visibility.Visible;
                        LoadStockCheckData();
                        break;

                    case "Sales by Product":
                        salesByProductDG.Visibility = Visibility.Visible;
                        LoadSalesByProductData();
                        break;
                }
            }
        }



        private void beginDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (beginDatePicker.SelectedDate.HasValue)
            {
                // Optionally display a formatted date (dd/MM/yyyy)
                beginDatePicker.Text = beginDatePicker.SelectedDate.Value.ToString("yyyy/MM/dd");
            }
        }

        private void endDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (endDatePicker.SelectedDate.HasValue)
            {
                // Optionally display a formatted date (dd/MM/yyyy)
                endDatePicker.Text = endDatePicker.SelectedDate.Value.ToString("yyyy/MM/dd");
            }
        }


        private void reportsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection changes if necessary
        }

        //private void FilterReportsByDate()
        //{
        //    if (beginDatePicker.SelectedDate.HasValue && endDatePicker.SelectedDate.HasValue)
        //    {
        //        DateTime startDate = beginDatePicker.SelectedDate.Value.Date;
        //        DateTime endDate = endDatePicker.SelectedDate.Value.Date.AddDays(1).AddMilliseconds(-1);

        //        if (startDate > endDate)
        //        {
        //            MessageBox.Show("Begin Date cannot be later than End Date.");
        //            return;
        //        }

        //        var filteredReports = allReports
        //            .Where(report => report.Date >= startDate && report.Date <= endDate)
        //            .ToList();

        //        if (filteredReports.Count == 0)
        //        {
        //            MessageBox.Show("No records found for the selected date range.");
        //            return;
        //        }

        //        allReports = filteredReports;
        //        currentPage = 1;
        //        totalPages = (int)Math.Ceiling((double)allReports.Count / itemsPerPage);
        //        LoadPage(currentPage);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please select both Begin Date and End Date.");
        //    }
        //}

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            //FilterReportsByDate();
        }

        private void CancelFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset the DatePickers to null (clear the selected dates)
            beginDatePicker.SelectedDate = null;
            endDatePicker.SelectedDate = null;

            // Optionally, reset other filters like Category combo box
            categoryComboBox.SelectedIndex = 0;

            // Reload all reports without filtering
            LoadInventorySalesData();
        }





        private void PrintPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (categoryComboBox.SelectedItem is string selectedCategory)
                {
                    DataGrid targetDataGrid = selectedCategory switch
                    {
                        "Inventory Sales Report" => inventorySalesDG,
                        "Stock Check Sheets" => stockCheckDG,
                        "Sales by Product" => salesByProductDG,
                        _ => null
                    };

                    if (targetDataGrid == null) return;

                    var printPreviewWindow = new PrintPreview(CreateFixedDocumentFromDataGrid(targetDataGrid));
                    printPreviewWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating print preview: {ex.Message}");
            }
        }

        private FixedDocument CreateFixedDocumentFromDataGrid(DataGrid dataGrid)
        {
            FixedDocument document = new FixedDocument();
            Size pageSize = new Size(800, 1120);
            document.DocumentPaginator.PageSize = pageSize;

            FixedPage page = new FixedPage { Width = pageSize.Width, Height = pageSize.Height };
            page.Children.Add(new TextBlock
            {
                Text = "Report Preview",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10),
                TextAlignment = TextAlignment.Center
            });

            DataGrid printGrid = new DataGrid { ItemsSource = dataGrid.ItemsSource, Width = 750, Height = 1000 };
            page.Children.Add(printGrid);

            PageContent pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(page);
            document.Pages.Add(pageContent);

            return document;
        }




        private UIElement CreateHeaderCell(string text, int column)
        {
            return CreateTextCell(text, column, FontWeights.Bold);
        }

        private UIElement CreateDataCell(string text, int row, int column)
        {
            return CreateTextCell(text, column, FontWeights.Normal, row);
        }

        private UIElement CreateTextCell(string text, int column, FontWeight fontWeight, int row = 0)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                FontWeight = fontWeight,
                Margin = new Thickness(2),
                TextAlignment = TextAlignment.Center
            };

            Border border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0.5),
                Child = textBlock
            };

            Grid.SetColumn(border, column);
            Grid.SetRow(border, row);

            return border;
        }
    }
}


