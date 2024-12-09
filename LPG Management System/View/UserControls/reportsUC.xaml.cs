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
            LoadReportsData(); // Update the method name to reflect reports
        }

        private void LoadReportsData()
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    // Fetching all reports from the tbl_reports table
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

        private void reportsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection changes if necessary
        }
    }
}
