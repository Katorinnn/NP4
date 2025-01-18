using LPG_Management_System.Models;
using LPG_Management_System.View.Windows;
using System;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LPG_Management_System.View.UserControls
{
    /// <summary>
    /// Interaction logic for pointofsaleUC.xaml
    /// </summary>
    public partial class pointofsaleUC : UserControl
    {

        private Button currentKgButton;

        public class ReceiptItem : INotifyPropertyChanged
        {
            private int _quantity = 1;

            public string Brand { get; set; }
            public string Size { get; set; }
            public double Price { get; set; }

            public int Quantity
            {
                get => _quantity;
                set
                {
                    if (_quantity != value)
                    {
                        _quantity = value;
                        OnPropertyChanged(nameof(Quantity));
                        OnPropertyChanged(nameof(Total)); // Notify Total change as well
                    }
                }
            }

            public double Total => Price * Quantity;

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private ObservableCollection<ReceiptItem> receiptItems = new ObservableCollection<ReceiptItem>();


        public pointofsaleUC()
        {
            InitializeComponent();
            dataGridItems.ItemsSource = receiptItems;
            LoadProducts();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the Dashboard window when this button is clicked
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
        }



        //Payment Options
        private void cashBtn_Click_1(object sender, RoutedEventArgs e)
        {
            // Check if there are any items in the receipt
            if (receiptItems.Count == 0)
            {
                MessageBox.Show("Please select at least one product before proceeding with payment.",
                                "No Products Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Calculate the total price
            double totalPrice = receiptItems.Sum(item => item.Total);

            // Create the payment window
            Payment paymentWindow = new Payment(totalPrice);

            // Show payment window and check if the payment is successful
            if (paymentWindow.ShowDialog() == true)
            {
                // Retrieve the payment amount entered
                double paymentAmount = paymentWindow.PaymentAmount;

                // Variables to pass
                double finalTotalPrice = totalPrice;  // Total price
                double change = paymentAmount - finalTotalPrice;

                // Check for negative change (insufficient payment)
                if (change < 0)
                {
                    MessageBox.Show("Payment amount is less than the total price. Please enter a valid amount.",
                                    "Insufficient Payment", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update the inventory in the database
                try
                {
                    using (var dbContext = new DataContext())
                    {
                        foreach (var item in receiptItems)
                        {
                            var product = dbContext.tbl_inventory.FirstOrDefault(p => p.ProductName == item.Brand && p.Size == item.Size && p.Price == (decimal)item.Price);


                            if (product != null)
                            {
                                // Update the quantity or remove the product if no stock is left
                                product.Quantity -= item.Quantity;

                                if (product.Quantity <= 0)
                                {
                                    dbContext.tbl_inventory.Remove(product);
                                }
                            }
                        }

                        dbContext.SaveChanges();
                    }

                    // Reload the available products
                    LoadProducts();

                    // Clear the receipt items
                    receiptItems.Clear();

                    // Update the total price label
                    UpdateTotalPrice();

                    // Show the invoice
                    var invoicePage = new Invoice(receiptItems.ToList(), "Customer Address Here",
                                                  finalTotalPrice, paymentAmount, change);
                    invoicePage.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating inventory: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }






        //Calculatipons of payment
        private void UpdateTotalPrice()
        {
            double totalPrice = receiptItems.Sum(item => item.Total);
            TotalPriceLabel.Content = $"₱{totalPrice:F2}";
        }


        //products
        private List<InventoryTable> GetProductsFromDatabase()
        {
            var productList = new List<InventoryTable>();

            try
            {
                using (var dbContext = new DataContext())
                {
                    productList = dbContext.tbl_inventory.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving data: {ex.Message}");
            }

            return productList;
        }

        private System.Windows.Media.ImageSource ConvertToImageSource(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                MessageBox.Show("The image data is invalid or empty.", "Image Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            try
            {
                using (var ms = new MemoryStream(imageBytes))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    return image;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Image Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }


        private void LoadProducts()
        {
            // Get products from the database using EF
            var products = GetProductsFromDatabase();

            // Clear existing items in the WrapPanel
            ProductsPanel.Children.Clear();

            // Create Product controls and add them to the WrapPanel
            foreach (var product in products)
            {
                var productControl = new Product
                {
                    DataContext = this
                };
                productControl.BrandLabel.Content = product.ProductName;
                productControl.PriceLabel.Content = $"₱{product.Price:F2}";
                productControl.SizeLabel.Content = $"{product.Size} Kg";

                if (product.ProductImage != null && product.ProductImage.Length > 0)
                {
                    productControl.ProductImage.Source = ConvertToImageSource(product.ProductImage);
                }

                ProductsPanel.Children.Add(productControl);
            }
        }

        public void AddToReceipt(ReceiptItem item)
        {
            var existingItem = receiptItems.FirstOrDefault(r => r.Brand == item.Brand && r.Size == item.Size && r.Price == item.Price);

            if (existingItem != null)
            {
                existingItem.Quantity++; // Increment the quantity
            }
            else
            {
                receiptItems.Add(item); // Add a new item
            }   

            UpdateTotalPrice(); // Update total price
        }

        private void ReduceQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridItems.SelectedItem is ReceiptItem selectedItem)
            {
                // Reduce quantity by 1
                selectedItem.Quantity--;

                // If quantity is 0, remove the item
                if (selectedItem.Quantity <= 0)
                {
                    receiptItems.Remove(selectedItem);
                }

                // Refresh the DataGrid and update the total price
                dataGridItems.Items.Refresh();
                UpdateTotalPrice();
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridItems.SelectedItem is ReceiptItem selectedItem)
            {
                // Remove the selected item from the receipt
                receiptItems.Remove(selectedItem);

                // Refresh the DataGrid and update the total price
                dataGridItems.Items.Refresh();
                UpdateTotalPrice();
            }
        }



        private void FilterByBrand_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Check if the "All" button is clicked
                if (button.Tag is string brand)
                {
                    if (brand.Equals("All", StringComparison.OrdinalIgnoreCase))
                    {
                        // Load all products without filtering
                        LoadProducts();
                    }
                    else
                    {
                        // Load products filtered by the selected brand
                        LoadProducts(brand);
                    }
                }
            }
        }


        private void LoadProducts(string filterBrand = null)
        {
            // Get products from the database using EF
            var products = GetProductsFromDatabase();

            // Apply brand filter if specified
            if (!string.IsNullOrEmpty(filterBrand))
            {
                products = products.Where(p => p.ProductName.Equals(filterBrand, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Remove duplicate products based on Brand, Size, and Price
            var distinctProducts = products
                .GroupBy(p => new { p.ProductName, p.Size, p.Price })
                .Select(g => g.First())
                .ToList();

            // Clear existing items in the WrapPanel
            ProductsPanel.Children.Clear();

            // Create Product controls and add them to the WrapPanel
            foreach (var product in distinctProducts)
            {
                var productControl = new Product
                {
                    DataContext = this
                };
                productControl.BrandLabel.Content = product.ProductName;
                productControl.PriceLabel.Content = $"₱{product.Price:F2}";
                productControl.SizeLabel.Content = product.Size;

                if (product.ProductImage != null && product.ProductImage.Length > 0)
                {
                    productControl.ProductImage.Source = ConvertToImageSource(product.ProductImage);
                }

                ProductsPanel.Children.Add(productControl);
            }
        }



    }
}
