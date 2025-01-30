using LPG_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
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
            LoadInventoryData();
        }
        private void LoadInventoryData()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var customers = context.tbl_inventory.ToList(); 

                    if (inventoryDG != null)
                    {
                        inventoryDG.ItemsSource = customers; 
                    }
                    else
                    {
                        Console.WriteLine("customersDG is null.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string searchText = textBox.Text.Trim().ToLower(); 

            if (!string.IsNullOrEmpty(searchText) && searchText != "search here")
            {
                try
                {
                    using (var context = new DataContext())
                    {
                        var filteredInventory = context.tbl_inventory
                            .Where(i =>
                                i.ProductName.ToLower().Contains(searchText) ||
                                i.Size.ToLower().Contains(searchText) ||
                                i.Stocks.ToString().Contains(searchText) ||
                                i.Price.ToString().Contains(searchText) ||
                                i.Date.ToString().Contains(searchText) ||
                                i.StocksID.ToString().Contains(searchText))
                            .ToList();

                        inventoryDG.ItemsSource = filteredInventory; 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching data: " + ex.Message);
                }
            }
            else
            {
                LoadInventoryData(); 
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBox.Text == "Search here")
            {
                TextBox.Text = string.Empty;
                TextBox.Foreground = Brushes.Black; 
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox.Text))
            {
                TextBox.Text = "Search here";
                TextBox.Foreground = Brushes.Gray; 
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
            inventoryCRUD inventoryCRUD = new inventoryCRUD(); 
            bool? dialogResult = inventoryCRUD.ShowDialog();
            if (dialogResult == true)
            {
                LoadInventoryData();
            }

        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int selectedStockID = Convert.ToInt32(btn.Tag);
                OpenInventoryUpdateWindow(selectedStockID);  
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
                        LoadInventoryData();
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
            LoadInventoryData();
        }
        public class InventoryTable
        {
            public int TankID { get; set; }
            public string ProductName { get; set; }
            public string Size { get; set; }
            public decimal Price { get; set; }
            public byte[] ProductImage { get; set; }
            public int Stock { get; set; } 
        }

        private void OpenInventoryUpdateWindow(int stocksID)
        {
            inventoryUpdate updateWindow = new inventoryUpdate(stocksID, inventoryDG);
            updateWindow.OnInventoryUpdated += RefreshDataGrid;
            updateWindow.ShowDialog();
        }

        private void RefreshDataGrid()
        {
            using (var dbContext = new DataContext())
            {
                var updatedInventoryList = dbContext.tbl_inventory.ToList();  
                inventoryDG.ItemsSource = updatedInventoryList; 
            }
        }
        private void LoadProductsForDisplay(InventoryTable selectedItem)
        {
            ProductsWrapPanel.Children.Clear();
            var products = _context.tbl_inventory.ToList();

            if (selectedItem != null)
            {
                products = products.Where(p => p.ProductName == selectedItem.ProductName).ToList();
            }

            foreach (var product in products.GroupBy(p => new { p.ProductName, p.Size, p.Price }).Select(g => g.First()))
            {
                var productContainer = new Border
                {
                    Margin = new Thickness(10),
                    Background = Brushes.AliceBlue,
                    Width = 210,
                    Height = 270,
                    CornerRadius = new CornerRadius(10),
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    Effect = new DropShadowEffect
                    {
                        Color = Colors.Gray,
                        Direction = 315,
                        ShadowDepth = 1,
                        BlurRadius = 5,
                        Opacity = 0.5
                    }
                };
                productContainer.MouseEnter += (s, e) =>
                {
                    productContainer.RenderTransform = new ScaleTransform(1.05, 1.05); 
                    productContainer.RenderTransformOrigin = new Point(0.5, 0.5);    
                };

                productContainer.MouseLeave += (s, e) =>
                {
                    productContainer.RenderTransform = new ScaleTransform(1.0, 1.0); 
                };

                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical
                };

                var image = new Image
                {
                    Source = ConvertToImageSource(product.ProductImage),
                    Height = 120,
                    Margin = new Thickness(20)
                };

                var brandText = new TextBlock
                {
                    Text = $"Brand: {product.ProductName}",
                    FontWeight = FontWeights.SemiBold,
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 16,
                    Margin = new Thickness(15, 5, 5, 5)
                };

                var sizeText = new TextBlock
                {
                    Text = $"Size: {product.Size}",
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 13,
                    Margin = new Thickness(15, 5, 5, 5)
                };

                var priceText = new TextBlock
                {
                    Text = $"₱{product.Price:F2}",
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 13,
                    Margin = new Thickness(15, 5, 5, 5)
                };

                stackPanel.Children.Add(image);
                stackPanel.Children.Add(brandText);
                stackPanel.Children.Add(sizeText);
                stackPanel.Children.Add(priceText);
                productContainer.Child = stackPanel;

                ProductsWrapPanel.Children.Add(productContainer);
            }
        }
        private ImageSource ConvertToImageSource(byte[] imageBytes)
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
                MessageBox.Show($"Error loading image: {ex.Message}");
                return null;
            }
        }


        private void GridView_Click(object sender, RoutedEventArgs e)
        {
            inventoryDG.Visibility = Visibility.Collapsed;
            ProductScrollViewer.Visibility = Visibility.Visible;
            ProductsWrapPanel.Visibility = Visibility.Visible;
            ProductBorder.Visibility = Visibility.Visible;

            LoadProductsForDisplay(null);

            listViewButton.IsChecked = false;

            gridViewButton.IsChecked = true;
        }

        private void ListView_Click(object sender, RoutedEventArgs e)
        {
            inventoryDG.Visibility = Visibility.Visible;
            ProductsWrapPanel.Visibility = Visibility.Collapsed;
            ProductScrollViewer.Visibility = Visibility.Collapsed;
            ProductBorder.Visibility = Visibility.Collapsed;
            gridViewButton.IsChecked = false;
            listViewButton.IsChecked = true;
        }

        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inventoryDG.SelectedItem is InventoryTable selectedItem)
            {
                inventoryDG.Visibility = Visibility.Collapsed;
                ProductsWrapPanel.Visibility = Visibility.Visible;
                ProductBorder.Visibility = Visibility.Visible;
                LoadProductsForDisplay(selectedItem); 
            }
            else
            {
                inventoryDG.Visibility = Visibility.Visible;
                ProductsWrapPanel.Visibility = Visibility.Collapsed;
                ProductBorder.Visibility = Visibility.Collapsed;
            }
        }
        private void gridViewButton_Checked(object sender, RoutedEventArgs e)
        {
            TextBox.Visibility = Visibility.Collapsed;
        }

        private void listViewButton_Checked(object sender, RoutedEventArgs e)
        {
            TextBox.Visibility = Visibility.Visible;
        }
    }
}
