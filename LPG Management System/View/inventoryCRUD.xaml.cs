using LPG_Management_System.Models;
using LPG_Management_System.View.UserControls;
using System;
using System.Windows;

namespace LPG_Management_System.View
{
    public partial class inventoryCRUD : Window
    {
        public int TankId { get; set; }

        public inventoryCRUD(int tankID)
        {
            InitializeComponent();
            TankId = tankID;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (TankId > 0)
            {
                // Example: Display the TankId or prefill data based on it
                MessageBox.Show($"Editing details for Tank ID: {TankId}");
                // LoadItemData(TankId); // Use the TankId to load data if needed
            }
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            // Collect user input
            string tankIDText = tankIDtxtBox.Text;
            string brandname = brandtxtBox.Text;
            string size = sizetxtBox.Text;
            string priceText = pricetxtBox.Text;  // Capture the price as a string

            // Validate input
            if (string.IsNullOrEmpty(tankIDText) || string.IsNullOrEmpty(brandname) || string.IsNullOrEmpty(size) || string.IsNullOrEmpty(priceText))
            {
                MessageBox.Show("Please fill in all the fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Exit the method early
            }

            // Validate and parse tankID as an integer
            if (!int.TryParse(tankIDText, out int tankID))
            {
                MessageBox.Show("Tank ID must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate and parse price as a decimal
            if (!decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Price must be a valid decimal number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new DataContext())
                {
                    // Create a new inventory record
                    var inventory = new InventoryTable
                    {
                        TankID = tankID,
                        ProductName = brandname,
                        Size = size,
                        Price = price  // Use the decimal value for price
                    };

                    // Add and save the new inventory item
                    db.tbl_inventory.Add(inventory);
                    db.SaveChanges();

                    MessageBox.Show("Inventory item added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;

                    // Close the form
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save logic here
            DialogResult = true; // Indicate success and close the dialog
            Close();
        }

        //private void tankIDtxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        //{

        //}
    }
}
