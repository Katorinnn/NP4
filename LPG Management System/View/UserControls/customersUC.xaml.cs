using LPG_Management_System.Models;
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
            customersDG.CanUserReorderColumns = false;
            //PopulateFilterMenu();
        }



        private void LoadCustomersData()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var customers = context.tbl_customers.ToList(); // Fetch all customers

                    // Check if the DataGrid is null
                    if (customersDG != null)
                    {
                        customersDG.ItemsSource = customers; // Bind data to DataGrid
                    }
                    else
                    {
                        Console.WriteLine("customersDG is null.");
                    }
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
            // Ensure sender is a TextBox and extract the text
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string searchText = textBox.Text.Trim().ToLower(); // Trim and convert to lowercase

            if (!string.IsNullOrEmpty(searchText) && searchText != "search here")
            {
                try
                {
                    using (var context = new DataContext())
                    {
                        var filteredCustomers = context.tbl_customers
                    .Where(c =>
                            c.CustomerName.ToLower().Contains(searchText) ||
                            c.ContactNumber.ToLower().Contains(searchText) ||
                            c.Address.ToLower().Contains(searchText) ||
                            c.TankClassification.ToLower().Contains(searchText))
                        .ToList();

                        customersDG.ItemsSource = filteredCustomers; // Bind the filtered data to DataGrid
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching data: " + ex.Message);
                }
            }
            else
            {
                // Reload all data if search is empty or contains default placeholder text
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

        // Event handler for button click
        //private void ShowFilterButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // Toggle the visibility of the ComboBox
        //    if (FilterComboBox.Visibility == Visibility.Collapsed)
        //    {
        //        FilterComboBox.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        FilterComboBox.Visibility = Visibility.Collapsed;
        //    }
        //}

        //// Event handler for ComboBox selection change
        //private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    string selectedFilter = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

        //    if (!string.IsNullOrEmpty(selectedFilter))
        //    {
        //        ApplyFilter(selectedFilter);
        //    }
        //}

        //// Apply filter based on selected category
        //private void ApplyFilter(string column)
        //{
        //    string searchText = TextBox.Text;

        //    if (!string.IsNullOrEmpty(searchText) && searchText != "Search here")
        //    {
        //        try
        //        {
        //            using (var context = new DataContext())
        //            {
        //                var filteredCustomers = context.tbl_customers.AsQueryable();

        //                switch (column)
        //                {
        //                    case "Customer Name":
        //                        filteredCustomers = filteredCustomers.Where(c => c.CustomerName.Contains(searchText));
        //                        break;
        //                    case "Contact Number":
        //                        filteredCustomers = filteredCustomers.Where(c => c.ContactNumber.Contains(searchText));
        //                        break;
        //                    case "Address":
        //                        filteredCustomers = filteredCustomers.Where(c => c.Address.Contains(searchText));
        //                        break;
        //                    case "Tank Classification":
        //                        filteredCustomers = filteredCustomers.Where(c => c.TankClassification.Contains(searchText));
        //                        break;
        //                    default:
        //                        break;
        //                }

        //                customersDG.ItemsSource = filteredCustomers.ToList();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error filtering data: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please enter a valid search term.");
        //    }
        //}

    }
}