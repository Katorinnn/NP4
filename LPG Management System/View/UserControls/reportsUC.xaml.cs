﻿using iTextSharp.text.pdf;
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
                    allStockCheckReports = stockData; 
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
                    var reports = dbContext.tbl_reports.ToList();

                    var salesData = reports
                        .SelectMany(report => report.ProductName.Split(',')
                            .Select(product => new
                            {
                                ProductName = product.Trim(),
                                report.Quantity,
                                report.TotalPrice
                            }))
                        .GroupBy(entry => entry.ProductName)
                        .Select(group => new
                        {
                            ProductName = group.Key,
                            TotalQuantitySold = group.Sum(entry => entry.Quantity),
                            TotalSales = group.Sum(entry => entry.TotalPrice)
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
            double dataGridHeight = dataGrid.ActualHeight;
            double headerHeight = dataGrid.ColumnHeaderHeight;
            double availableHeight = dataGridHeight - headerHeight;
            double averageRowHeight = 0;

            if (dataGrid.Items.Count > 0)
            {
                var firstRow = dataGrid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                if (firstRow != null)
                {
                    averageRowHeight = firstRow.ActualHeight;
                }
            }
            if (averageRowHeight == 0)
            {
                averageRowHeight = 30;
            }
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
                pageIndicator.Visibility = Visibility.Visible;
                previousButton.Visibility = Visibility.Visible;
                nextButton.Visibility = Visibility.Visible;
            }
            else
            {
                pageIndicator.Visibility = Visibility.Collapsed;
                previousButton.Visibility = Visibility.Collapsed;
                nextButton.Visibility = Visibility.Collapsed;
            }
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
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

        private void nextBtn_Click(object sender, RoutedEventArgs e)
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
                        UpdatePaginationVisibility(inventorySalesTotalPages); 
                        break;

                    case "Stock Check Sheets":
                        stockCheckDG.Visibility = Visibility.Visible;
                        LoadStockCheckData();
                        UpdatePaginationVisibility(stockCheckTotalPages); 
                        break;

                    case "Sales by Product":
                        salesByProductDG.Visibility = Visibility.Visible;
                        LoadSalesByProductData();
                        UpdatePaginationVisibility(salesByProductTotalPages);
                        break;
                }
            }
        }
        private void beginDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (beginDatePicker.SelectedDate.HasValue)
            {
                beginDatePicker.Text = beginDatePicker.SelectedDate.Value.ToString("yyyy/MM/dd");
            }
        }
        private void endDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (endDatePicker.SelectedDate.HasValue)
            {
                endDatePicker.Text = endDatePicker.SelectedDate.Value.ToString("yyyy/MM/dd");
            }
        }
        private void FilterReportsByDate(DataGrid targetDataGrid)
        {
            if (targetDataGrid == null)
            {
                MessageBox.Show("Invalid data grid. Please try again.");
                return;
            }
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

            if (targetDataGrid.ItemsSource == null)
            {
                MessageBox.Show("No data available to filter.");
                return;
            }

            if (targetDataGrid == inventorySalesDG)
            {
                filteredInventorySalesReports = allInventorySalesReports
                    .Where(report =>
                        (startDate == null || report.Date >= startDate) &&
                        (endDate == null || report.Date < endDateExclusive))
                    .ToList();

                inventorySalesTotalPages = (int)Math.Ceiling((double)filteredInventorySalesReports.Count / itemsPerPage);
                inventorySalesCurrentPage = 1;

            if (!filteredInventorySalesReports.Any())
            {
                MessageBox.Show("No records found for the selected date range.");
            }

            LoadPage("InventorySales", inventorySalesCurrentPage);
            }
                else if (targetDataGrid == stockCheckDG)
                {
                    if (allStockCheckReports == null)
                    {
                        MessageBox.Show("Stock Check Reports data is missing. Please ensure data is loaded.");
                        return;
                    }

                        filteredStockCheckReports = allStockCheckReports
                            .Where(report =>
                                (startDate == null || report.Date >= startDate) &&
                                (endDate == null || report.Date < endDateExclusive))
                            .ToList();

                        stockCheckTotalPages = (int)Math.Ceiling((double)filteredStockCheckReports.Count / itemsPerPage);
                        stockCheckCurrentPage = 1;

                        if (!filteredStockCheckReports.Any())
                        {
                            MessageBox.Show("No records found for the selected date range.");
                        }

                        LoadPage("StockCheck", stockCheckCurrentPage);
                    }
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



        private void CancelFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            beginDatePicker.SelectedDate = null;
            endDatePicker.SelectedDate = null;

            if (categoryComboBox.SelectedItem is string selectedCategory)
            {
                switch (selectedCategory)
                {
                    case "Inventory Sales Report":
                        LoadInventorySalesData();
                        break;

                    case "Stock Check Sheets":
                        LoadStockCheckData();
                        break;

                    case "Sales by Product":
                        LoadSalesByProductData();
                        break;
                }
            }
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

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 36f, 36f, 36f, 36f); 
                        PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                        writer.CloseStream = false;

                        doc.Open();


                        PdfPTable topBorderTable = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };
                        topBorderTable.DefaultCell.BorderWidthTop = 1.5f;
                        topBorderTable.DefaultCell.Border = Rectangle.TOP_BORDER; 
                        topBorderTable.AddCell(""); 
                        doc.Add(topBorderTable);

                        Font headerFont = new Font(Font.FontFamily.HELVETICA, 12f, Font.BOLD);
                        Font semiheaderFont = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);
                        Font normalFont = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);

                        PdfPTable titleTable = new PdfPTable(2)
                        {
                            WidthPercentage = 100
                        };
                        titleTable.SetWidths(new float[] { 70f, 30f });

                        doc.Add(new iTextSharp.text.Paragraph(new iTextSharp.text.Phrase(new string(' ', 70)))); 
                                                                                                                 

                        titleTable.AddCell(new PdfPCell(new Phrase(selectedCategory, headerFont))
                        {
                            Border = 0,
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        });

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
                        topBorderTable.DefaultCell.Border = Rectangle.TOP_BORDER; 
                        topBorderTable.AddCell(""); 
                        doc.Add(new iTextSharp.text.Paragraph(new iTextSharp.text.Phrase(new string(' ', 70)))); 
                        doc.Add(topBorderTable);

                        PdfPTable table = new PdfPTable(targetDataGrid.Columns.Count)
                        {
                            WidthPercentage = 100
                        };

                        foreach (var column in targetDataGrid.Columns)
                        {
                            table.AddCell(new PdfPCell(new Phrase(column.Header.ToString(), semiheaderFont)) { Border = 0, HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        }

                        foreach (var item in targetDataGrid.ItemsSource)
                        {
                            foreach (var column in targetDataGrid.Columns)
                            {
                                var cellContent = column.GetCellContent(item);

                                string cellText = string.Empty;

                                if (cellContent is ContentPresenter contentPresenter)
                                {
                                    var textBlock = FindChild<TextBlock>(contentPresenter);

                                    if (textBlock != null)
                                    {
                                        cellText = textBlock.Text;
                                    }
                                }
                                else if (cellContent is TextBlock textBlock)
                                {
                                    cellText = textBlock.Text;
                                }
                                else
                                {
                                    cellText = cellContent?.ToString() ?? string.Empty;
                                }
                                table.AddCell(new PdfPCell(new Phrase(cellText, normalFont)) { Border = 0, HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                            }
                        }

                        doc.Add(table);

                        PdfPTable BottomBorder = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };
                        topBorderTable.DefaultCell.BorderWidthTop = 1.5f;
                        topBorderTable.DefaultCell.Border = Rectangle.TOP_BORDER; 
                        topBorderTable.AddCell(""); 
                        doc.Add(topBorderTable);

                        doc.Close();

                        string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
                        File.WriteAllBytes(tempFilePath, memoryStream.ToArray());

                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFilePath) { UseShellExecute = true });

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
                return context.tbl_company.FirstOrDefault();
            }
        }

        private T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
                return null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    return (T)child;

                var result = FindChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = TextBox.Text.ToLower();

            if (categoryComboBox.SelectedItem is string selectedCategory)
            {
                switch (selectedCategory)
                {
                    case "Inventory Sales Report":
                        filteredInventorySalesReports = allInventorySalesReports
                            .Where(report => report.ProductName.ToLower().Contains(searchQuery) ||
                                             report.TransactionID.ToString().Contains(searchQuery) ||
                                             report.Quantity.ToString().Contains(searchQuery) ||
                                             report.TotalPrice.ToString().Contains(searchQuery))
                            .ToList();
                            inventorySalesTotalPages = (int)Math.Ceiling((double)filteredInventorySalesReports.Count / itemsPerPage);
                            LoadPage("InventorySales", inventorySalesCurrentPage);
                            break;

                    case "Stock Check Sheets":
                        filteredStockCheckReports = allStockCheckReports
                            .Where(stock => stock.ProductName.ToLower().Contains(searchQuery) ||
                                            stock.Stocks.ToString().Contains(searchQuery))
                            .ToList();
                            stockCheckTotalPages = (int)Math.Ceiling((double)filteredStockCheckReports.Count / itemsPerPage);
                            LoadPage("StockCheck", stockCheckCurrentPage);
                            break;

                    case "Sales by Product":
                        filteredSalesByProductReports = allSalesByProductReports
                            .Where(sales => sales.ProductName.ToLower().Contains(searchQuery) ||
                                            sales.TotalQuantitySold.ToString().Contains(searchQuery) ||
                                            sales.TotalSales.ToString().Contains(searchQuery))
                            .ToList();
                            salesByProductTotalPages = (int)Math.Ceiling((double)filteredSalesByProductReports.Count / itemsPerPage);
                            LoadPage("SalesByProduct", salesByProductCurrentPage);
                            break;
                }
            }
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBox.Text == "Search here")
            {
                TextBox.Text = string.Empty;
                TextBox.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox.Text))
            {
                TextBox.Text = "Search here";
                TextBox.Foreground = Brushes.Gray; 
            }
        }
    }
}
