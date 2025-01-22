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
                    // Fetch all reports from the tbl_reports table and order by Date descending
                    allReports = dbContext.tbl_reports
                        .OrderByDescending(report => report.Date)
                        .ToList();

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

        private void FilterReportsByDate()
        {
            if (beginDatePicker.SelectedDate.HasValue && endDatePicker.SelectedDate.HasValue)
            {
                DateTime startDate = beginDatePicker.SelectedDate.Value.Date;
                DateTime endDate = endDatePicker.SelectedDate.Value.Date.AddDays(1).AddMilliseconds(-1); // Set to 23:59:59.999

                if (startDate > endDate)
                {
                    MessageBox.Show("Begin Date cannot be later than End Date.");
                    return;
                }

                allReports = allReports
                    .Where(report => report.Date >= startDate && report.Date <= endDate)
                    .ToList();

                if (allReports.Count == 0)
                {
                    MessageBox.Show("No records found for the selected date range.");
                }

                currentPage = 1;
                totalPages = (int)Math.Ceiling((double)allReports.Count / itemsPerPage);
                LoadPage(currentPage);
            }
            else
            {
                MessageBox.Show("Please select both Begin Date and End Date.");
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
            LoadReportsData();
        }





        private void PrintPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a FixedDocument to hold the preview content
                FixedDocument fixedDocument = new FixedDocument();

                // Define a page size
                Size pageSize = new Size(800, 1120); // A4 size (8.3 x 11.7 inches in pixels at 96 DPI)
                fixedDocument.DocumentPaginator.PageSize = pageSize;

                // Create a FixedPage for each page of content
                FixedPage fixedPage = new FixedPage
                {
                    Width = pageSize.Width,
                    Height = pageSize.Height
                };

                // Add a title
                TextBlock title = new TextBlock
                {
                    Text = "Reports Preview",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 20, 50, 20)
                };
                FixedPage.SetLeft(title, (pageSize.Width - title.ActualWidth) / 2);
                fixedPage.Children.Add(title);

                // Add a DataGrid-like table representation
                var grid = new Grid();
                grid.Margin = new Thickness(100);

                // Define columns
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Add header row
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.Children.Add(CreateHeaderCell("Transaction ID", 0));
                grid.Children.Add(CreateHeaderCell("Tank ID", 1));
                grid.Children.Add(CreateHeaderCell("Product Name", 2));
                grid.Children.Add(CreateHeaderCell("Date", 3));

                // Add data rows
                int rowIndex = 1;
                foreach (ReportsTable report in reportsDG.Items)
                {
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    grid.Children.Add(CreateDataCell(report.TransactionID.ToString(), rowIndex, 0));
                    grid.Children.Add(CreateDataCell(report.TankID.ToString(), rowIndex, 1));
                    grid.Children.Add(CreateDataCell(report.ProductName, rowIndex, 2));
                    grid.Children.Add(CreateDataCell(report.Date.ToString("yyyy-MM-dd"), rowIndex, 3));
                    rowIndex++;
                }

                fixedPage.Children.Add(grid);

                // Add the page to the FixedDocument
                PageContent pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fixedPage);
                fixedDocument.Pages.Add(pageContent);

                // Open the PrintPreview window with the generated document
                PrintPreview previewWindow = new PrintPreview(fixedDocument);
                previewWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating print preview: " + ex.Message);
            }
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


        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterReportsByDate();
        }

    }
}


