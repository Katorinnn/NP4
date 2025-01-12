﻿using LPG_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace LPG_Management_System.View.UserControls
{
    /// <summary>
    /// Interaction logic for customersUC.xaml
    /// </summary>
    public partial class customersUC : UserControl
    {
        public customersUC()
        {
            InitializeComponent();
            LoadCustomersData();
            //PopulateFilterMenu();
        }

        private void LoadCustomersData()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var customers = context.tbl_customers.ToList(); // Fetch all customers

                    // Bind data to DataGrid
                    customersDG.ItemsSource = customers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            customerCRUD cg = new customerCRUD();
            bool? dialogResult = cg.ShowDialog();

            // Check if the dialog was successful (e.g., a new record was added)
            if (dialogResult == true) // Assuming `true` is returned when a record is added
            {
                // Refresh the data in the DataGrid
                LoadCustomersData();
            }
        }

        //SearchBar

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = (sender as TextBox)?.Text;

            if (!string.IsNullOrEmpty(searchText))
            {
                try
                {
                    using (var context = new DataContext())
                    {
                        var filteredCustomers = context.tbl_customers
                            .Where(c => c.CustomerName.Contains(searchText) || c.ContactNumber.Contains(searchText))
                            .ToList();

                        customersDG.ItemsSource = filteredCustomers;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching data: " + ex.Message);
                }
            }
            else
            {
                // Reload all data if search is empty
                LoadCustomersData();
            }
            

            
        }


        private void customersDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //edit and delte 
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                // Get the tankID from the button's Tag
                int selectedcustomersID = Convert.ToInt32(btn.Tag);

                // Open the inventoryCRUD form and pass the correct tankID
                customerUpdate update = new customerUpdate(selectedcustomersID);
                update.ShowDialog(); // Show the dialog for editing
            }
        }

        // Delete button click event handler
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int id = Convert.ToInt32(btn.Tag);  // Assuming the ID is bound to the Tag

            // Confirm deletion
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new DataContext())
                    {
                        var customer = context.tbl_customers.FirstOrDefault(c => c.CustomerID == id);
                        if (customer != null)
                        {
                            context.tbl_customers.Remove(customer); // Remove the customer
                            context.SaveChanges(); // Save changes to database

                            // Reload the DataGrid
                            LoadCustomersData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting item: " + ex.Message);
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBox.Text == "Search here")
            {
                TextBox.Text = string.Empty;
                TextBox.Foreground = Brushes.Black; // Set text color to normal
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox.Text))
            {
                TextBox.Text = "Search here";
                TextBox.Foreground = Brushes.Gray; // Set text color to placeholder style
            }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}