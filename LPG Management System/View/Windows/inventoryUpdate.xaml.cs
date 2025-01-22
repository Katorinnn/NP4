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

        private void LoadItemData(int stocksId)
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    var inventoryItem = dbContext.tbl_inventory.FirstOrDefault(i => i.StocksID == stocksId);
                    if (inventoryItem != null)
                    {
                        // Populate fields with data from database
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



        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            string stocksID = stockstxtBox.Text;
            string brandname = brandtxtBox.Text;
            string size = sizetxtBox.Text;
            string price = pricetxtBox.Text;

            if (string.IsNullOrEmpty(stocksID) || string.IsNullOrEmpty(brandname) || string.IsNullOrEmpty(size) || string.IsNullOrEmpty(price))
            {
                MessageBox.Show("Please fill in all the fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var dbContext = new DataContext())
                {
                    var inventoryItem = dbContext.tbl_inventory.FirstOrDefault(i => i.StocksID == int.Parse(stocksID));
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
