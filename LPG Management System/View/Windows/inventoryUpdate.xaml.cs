using LPG_Management_System.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LPG_Management_System.View
{
    public partial class inventoryUpdate : Window
    {
        public event Action OnInventoryUpdated;
        public int StocksID { get; set; }
        private DataGrid inventoryDataGrid; // Field to store the DataGrid reference


        public inventoryUpdate(int StocksID, DataGrid inventoryDataGrid)
        {
            InitializeComponent();
            this.StocksID = StocksID;
            this.inventoryDataGrid = inventoryDataGrid; // Assigning the DataGrid reference
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

                        // Split size if it includes the unit (e.g., "2 kg")
                        var sizeParts = inventoryItem.Size.Split(' ');
                        if (sizeParts.Length == 2)
                        {
                            sizetxtBox.Text = sizeParts[0]; // Size value
                            sizeUnitComboBox.SelectedItem = sizeUnitComboBox.Items.Cast<ComboBoxItem>()
                                .FirstOrDefault(item => item.Content.ToString() == sizeParts[1]); // Select the unit
                        }

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
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string imagePath = openFileDialog.FileName;
                    var bitmap = new BitmapImage(new Uri(imagePath));
                    productImagePreview.Source = bitmap;

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
            string sizeUnit = (sizeUnitComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(); // Get selected unit

            int parsedStocksID;
            if (!int.TryParse(stocksID, out parsedStocksID))
            {
                MessageBox.Show("Invalid Stocks ID.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(brandname) || string.IsNullOrEmpty(size) || string.IsNullOrEmpty(price) || string.IsNullOrEmpty(sizeUnit))
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
                        inventoryItem.Size = size + " " + sizeUnit; 
                        inventoryItem.Price = parsedPrice;
                        inventoryItem.Stocks = parsedStocks;
                        inventoryItem.Date = DateTime.Now;

                        // Save the image if it's updated
                        if (this.imageBytes != null && this.imageBytes.Length > 0)
                        {
                            inventoryItem.ProductImage = this.imageBytes;
                        }

                        dbContext.SaveChanges();  // Save the changes to the database

                        MessageBox.Show("Inventory updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Trigger the event to refresh the DataGrid in the parent
                        OnInventoryUpdated?.Invoke();

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
                currentStock++;
                stockstxtBox.Text = currentStock.ToString();
            }
            else
            {
                MessageBox.Show("Invalid stock value.");
            }
        }
        private void RefreshDataGrid()
        {
            // Refresh the DataGrid by re-binding it to the updated data from the database
            using (var dbContext = new DataContext())
            {
                var updatedInventoryList = dbContext.tbl_inventory.ToList();  // Fetch updated data
                inventoryDataGrid.ItemsSource = updatedInventoryList;  // Re-bind the DataGrid
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void stockstxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
