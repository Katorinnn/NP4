using LPG_Management_System.Models;
using LPG_Management_System.View.UserControls;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            datePicker.SelectedDate = DateTime.Today; // Set today's date
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            string brandname = brandtxtBox.Text;
            string size = sizetxtBox.Text;
            string priceText = pricetxtBox.Text;
            string stocksText = stockstxtBox.Text;
            var selectedDate = datePicker.SelectedDate;

            if (string.IsNullOrEmpty(brandname) || string.IsNullOrEmpty(size) ||
                string.IsNullOrEmpty(priceText) || string.IsNullOrEmpty(stocksText) ||
                selectedDate == null || selectedImageBytes == null)
            {
                MessageBox.Show("Please fill in all fields and select an image.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price) || !int.TryParse(stocksText, out int stocks))
            {
                MessageBox.Show("Invalid number format for Price or Stocks.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new DataContext())
                {
                    var inventory = new InventoryTable
                    {
                        ProductName = brandname,
                        Size = size,
                        Price = price,
                        Stocks = stocks,
                        Date = selectedDate.Value, // Ensure this matches the database type
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
    }
}
