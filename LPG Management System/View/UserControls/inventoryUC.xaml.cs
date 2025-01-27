using LPG_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LPG_Management_System.View.UserControls
{
    public partial class inventoryUC : UserControl
    {
        private readonly DataContext _context;

        private ContextMenu FilterContextMenu = new ContextMenu();
        public inventoryUC()
        {
            InitializeComponent();
            _context = new DataContext();
            LoadCustomersData();
            LoadListViewData();
            LoadGridViewData();


        }

        private void LoadCustomersData()
        {
            try
            {
                var inventoryData = _context.tbl_inventory.ToList();  // Correct DbSet reference
                inventoryDG.ItemsSource = inventoryData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = (sender as TextBox)?.Text;

            if (!string.IsNullOrEmpty(searchText))
            {
                try
                {
                    var filteredData = _context.tbl_inventory
                        .Where(i => i.StocksID.ToString().Contains(searchText) || i.ProductName.Contains(searchText))
                        .ToList();
                    inventoryDG.ItemsSource = filteredData;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching data: " + ex.Message);
                }
            }
            else
            {
                LoadCustomersData();
            }
        }

        

        private void ApplyFilter(string column, string value)
        {
            try
            {
                var filteredData = column switch
                {
                    "BrandName" => _context.tbl_inventory.Where(i => i.ProductName == value).ToList(),
                    "Size" => _context.tbl_inventory.Where(i => i.Size == value).ToList(),
                    _ => throw new ArgumentException("Invalid filter column")
                };
                inventoryDG.ItemsSource = filteredData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying filter: " + ex.Message);
            }
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            inventoryCRUD inventoryCRUD = new inventoryCRUD(); // No parameter needed
            bool? dialogResult = inventoryCRUD.ShowDialog();

            if (dialogResult == true)
            {
                LoadCustomersData();
                
            }
        }


        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int selectedStockID = Convert.ToInt32(btn.Tag);
                inventoryUpdate inventoryUpdate = new inventoryUpdate(selectedStockID, inventoryDG);
                inventoryUpdate.ShowDialog();
                LoadCustomersData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int id = Convert.ToInt32(btn.Tag);

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var itemToDelete = _context.tbl_inventory.FirstOrDefault(i => i.StocksID == id);
                    if (itemToDelete != null)
                    {
                        _context.tbl_inventory.Remove(itemToDelete);
                        _context.SaveChanges();
                        LoadCustomersData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting item: " + ex.Message);
                }
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCustomersData();
        }


        public class InventoryTable
        {
            public int TankID { get; set; }
            public string ProductName { get; set; }
            public string Size { get; set; }
            public decimal Price { get; set; }
            public byte[] ProductImage { get; set; }
            public int Stock { get; set; } // New stock property
        }

        private void OpenInventoryUpdate(int stocksID)
        {
            // Pass the correct DataGrid reference (inventoryDG) to inventoryUpdate
            inventoryUpdate inventoryUpdateWindow = new inventoryUpdate(stocksID, inventoryDG);
            inventoryUpdateWindow.ShowDialog();
        }

        private void ListViewButton_Checked(object sender, RoutedEventArgs e)
        {
            // Show the DataGrid and hide the Product Grid
            inventoryDG.Visibility = Visibility.Visible;
            productView.Visibility = Visibility.Collapsed;

            // Ensure GridViewButton is unchecked when ListViewButton is checked
            gridViewButton.IsChecked = false;
        }

        private void GridViewButton_Checked(object sender, RoutedEventArgs e)
        {
            // Show the Product Grid and hide the DataGrid
            inventoryDG.Visibility = Visibility.Collapsed;
            productView.Visibility = Visibility.Visible;

            // Ensure ListViewButton is unchecked when GridViewButton is checked
            listViewButton.IsChecked = false;
        }

        private void ListViewButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // Code to handle when the ListView button is unchecked
            inventoryDG.Visibility = Visibility.Collapsed;
            productView.Visibility = Visibility.Collapsed;
        }

        private void GridViewButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // Code to handle when the GridView button is unchecked
            inventoryDG.Visibility = Visibility.Visible;
            productView.Visibility = Visibility.Collapsed;
        }



        private void LoadListViewData()
        {
            try
            {
                var inventoryData = _context.tbl_inventory.ToList();  // Correct DbSet reference
                inventoryDG.ItemsSource = inventoryData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
        }

        private void LoadGridViewData()
        {
            try
            {
                // Clear existing items in the WrapPanel
                productView.Children.Clear();

                // Fetch products from the database
                var products = _context.tbl_inventory.ToList();

                // Dynamically create Product user controls and add them to the WrapPanel
                foreach (var product in products)
                {
                    // Create a new instance of the Product user control
                    var productControl = new Product
                    {
                        BrandLabel = { Content = product.ProductName },
                        PriceLabel = { Content = $"₱{product.Price:F2}" },
                        SizeLabel = { Content = product.Size }
                    };

                    // Set product image if available
                    if (product.ProductImage?.Length > 0)
                    {
                        productControl.ProductImage.Source = ConvertToImageSource(product.ProductImage);
                    }

                    // Add a click event to handle actions (e.g., editing or selecting the product)
                    productControl.MouseLeftButtonUp += (sender, e) =>
                    {
                        // Handle product click
                        OpenInventoryUpdate(product.StocksID);
                    };

                    // Add the control to the WrapPanel
                    productView.Children.Add(productControl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading grid view data: " + ex.Message);
            }
        }

        private System.Windows.Media.ImageSource ConvertToImageSource(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0) return null;

            try
            {
                using var ms = new MemoryStream(imageBytes);
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error converting image: " + ex.Message);
                return null;
            }
        }






    }
}
