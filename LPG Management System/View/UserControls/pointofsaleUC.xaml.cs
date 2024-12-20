using LPG_Management_System.Models;
using LPG_Management_System.View.Windows;
using System;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace LPG_Management_System.View.UserControls
{
    /// <summary>
    /// Interaction logic for pointofsaleUC.xaml
    /// </summary>
    public partial class pointofsaleUC : UserControl
    {

        private Button currentKgButton;

        public class ReceiptItem
        {
            public string Brand { get; set; }
            public string Size { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; } = 1;
            public double Total => Price * Quantity;
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
        private void UpdateReceiptDisplay()
        {
            // Bind receipt items to DataGrid
            dataGridItems.ItemsSource = null;
            dataGridItems.ItemsSource = receiptItems;

            // Update Total Price
            UpdateTotalPrice();
        }


        private void DecreaseQuantity(ReceiptItem item)
        {
            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                receiptItems.Remove(item);
            }

            UpdateTotalPrice();
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

                // Ensure receiptItems is a List<ReceiptItem>
                var receiptList = receiptItems.ToList();

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

                // Create and show Invoice page
                var invoicePage = new Invoice(receiptList, "Customer Address Here",
                                              finalTotalPrice, paymentAmount, change);
                invoicePage.Show();
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
                productControl.SizeLabel.Content = product.Size;

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
                existingItem.Quantity++;
            }
            else
            {
                receiptItems.Add(item);
            }

            UpdateTotalPrice();
        }

        private void FilterByBrand_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string brand)
            {
                // Call LoadProducts with the selected brand
                LoadProducts(brand);
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
