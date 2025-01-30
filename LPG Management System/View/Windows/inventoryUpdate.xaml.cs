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
        private DataGrid inventoryDataGrid; 


        public inventoryUpdate(int StocksID, DataGrid inventoryDataGrid)
        {
            InitializeComponent();
            this.StocksID = StocksID;
            this.inventoryDataGrid = inventoryDataGrid; 
            LoadItemData(StocksID);
        }


        private byte[] imageBytes; 
        private void LoadItemData(int stocksId)
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    var inventoryItem = dbContext.tbl_inventory.FirstOrDefault(i => i.StocksID == stocksId);
                    if (inventoryItem != null)
                    {
                        stockIDtxtBox.Text = inventoryItem.StocksID.ToString();
                        brandtxtBox.Text = inventoryItem.ProductName;
                        sizetxtBox.Text = inventoryItem.Size;

                        var sizeParts = inventoryItem.Size.Split(' ');
                        if (sizeParts.Length == 2)
                        {
                            sizetxtBox.Text = sizeParts[0]; 
                            sizeUnitComboBox.SelectedItem = sizeUnitComboBox.Items.Cast<ComboBoxItem>()
                                .FirstOrDefault(item => item.Content.ToString() == sizeParts[1]); 
                        }

                        stockstxtBox.Text = inventoryItem.Stocks.ToString();
                        pricetxtBox.Text = inventoryItem.Price.ToString("F2");

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

                        this.imageBytes = newImageBytes;
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
            string stock = stockstxtBox.Text; 
            string size = sizetxtBox.Text;
            string price = pricetxtBox.Text;
            string sizeUnit = (sizeUnitComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

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

                        if (this.imageBytes != null && this.imageBytes.Length > 0)
                        {
                            inventoryItem.ProductImage = this.imageBytes;
                        }

                        dbContext.SaveChanges();  

                        MessageBox.Show("Inventory updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

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
            if (int.TryParse(stockstxtBox.Text, out int currentStock))
            {
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
        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
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
            using (var dbContext = new DataContext())
            {
                var updatedInventoryList = dbContext.tbl_inventory.ToList(); 
                inventoryDataGrid.ItemsSource = updatedInventoryList;  
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
