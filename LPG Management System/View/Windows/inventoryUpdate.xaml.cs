using LPG_Management_System.Models;
using System;
using System.Linq;
using System.Windows;

namespace LPG_Management_System.View
{
    public partial class inventoryUpdate : Window
    {
        public int TankId { get; set; }

        public inventoryUpdate(int tankID)
        {
            InitializeComponent();
            TankId = tankID;
            LoadItemData(tankID);
        }

        private void LoadItemData(int tankId)
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    var inventoryItem = dbContext.tbl_inventory.FirstOrDefault(i => i.TankID == tankId);
                    if (inventoryItem != null)
                    {
                        // Populate the controls (e.g., TextBoxes) with the data from the database
                        tankIDtxtBox.Text = inventoryItem.TankID.ToString();
                        brandtxtBox.Text = inventoryItem.ProductName;
                        sizetxtBox.Text = inventoryItem.Size;
                        pricetxtBox.Text = inventoryItem.Price.ToString("F2");
                    }
                    else
                    {
                        MessageBox.Show("Item not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading item data: " + ex.Message);
            }
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            string tankID = tankIDtxtBox.Text;
            string brandname = brandtxtBox.Text;
            string size = sizetxtBox.Text;
            string price = pricetxtBox.Text;

            if (string.IsNullOrEmpty(tankID) || string.IsNullOrEmpty(brandname) || string.IsNullOrEmpty(size) || string.IsNullOrEmpty(price))
            {
                MessageBox.Show("Please fill in all the fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var dbContext = new DataContext())
                {
                    var inventoryItem = dbContext.tbl_inventory.FirstOrDefault(i => i.TankID == int.Parse(tankID));
                    if (inventoryItem != null)
                    {
                        inventoryItem.ProductName = brandname;
                        inventoryItem.Size = size;
                        inventoryItem.Price = decimal.Parse(price);

                        dbContext.SaveChanges();  // Save the changes to the database

                        MessageBox.Show("Inventory updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Item not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
