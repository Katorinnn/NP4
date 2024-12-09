using LPG_Management_System.Models;
using LPG_Management_System.View.Windows;
using System;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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


        private List<ReceiptItem> receiptItems = new List<ReceiptItem>();
        public pointofsaleUC()
        {
            InitializeComponent();
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
            ReceiptItemsPanel.Children.Clear();

            foreach (var item in receiptItems)
            {
                // Create a horizontal layout for each receipt item
                var itemPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

                var brandLabel = new Label { Content = item.Brand, Width = 150, FontSize = 16 };
                var sizeLabel = new Label { Content = item.Size, Width = 100, FontSize = 16 };
                var priceLabel = new Label { Content = $"₱{item.Price:F2}", Width = 100, FontSize = 16 };
                var quantityLabel = new Label { Content = $"Qty: {item.Quantity}", Width = 100, FontSize = 16 };
                var totalLabel = new Label { Content = $"₱{item.Total:F2}", Width = 100, FontSize = 16 };

                itemPanel.Children.Add(brandLabel);
                itemPanel.Children.Add(sizeLabel);
                itemPanel.Children.Add(priceLabel);
                itemPanel.Children.Add(quantityLabel);
                itemPanel.Children.Add(totalLabel);

                ReceiptItemsPanel.Children.Add(itemPanel);
            }
        }

        //Payment Options
        private void cashBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (receiptItems.Count == 0)
            {
                // No products selected, show a warning message
                MessageBox.Show("Please select at least one product before proceeding with payment.", "No Products Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Prevent further actions
            }

            // Calculate the total price of receipt items
            double totalPrice = receiptItems.Sum(item => item.Total);

            // Create a new Payment window and pass the total price
            Payment paymentWindow = new Payment(totalPrice);

            // Show the Payment window as a dialog
            if (paymentWindow.ShowDialog() == true)
            {
                // Get the payment amount entered by the user
                double paymentAmount = paymentWindow.PaymentAmount;

                // Calculate the change
                double change = paymentAmount - totalPrice;

                // Update the Change label
                ChangeLabel.Content = change >= 0
                    ? $"₱{change:F2}"
                    : "Insufficient payment!";

                // Show a warning if payment is insufficient
                if (change < 0)
                {
                    MessageBox.Show("The payment amount is less than the total price.", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void GcashBtn_Click(object sender, RoutedEventArgs e)
        {
            if (receiptItems.Count == 0)
            {
                // No products selected, show a warning message
                MessageBox.Show("Please select at least one product before proceeding with payment.", "No Products Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Prevent further actions
            }

            // Calculate the total price of receipt items
            double totalPrice = receiptItems.Sum(item => item.Total);

            // Create a new Payment window and pass the total price
            Payment paymentWindow = new Payment(totalPrice);

            // Show the Payment window as a dialog
            if (paymentWindow.ShowDialog() == true)
            {
                // Get the payment amount entered by the user
                double paymentAmount = paymentWindow.PaymentAmount;

                // Calculate the change
                double change = paymentAmount - totalPrice;

                // Update the Change label
                ChangeLabel.Content = change >= 0
                    ? $"₱{change:F2}"
                    : "Insufficient payment!";

                // Show a warning if payment is insufficient
                if (change < 0)
                {
                    MessageBox.Show("The payment amount is less than the total price.", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        //Calculatipons of payment
        private void UpdateTotalPrice()
        {
            // Calculate the total price using the total of each receipt item
            double totalPrice = receiptItems.Sum(item => item.Total);

            // Update the total price label
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
            // Check if the item already exists in the receipt
            var existingItem = receiptItems.FirstOrDefault(r => r.Brand == item.Brand && r.Size == item.Size && r.Price == item.Price);

            if (existingItem != null)
            {
                // If the item exists, increase its quantity
                existingItem.Quantity++;
            }
            else
            {
                // If the item does not exist, add it to the receipt list
                receiptItems.Add(item);
            }

            // Update the receipt display
            UpdateReceiptDisplay();

            // Update the total price
            UpdateTotalPrice();
        }



    }
}
