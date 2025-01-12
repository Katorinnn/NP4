using LPG_Management_System.Models;
using LPG_Management_System.View.UserControls;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LPG_Management_System.View
{
    public partial class inventoryCRUD : Window
    {
        public int TankId { get; set; }
        private byte[] selectedImageBytes;

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
            string tankIDText = tankIDtxtBox.Text;
            string brandname = brandtxtBox.Text;
            string size = sizetxtBox.Text;
            string priceText = pricetxtBox.Text;

            if (string.IsNullOrEmpty(tankIDText) || string.IsNullOrEmpty(brandname) ||
                string.IsNullOrEmpty(size) || string.IsNullOrEmpty(priceText) || selectedImageBytes == null)
            {
                MessageBox.Show("Please fill in all the fields and select an image.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(tankIDText, out int tankID) || !decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Tank ID and Price must be valid numbers.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new DataContext())
                {
                    var inventory = new InventoryTable
                    {
                        TankID = tankID,
                        ProductName = brandname,
                        Size = size,
                        Price = price,
                        ProductImage = selectedImageBytes // Save the image as a BLOB
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


        //private void tankIDtxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        //{

        //}
    }
}
