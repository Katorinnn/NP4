﻿using MySql.Data.MySqlClient;
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

        private readonly string connectionString = "server=localhost;database=db_lpgpos;user=root;";
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
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Query to fetch customer data
                    string query = "SELECT * FROM tbl_customers";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind data to DataGrid
                    customersDG.ItemsSource = dataTable.DefaultView;
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
            // Get the text from the search TextBox
            string searchText = (sender as TextBox)?.Text;

            if (!string.IsNullOrEmpty(searchText))
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        // Use a parameterized query to prevent SQL injection
                        string query = "SELECT * FROM tbl_customers WHERE customerName LIKE @SearchText OR contactNumber LIKE @SearchText";

                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Bind filtered data to DataGrid
                        customersDG.ItemsSource = dataTable.DefaultView;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching data: " + ex.Message);
                }
            }
            else
            {
                // If the search box is empty, reload all data
                LoadCustomersData();
            }
        }

        private void customersDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //filter
        private ContextMenu FilterContextMenu = new ContextMenu();
        //private void PopulateFilterMenu()
        //{
        //    // Clear existing filter menu items
        //    FilterContextMenu.Items.Clear();

        //    // Create "Filter by Name" MenuItem
        //    MenuItem filterByNameItem = new MenuItem { Header = "Date" };
        //    HashSet<string> addedBrands = new HashSet<string>();

        //    using (MySqlConnection connection = new MySqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        // Fetch unique brands
        //        string brandQuery = "SELECT DISTINCT date_purchased FROM tbl_customers";
        //        MySqlCommand brandCommand = new MySqlCommand(brandQuery, connection);
        //        using (MySqlDataReader brandReader = brandCommand.ExecuteReader())
        //        {
        //            while (brandReader.Read())
        //            {
        //                string brandName = brandReader["brandName"].ToString();
        //                if (!addedBrands.Contains(brandName))
        //                {
        //                    addedBrands.Add(brandName);

        //                    MenuItem brandItem = new MenuItem { Header = brandName };

        //                    // Apply filter on click
        //                    brandItem.Click += (sender, args) =>
        //                    {
        //                        ApplyFilter("brandName", brandName);
        //                    };
        //                    filterByNameItem.Items.Add(brandItem);
        //                }
        //            }
        //        }
        //    }

        //    // Add both filter options to the context menu
        //    FilterContextMenu.Items.Add(filterByNameItem);

        //    // Assign the context menu to the button
        //    FilterButton.ContextMenu = FilterContextMenu;
        //}

        private void ApplyFilter(string column, string value)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Use a parameterized query to filter data
                    string query = $"SELECT * FROM tbl_customer WHERE {column} = @Value";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Value", value);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind filtered data to the DataGrid
                    customersDG.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying filter: " + ex.Message);
            }
        }
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilterButton.ContextMenu != null)
            {
                FilterButton.ContextMenu.PlacementTarget = FilterButton;
                FilterButton.ContextMenu.IsOpen = true;
            }
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
                customerUpdate update= new customerUpdate(selectedcustomersID);
                update.ShowDialog(); // Show the dialog for editing
            }
        }

        // Delete button click event handler
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the ID of the selected item
            Button btn = sender as Button;
            int id = Convert.ToInt32(btn.Tag);  // Assuming the ID is bound to the Tag

            // Confirm deletion
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        // Delete query
                        string query = "DELETE FROM tbl_customers WHERE customerID = @Id";
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@Id", id);

                        cmd.ExecuteNonQuery();  // Execute the deletion

                        // Reload the DataGrid to reflect the changes
                        LoadCustomersData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting item: " + ex.Message);
                }
            }
        }
    }
}
