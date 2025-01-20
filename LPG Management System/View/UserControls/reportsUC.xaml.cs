using LPG_Management_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


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
                // Ensure only the date part is displayed (without time)
                beginDatePicker.SelectedDate = beginDatePicker.SelectedDate.Value.Date;
            }
        }

        private void endDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (endDatePicker.SelectedDate.HasValue)
            {
                // Ensure only the date part is displayed (without time)
                endDatePicker.SelectedDate = endDatePicker.SelectedDate.Value.Date;
            }
        }

        private void reportsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection changes if necessary
        }
    }
}


