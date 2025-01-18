using LPG_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LPG_Management_System.View.UserControls
{
    public partial class reportsUC : UserControl
    {
        public reportsUC()
        {
            InitializeComponent();
            LoadReportsData();
            InitializeCategoryComboBox(); // Populate the ComboBox
        }

        private void LoadReportsData()
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    // Fetching all reports from the tbl_reports table by default
                    var reports = dbContext.tbl_reports.ToList();

                    // Bind the data to the DataGrid
                    reportsDG.ItemsSource = reports;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message);
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

        private void reportsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection changes if necessary
        }
    }
}
