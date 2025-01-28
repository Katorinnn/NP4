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

        private const int itemsPerPage = 14;

        private int inventorySalesTotalPages;
        private int stockCheckTotalPages;
        private int salesByProductTotalPages;

        private List<ReportsTable> allInventorySalesReports;
        private List<InventoryTable> allStockCheckReports;
        private List<dynamic> allSalesByProductReports;

        private List<ReportsTable> filteredInventorySalesReports;
        private List<InventoryTable> filteredStockCheckReports;
        private List<dynamic> filteredSalesByProductReports;

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

        private int CalculateItemsPerPage(DataGrid dataGrid)
        {
            // Get the height of the DataGrid
            double dataGridHeight = dataGrid.ActualHeight;

            // Estimate the height of the header row
            double headerHeight = dataGrid.ColumnHeaderHeight;

            // Calculate the available height for rows
            double availableHeight = dataGridHeight - headerHeight;

            // Measure the height of an average row
            double averageRowHeight = 0;
            if (dataGrid.Items.Count > 0)
            {
                // Use the first visible row to estimate the row height
                var firstRow = dataGrid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                if (firstRow != null)
                {
                    averageRowHeight = firstRow.ActualHeight;
                }
            }

            // Fallback to a default row height if no rows are visible yet
            if (averageRowHeight == 0)
            {
                averageRowHeight = 30; // Default row height (adjust as needed)
            }

            // Calculate how many rows can fit in the available space
            return (int)(availableHeight / averageRowHeight);
        }



        private void LoadPage(string category, int page)
        {
            int startIndex = (page - 1) * itemsPerPage;

            switch (category)
            {
                case "InventorySales":
                    var inventoryPageData = filteredInventorySalesReports?.Skip(startIndex).Take(itemsPerPage).ToList()
                                          ?? allInventorySalesReports.Skip(startIndex).Take(itemsPerPage).ToList();
                    inventorySalesDG.ItemsSource = inventoryPageData;

                    pageIndicator.Text = $"Page {inventorySalesCurrentPage} of {inventorySalesTotalPages}";
                    UpdatePaginationVisibility(inventorySalesTotalPages);
                    break;

                case "StockCheck":
                    var stockPageData = filteredStockCheckReports?.Skip(startIndex).Take(itemsPerPage).ToList()
                                      ?? allStockCheckReports.Skip(startIndex).Take(itemsPerPage).ToList();
                    stockCheckDG.ItemsSource = stockPageData;

                    pageIndicator.Text = $"Page {stockCheckCurrentPage} of {stockCheckTotalPages}";
                    UpdatePaginationVisibility(stockCheckTotalPages);
                    break;

                case "SalesByProduct":
                    var salesPageData = filteredSalesByProductReports?.Skip(startIndex).Take(itemsPerPage).ToList()
                                       ?? allSalesByProductReports.Skip(startIndex).Take(itemsPerPage).ToList();
                    salesByProductDG.ItemsSource = salesPageData;

                    pageIndicator.Text = $"Page {salesByProductCurrentPage} of {salesByProductTotalPages}";
                    UpdatePaginationVisibility(salesByProductTotalPages);
                    break;
            }
        }


        private void UpdatePaginationVisibility(int totalPages)
        {
            if (totalPages > 1)
            {
                // Show pagination controls if more than one page
                pageIndicator.Visibility = Visibility.Visible;
                previousButton.Visibility = Visibility.Visible;
                nextButton.Visibility = Visibility.Visible;
            }
            else
            {
                // Hide pagination controls if only one page
                pageIndicator.Visibility = Visibility.Collapsed;
                previousButton.Visibility = Visibility.Collapsed;
                nextButton.Visibility = Visibility.Collapsed;
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
                        UpdatePaginationVisibility(inventorySalesTotalPages); // Ensure visibility updates
                        break;

                    case "Stock Check Sheets":
                        stockCheckDG.Visibility = Visibility.Visible;
                        LoadStockCheckData();
                        UpdatePaginationVisibility(stockCheckTotalPages); // Ensure visibility updates
                        break;

                    case "Sales by Product":
                        salesByProductDG.Visibility = Visibility.Visible;
                        LoadSalesByProductData();
                        UpdatePaginationVisibility(salesByProductTotalPages); // Ensure visibility updates
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
            DateTime? startDate = beginDatePicker.SelectedDate?.Date;
            DateTime? endDate = endDatePicker.SelectedDate?.Date;

            if (startDate == null && endDate == null)
            {
                MessageBox.Show("Please select at least one date (Begin Date or End Date).");
                return;
            }

            if (startDate != null && endDate != null && startDate > endDate)
            {
                MessageBox.Show("Begin Date cannot be later than End Date.");
                return;
            }

            DateTime endDateExclusive = endDate?.AddDays(1) ?? DateTime.MaxValue;

            if (targetDataGrid == inventorySalesDG)
            {
                // Filter the data
                filteredInventorySalesReports = allInventorySalesReports
                    .Where(report =>
                        (startDate == null || report.Date >= startDate) &&
                        (endDate == null || report.Date < endDateExclusive))
                    .ToList();

                // Update pagination and load the first page
                inventorySalesTotalPages = (int)Math.Ceiling((double)filteredInventorySalesReports.Count / itemsPerPage);
                inventorySalesCurrentPage = 1;

                if (filteredInventorySalesReports.Count == 0)
                {
                    MessageBox.Show("No records found for the selected date range.");
                }

                LoadPage("InventorySales", inventorySalesCurrentPage);
            }
            else if (targetDataGrid == stockCheckDG)
            {
                filteredStockCheckReports = allStockCheckReports
                    .Where(report =>
                        (startDate == null || report.Date.Date >= startDate) &&
                        (endDate == null || report.Date.Date < endDateExclusive))
                    .ToList();

                stockCheckTotalPages = (int)Math.Ceiling((double)filteredStockCheckReports.Count / itemsPerPage);
                stockCheckCurrentPage = 1;

                if (filteredStockCheckReports.Count == 0)
                {
                    MessageBox.Show("No records found for the selected date range.");
                }

                LoadPage("StockCheck", stockCheckCurrentPage);
            }
            else if (targetDataGrid == salesByProductDG)
            {
                filteredSalesByProductReports = allSalesByProductReports
                    .Where(report =>
                        (startDate == null || report.Date.Date >= startDate) &&
                        (endDate == null || report.Date.Date < endDateExclusive))
                    .ToList();

                salesByProductTotalPages = (int)Math.Ceiling((double)filteredSalesByProductReports.Count / itemsPerPage);
                salesByProductCurrentPage = 1;

                if (filteredSalesByProductReports.Count == 0)
                {
                    MessageBox.Show("No records found for the selected date range.");
                }

                LoadPage("SalesByProduct", salesByProductCurrentPage);
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
                    UpdatePaginationVisibility(inventorySalesTotalPages);
                }
                else if (selectedCategory == "Stock Check Sheets")
                {
                    FilterReportsByDate(stockCheckDG);
                    UpdatePaginationVisibility(stockCheckTotalPages);
                }
                else if (selectedCategory == "Sales by Product")
                {
                    FilterReportsByDate(salesByProductDG);
                    UpdatePaginationVisibility(salesByProductTotalPages);
                }
            }
        }



        private void CancelFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset the DatePickers to null (clear the selected dates)
            beginDatePicker.SelectedDate = null;
            endDatePicker.SelectedDate = null;

            // Reload all reports without filtering
            LoadInventorySalesData();
        }





        private void PrintPreview_Click(object sender, RoutedEventArgs e)
        {
            var companyDetails = GetCompanyDetails();

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
                        // Use the A4 page size (portrait)
                        iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 36f, 36f, 36f, 36f); // A4 with 1-inch margins (36f points)
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        writer.CloseStream = false;

                        doc.Open();

                        PdfPTable topBorderTable = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };
                        topBorderTable.DefaultCell.BorderWidthTop = 1.5f;
                        topBorderTable.DefaultCell.Border = Rectangle.TOP_BORDER; // Add a top border
                        topBorderTable.AddCell(""); // Add a blank cell to create the border effect
                        doc.Add(topBorderTable);

                        // Add Title and Date of Report
                        Font headerFont = new Font(Font.FontFamily.HELVETICA, 12f, Font.BOLD);
                        Font semiheaderFont = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);
                        Font normalFont = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);

                        PdfPTable titleTable = new PdfPTable(2)
                        {
                            WidthPercentage = 100
                        };

                        // Set column widths (the first column will take more space, the second column will take the minimum space)
                        titleTable.SetWidths(new float[] { 70f, 30f });  // Adjust the values based on your layout needs

                        // Add the category name to the left column
                        titleTable.AddCell(new PdfPCell(new Phrase(selectedCategory, headerFont))
                        {
                            Border = 0,
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        });

                        // Add the company name to the right column
                        titleTable.AddCell(new PdfPCell(new Phrase(companyDetails.CompanyName, headerFont))
                        {
                            Border = 0,
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        });

                        // Add the table to the document
                        doc.Add(titleTable);
                        doc.Add(new iTextSharp.text.Paragraph($"Date: {DateTime.Now:yyyy-MM-dd}", normalFont) { Alignment = Element.ALIGN_LEFT });
                        PdfPTable BottomHeaderBorder = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };
                        topBorderTable.DefaultCell.BorderWidthTop = 1.5f;
                        topBorderTable.DefaultCell.Border = Rectangle.TOP_BORDER; // Add a top border
                        topBorderTable.AddCell(""); // Add a blank cell to create the border effect
                        doc.Add(topBorderTable);

                        // Add the DataGrid content to the PDF
                        PdfPTable table = new PdfPTable(targetDataGrid.Columns.Count)
                        {
                            WidthPercentage = 100
                        };

                        // Add column headers
                        foreach (var column in targetDataGrid.Columns)
                        {
                            table.AddCell(new PdfPCell(new Phrase(column.Header.ToString(), semiheaderFont)) { Border = 0, HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        }

                        // Add data rows
                        foreach (var item in targetDataGrid.ItemsSource)
                        {
                            foreach (var column in targetDataGrid.Columns)
                            {
                                var cellContent = column.GetCellContent(item);

                                // Variable to hold the actual text
                                string cellText = string.Empty;

                                // Check if the content is a ContentPresenter, as that's what typically holds the TextBlock
                                if (cellContent is ContentPresenter contentPresenter)
                                {
                                    // Use VisualTreeHelper to find the TextBlock inside the ContentPresenter
                                    var textBlock = FindChild<TextBlock>(contentPresenter);

                                    // If we find the TextBlock, extract its text
                                    if (textBlock != null)
                                    {
                                        cellText = textBlock.Text;
                                    }
                                }
                                else if (cellContent is TextBlock textBlock)
                                {
                                    // If the content is directly a TextBlock, extract the text
                                    cellText = textBlock.Text;
                                }
                                else
                                {
                                    // If it's neither a ContentPresenter nor a TextBlock, use ToString
                                    cellText = cellContent?.ToString() ?? string.Empty;
                                }

                                // Add the cell's text to the PDF table cell
                                table.AddCell(new PdfPCell(new Phrase(cellText, normalFont)) { Border = 0, HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                            }
                        }




                        // Add the table to the document
                        doc.Add(table);

                        // Add Footer
                        PdfPTable BottomBorder = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };
                        topBorderTable.DefaultCell.BorderWidthTop = 1.5f;
                        topBorderTable.DefaultCell.Border = Rectangle.TOP_BORDER; // Add a top border
                        topBorderTable.AddCell(""); // Add a blank cell to create the border effect
                        doc.Add(topBorderTable);

                        doc.Close();

                        // Save the PDF to a temporary location
                        string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
                        File.WriteAllBytes(tempFilePath, memoryStream.ToArray());

                        // Open the PDF for preview
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFilePath) { UseShellExecute = true });

                        // Clean up the file after a delay
                        Task.Run(() =>
                        {
                            System.Threading.Thread.Sleep(500000);
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
        private CompanyTable GetCompanyDetails()
        {
            using (var context = new DataContext())
            {
                // Assuming there is only one company record
                return context.tbl_company.FirstOrDefault();
            }
        }

        private T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            // Check if the parent is null
            if (parent == null)
                return null;

            // Try to find the child of the given type (T)
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    return (T)child;

                // Recursively search in the child
                var result = FindChild<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}


