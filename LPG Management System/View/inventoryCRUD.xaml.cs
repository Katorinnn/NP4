using LPG_Management_System.Models;
using LPG_Management_System.View.UserControls;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LPG_Management_System.View
{
    public partial class inventoryCRUD : Window
    {
        private byte[] selectedImageBytes;
        public inventoryCRUD()
        {
            InitializeComponent();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            string brandname = brandtxtBox.Text;
            string size = sizetxtBox.Text;
            string priceText = pricetxtBox.Text;
            string stocksText = stockstxtBox.Text;

            string unit = (unitComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            string fullSize = $"{size} {unit}";

            if (string.IsNullOrEmpty(brandname) || string.IsNullOrEmpty(size) ||
                string.IsNullOrEmpty(priceText) || string.IsNullOrEmpty(stocksText) ||
                selectedImageBytes == null)
            {
                MessageBox.Show("Please fill in all fields, including stocks, and select an image.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price) || price <= 0)
            {
                MessageBox.Show("Price must be a positive number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(stocksText, out int stocks) || stocks < 0)
            {
                MessageBox.Show("Stocks must be a non-negative whole number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new DataContext())
                {
                    // Check if a product with the same brand and size already exists
                    var existingProduct = db.tbl_inventory
                        .AsEnumerable() // Forces in-memory comparison
                        .FirstOrDefault(i => i.ProductName.Equals(brandname, StringComparison.Ordinal)
                                          && i.Size.Equals(fullSize, StringComparison.Ordinal));

                    if (existingProduct != null)
                    {
                        MessageBox.Show("A product with the exact same brand name and size already exists.",
                                        "Duplicate Product", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var inventory = new InventoryTable
                    {
                        ProductName = brandname,
                        Size = fullSize, // Save the combined size and unit
                        Price = price,
                        Stocks = stocks,
                        Date = DateTime.Now, // Ensure this matches the database type
                        ProductImage = selectedImageBytes
                    };

                    db.tbl_inventory.Add(inventory);
                    db.SaveChanges();

                    MessageBox.Show("Inventory item added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void ReduceQuantity_Click(object sender, RoutedEventArgs e)
        {
            // Parse the current stock value from the TextBox
            if (int.TryParse(stockstxtBox.Text, out int currentStock))
            {
                // Decrement the stock value, ensuring it doesn't go below 0
                if (currentStock > 0)
                {
                    currentStock--;
                    stockstxtBox.Text = currentStock.ToString();
                }
            }
            else
            {
                MessageBox.Show("Invalid stock value.");
            }
        }

        // Event handler for the increment button
        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            // Parse the current stock value from the TextBox
            if (int.TryParse(stockstxtBox.Text, out int currentStock))
            {
                // Increment the stock value
                currentStock++;
                stockstxtBox.Text = currentStock.ToString();
            }
            else
            {
                MessageBox.Show("Invalid stock value.");
            }
        }



        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save logic here
            DialogResult = true; // Indicate success and close the dialog
            Close();
        }

        private void imageSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            // Open a file dialog for selecting an image
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Load the selected image
                string selectedFileName = openFileDialog.FileName;
                selectedImageBytes = File.ReadAllBytes(selectedFileName);

                // Display the selected image in the preview
                productImagePreview.Source = new BitmapImage(new Uri(selectedFileName));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Brand TextBox - Only letters and spaces
        private void brandtxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only letters and spaces
            e.Handled = !Regex.IsMatch(e.Text, @"^[a-zA-Z\s]+$");
        }

        // Size TextBox - Only numbers and decimal points
        private void sizetxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numbers and decimal points
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]*\.?[0-9]*$");
        }

        // Price TextBox - Only numbers and decimal points
        private void pricetxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numbers and decimal points
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]*\.?[0-9]*$");
        }

        // Stocks TextBox - Only whole numbers
        private void stockstxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only whole numbers
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]+$");
        }

        private void sizetxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
