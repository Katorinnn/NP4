using iTextSharp.text.pdf;
using iTextSharp.text;
using LPG_Management_System.Models;
using LPG_Management_System.View.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
                    pageIndicator.Text = $"Page {inventorySalesCurrentPage} of {inventorySalesTotalPages}";
                    break;

                case "SalesByProduct":
                    var salesPageData = allSalesByProductReports.Skip(startIndex).Take(itemsPerPage).ToList();
                    salesByProductDG.ItemsSource = salesPageData;
                    pageIndicator.Text = $"Page {inventorySalesCurrentPage} of {inventorySalesTotalPages}";
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

        private void FilterReportsByDate(DataGrid targetDataGrid)
        {
            if (!beginDatePicker.SelectedDate.HasValue || !endDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select both Begin Date and End Date.");
                return;
            }

            DateTime startDate = beginDatePicker.SelectedDate.Value.Date;
            DateTime endDate = endDatePicker.SelectedDate.Value.Date;

            if (startDate > endDate)
            {
                MessageBox.Show("Begin Date cannot be later than End Date.");
                return;
            }

            if (targetDataGrid == inventorySalesDG)
            {
                var filteredReports = allInventorySalesReports
                    .Where(report => report.Date.Date >= startDate && report.Date.Date <= endDate)
                    .ToList();

                inventorySalesDG.ItemsSource = filteredReports;
                inventorySalesTotalPages = (int)Math.Ceiling((double)filteredReports.Count / itemsPerPage);
                inventorySalesCurrentPage = 1;

                if (filteredReports.Count == 0)
                {
                    MessageBox.Show("No records found for the selected date range.");
                }
            }
            else if (targetDataGrid == stockCheckDG)
            {
                var filteredReports = allStockCheckReports
                    .Where(report => report.Date.Date >= startDate && report.Date.Date <= endDate)
                    .ToList();

                stockCheckDG.ItemsSource = filteredReports;
                stockCheckTotalPages = (int)Math.Ceiling((double)filteredReports.Count / itemsPerPage);
                stockCheckCurrentPage = 1;

                if (filteredReports.Count == 0)
                {
                    MessageBox.Show("No records found for the selected date range.");
                }
            }
            else if (targetDataGrid == salesByProductDG)
            {
                var filteredReports = allSalesByProductReports
                    .Where(report => report.Date.Date >= startDate && report.Date.Date <= endDate)
                    .ToList();

                salesByProductDG.ItemsSource = filteredReports;
                salesByProductTotalPages = (int)Math.Ceiling((double)filteredReports.Count / itemsPerPage);
                salesByProductCurrentPage = 1;

                if (filteredReports.Count == 0)
                {
                    MessageBox.Show("No records found for the selected date range.");
                }
            }

            // Update the pagination UI
            UpdatePaginationUI(targetDataGrid);
        }


        private void UpdatePaginationUI(DataGrid targetDataGrid)
        {
            if (targetDataGrid == inventorySalesDG)
            {
                pageIndicator.Text = $"Page {inventorySalesCurrentPage} of {inventorySalesTotalPages}";
            }
            else if (targetDataGrid == stockCheckDG)
            {
                pageIndicator.Text = $"Page {stockCheckCurrentPage} of {stockCheckTotalPages}";
            }
            else if (targetDataGrid == salesByProductDG)
            {
                pageIndicator.Text = $"Page {salesByProductCurrentPage} of {salesByProductTotalPages}";
            }
        }
        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (categoryComboBox.SelectedItem is string selectedCategory)
            {
                if (selectedCategory == "Inventory Sales Report")
                {
                    FilterReportsByDate(inventorySalesDG);
                }
                else if (selectedCategory == "Stock Check Sheets")
                {
                    FilterReportsByDate(stockCheckDG);
                }
                else if (selectedCategory == "Sales by Product")
                {
                    FilterReportsByDate(salesByProductDG);
                }
            }
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

                    // Create the PDF using iTextSharp
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        float width = 600f;
                        float height = targetDataGrid.Items.Count * 15f + 100f; // Dynamically set height based on number of items
                        iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(width, height);

                        iTextSharp.text.Document doc = new iTextSharp.text.Document(pageSize, 5f, 5f, 5f, 5f);
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        writer.CloseStream = false;

                        doc.Open();

                        // Add Title and Date of Report
                        Font headerFont = new Font(Font.FontFamily.HELVETICA, 12f, Font.BOLD);
                        Font normalFont = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);

                        doc.Add(new iTextSharp.text.Paragraph($"Report: {selectedCategory}", headerFont) { Alignment = Element.ALIGN_CENTER });
                        doc.Add(new iTextSharp.text.Paragraph($"Date: {DateTime.Now:yyyy-MM-dd}", normalFont) { Alignment = Element.ALIGN_CENTER });
                        doc.Add(new iTextSharp.text.Paragraph(new string('-', 40), normalFont) { Alignment = Element.ALIGN_CENTER });

                        // Add the DataGrid content to the PDF
                        PdfPTable table = new PdfPTable(targetDataGrid.Columns.Count)
                        {
                            WidthPercentage = 100
                        };

                        // Add column headers
                        foreach (var column in targetDataGrid.Columns)
                        {
                            table.AddCell(new PdfPCell(new Phrase(column.Header.ToString(), headerFont)) { Border = 0, HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        }

                        // Add data rows
                        foreach (var item in targetDataGrid.ItemsSource)
                        {
                            foreach (var column in targetDataGrid.Columns)
                            {
                                var cellContent = column.GetCellContent(item) as TextBlock;
                                table.AddCell(new PdfPCell(new Phrase(cellContent?.Text ?? string.Empty, normalFont)) { Border = 0, HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                            }
                        }

                        // Add the table to the document
                        doc.Add(table);

                        // Add Footer
                        doc.Add(new iTextSharp.text.Paragraph(new string('-', 40), normalFont) { Alignment = Element.ALIGN_CENTER });
                        doc.Add(new iTextSharp.text.Paragraph("End of Report", normalFont) { Alignment = Element.ALIGN_CENTER });

                        doc.Close();

                        // Save the PDF to a temporary location
                        string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
                        File.WriteAllBytes(tempFilePath, memoryStream.ToArray());

                        // Open the PDF for preview
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFilePath) { UseShellExecute = true });

                        // Clean up the file after a delay
                        Task.Run(() =>
                        {
                            System.Threading.Thread.Sleep(50000);
                            if (File.Exists(tempFilePath))
                            {
                                File.Delete(tempFilePath);
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating print preview: {ex.Message}");
            }
        }
    }
}


