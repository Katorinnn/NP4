using LPG_Management_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;


namespace LPG_Management_System.View.UserControls
{
    public partial class reportsUC : UserControl
    {
        private int currentPage = 1;
        private const int itemsPerPage = 20;
        private int totalPages;
        private List<ReportsTable> allReports;

        public reportsUC()
        {
            InitializeComponent();
            LoadReportsData();
            InitializeCategoryComboBox();

            // Set the MinDate for the DatePickers to prevent future dates
            beginDatePicker.DisplayDateEnd = DateTime.Today;
            endDatePicker.DisplayDateEnd = DateTime.Today;
        }

        private void LoadReportsData()
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    // Fetch all reports from the tbl_reports table
                    allReports = dbContext.tbl_reports.ToList();

                    // Calculate the total number of pages
                    totalPages = (int)Math.Ceiling((double)allReports.Count / itemsPerPage);

                    // Load the first page of reports
                    LoadPage(currentPage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
        }
        private void LoadPage(int page)
        {
            // Calculate the starting index for the page
            int startIndex = (page - 1) * itemsPerPage;

            // Get the data for the current page
            var pageData = allReports.Skip(startIndex).Take(itemsPerPage).ToList();

            // Bind the data to the DataGrid
            reportsDG.ItemsSource = pageData;

            // Update the page indicator
            pageIndicator.Text = $"Page {currentPage} of {totalPages}";
        }


        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadPage(currentPage);
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadPage(currentPage);
            }
        }

        private void InitializeCategoryComboBox()
        {
            try
            {
                // Define the report categories
                var categories = new List<string>
                {
                    "Inventory Sales Report",
                    "Stock Check Sheets"
                };

                // Bind the categories to the ComboBox
                categoryComboBox.ItemsSource = categories;
                categoryComboBox.SelectedIndex = 0; // Optionally set a default selection
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing category combo box: " + ex.Message);
            }
        }

        private void categoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoryComboBox.SelectedItem is string selectedCategory)
            {
                try
                {
                    // Check the selected category and load corresponding data
                    if (selectedCategory == "Inventory Sales Report")
                    {
                        LoadReportsData();
                    }
                    else if (selectedCategory == "Stock Check Sheets")
                    {
                        LoadStockData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error handling category selection: " + ex.Message);
                }
            }
        }

        private void LoadStockData()
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    // Fetching all stock data from the tbl_stocks table
                    var stocks = dbContext.tbl_stocks.ToList();

                    // Bind the data to the DataGrid
                    reportsDG.ItemsSource = stocks;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching stock data: " + ex.Message);
            }
        }

        private void beginDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (beginDatePicker.SelectedDate.HasValue)
            {
                // Optionally display a formatted date (dd/MM/yyyy)
                beginDatePicker.Text = beginDatePicker.SelectedDate.Value.ToString("yyyy/mm/dd");
            }
        }

        private void endDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (endDatePicker.SelectedDate.HasValue)
            {
                // Optionally display a formatted date (dd/MM/yyyy)
                endDatePicker.Text = endDatePicker.SelectedDate.Value.ToString("yyyy/mm/dd");
            }
        }


        private void reportsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection changes if necessary
        }

        private void FilterReportsByDate()
        {
            if (beginDatePicker.SelectedDate.HasValue && endDatePicker.SelectedDate.HasValue)
            {
                // Parse the selected dates
                DateTime startDate = beginDatePicker.SelectedDate.Value.Date;
                DateTime endDate = endDatePicker.SelectedDate.Value.Date;

                // Ensure start date is not greater than end date
                if (startDate > endDate)
                {
                    MessageBox.Show("Begin Date cannot be later than End Date.");
                    return;
                }

                // Filter the reports
                var filteredReports = allReports
                    .Where(report => report.Date.Date >= startDate && report.Date.Date <= endDate)
                    .ToList();

                // Handle no records case
                if (filteredReports.Count == 0)
                {
                    MessageBox.Show("No records found for the selected date range.");
                }

                // Update the data grid
                reportsDG.ItemsSource = filteredReports;

                // Update pagination
                totalPages = (int)Math.Ceiling((double)filteredReports.Count / itemsPerPage);
                currentPage = 1; // Reset to first page
                LoadPage(currentPage);
            }
            else
            {
                MessageBox.Show("Please select both Begin Date and End Date.");
            }
        }



        private void PrintPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    FlowDocument document = new FlowDocument
                    {
                        FontSize = 12,
                        FontFamily = new FontFamily("Arial")
                    };

                    // Create the header
                    document.Blocks.Add(new Paragraph(new Run("Reports Preview"))
                    {
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center
                    });

                    // Add a table to display the data
                    Table table = new Table
                    {
                        CellSpacing = 0,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };

                    // Define columns
                    table.Columns.Add(new TableColumn() { Width = new GridLength(100) });
                    table.Columns.Add(new TableColumn() { Width = new GridLength(140) });
                    table.Columns.Add(new TableColumn() { Width = new GridLength(140) });
                    table.Columns.Add(new TableColumn() { Width = new GridLength(100) });

                    // Add header row
                    var headerRow = new TableRow();
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Transaction ID"))) { FontWeight = FontWeights.Bold });
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Tank ID"))) { FontWeight = FontWeights.Bold });
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Product Name"))) { FontWeight = FontWeights.Bold });
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))) { FontWeight = FontWeights.Bold });

                    var headerRowGroup = new TableRowGroup();
                    headerRowGroup.Rows.Add(headerRow);
                    table.RowGroups.Add(headerRowGroup);

                    // Add data rows
                    foreach (ReportsTable report in reportsDG.Items)
                    {
                        var dataRow = new TableRow();
                        dataRow.Cells.Add(new TableCell(new Paragraph(new Run(report.TransactionID.ToString()))));
                        dataRow.Cells.Add(new TableCell(new Paragraph(new Run(report.TankID.ToString()))));
                        dataRow.Cells.Add(new TableCell(new Paragraph(new Run(report.ProductName))));
                        dataRow.Cells.Add(new TableCell(new Paragraph(new Run(report.Date.ToString("yyyy-MM-dd")))));
                        headerRowGroup.Rows.Add(dataRow);
                    }

                    document.Blocks.Add(table);

                    // Print the document
                    printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "Reports Preview");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating print preview: " + ex.Message);
            }
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterReportsByDate();
        }

    }
}


