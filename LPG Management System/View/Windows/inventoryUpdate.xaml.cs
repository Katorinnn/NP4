using LPG_Management_System.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LPG_Management_System.View
{
    public partial class inventoryUpdate : Window
    {
        public int StocksID { get; set; }

        public inventoryUpdate(int StocksID)
        {
            InitializeComponent();
            this.StocksID = StocksID;
            LoadItemData(StocksID);
        }

        private byte[] imageBytes; // Class-level variable to hold image byte array

        private void LoadItemData(int stocksId)
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    var inventoryItem = dbContext.tbl_inventory.FirstOrDefault(i => i.StocksID == stocksId);
                    if (inventoryItem != null)
                    {
                        // Populate fields with data from the database
                        stockIDtxtBox.Text = inventoryItem.StocksID.ToString();
                        brandtxtBox.Text = inventoryItem.ProductName;
                        sizetxtBox.Text = inventoryItem.Size;
                        stockstxtBox.Text = inventoryItem.Stocks.ToString();
                        pricetxtBox.Text = inventoryItem.Price.ToString("F2");

                        // Set the image in the Image control
                        if (inventoryItem.ProductImage != null && inventoryItem.ProductImage.Length > 0)
                        {
                            using (var stream = new System.IO.MemoryStream(inventoryItem.ProductImage))
                            {
                                var bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = stream;
                                bitmap.EndInit();
                                productImagePreview.Source = bitmap;
                            }
                        }
                        else
                        {
                            // Clear image if no data is present
                            productImagePreview.Source = null;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Item not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading item data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void imageSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create an OpenFileDialog to select an image file
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"; // Filter for image files

            // Show the dialog and check if the user selected a file
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Load the selected image into the Image control
                    string imagePath = openFileDialog.FileName;
                    var bitmap = new BitmapImage(new Uri(imagePath));
                    productImagePreview.Source = bitmap;

                    // Load the image into a byte array
                    using (var fs = new System.IO.FileStream(imagePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        byte[] newImageBytes = new byte[fs.Length];
                        fs.Read(newImageBytes, 0, (int)fs.Length);

                        // Store the image bytes in a class-level variable
                        this.imageBytes = newImageBytes; // Use class-level imageBytes
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error selecting image: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            string stocksID = stockIDtxtBox.Text;
            string brandname = brandtxtBox.Text;
            string stock = stockstxtBox.Text; // Get stock value
            string size = sizetxtBox.Text;
            string price = pricetxtBox.Text;

            // Validate StocksID is an integer
            int parsedStocksID;
            if (!int.TryParse(stocksID, out parsedStocksID))
            {
                MessageBox.Show("Invalid Stocks ID.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(brandname) || string.IsNullOrEmpty(size) || string.IsNullOrEmpty(price))
            {
                MessageBox.Show("Please fill in all the fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int parsedStocks;
            if (!int.TryParse(stock, out parsedStocks))
            {
                MessageBox.Show("Invalid stock value.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal parsedPrice;
            if (!decimal.TryParse(price, out parsedPrice))
            {
                MessageBox.Show("Invalid price value.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var dbContext = new DataContext())
                {
                    var inventoryItem = dbContext.tbl_inventory.FirstOrDefault(i => i.StocksID == parsedStocksID);
                    if (inventoryItem != null)
                    {
                        inventoryItem.ProductName = brandname;
                        inventoryItem.Size = size;
                        inventoryItem.Price = parsedPrice;
                        inventoryItem.Stocks = parsedStocks; // Update the Stocks field

                        // Save the image if it's updated
                        if (this.imageBytes != null && this.imageBytes.Length > 0)
                        {
                            inventoryItem.ProductImage = this.imageBytes;
                        }

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




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



    }
}
