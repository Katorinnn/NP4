using LPG_Management_System.View.Windows;
using System;
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
        }

        private List<ReceiptItem> receiptItems = new List<ReceiptItem>();

        private readonly Dictionary<string, Dictionary<string, double>> gasPrices = new Dictionary<string, Dictionary<string, double>>()
        {
            { "Coastal", new Dictionary<string, double> { { "5kg", 500.0 }, { "11kg", 1000.0 } } },
            { "Gaz Lite", new Dictionary<string, double> { { "230g", 100.0 }, { "330g", 150.0 } } },
            { "Solane", new Dictionary<string, double> { { "11kg", 1100.0 } } },
            { "Regasco", new Dictionary<string, double> { { "5kg", 550.0 }, { "2.7kg", 300.0 }, { "11kg", 1050.0 } } }
        };
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

        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide all panels initially
            CoastalPanel.Visibility = Visibility.Collapsed;
            GazLitePanel.Visibility = Visibility.Collapsed;
            SolanePanel.Visibility = Visibility.Collapsed;
            RegascoPanel.Visibility = Visibility.Collapsed;

            if (sender is Button button)
            {
                string selectedBrand = button.Content.ToString();
                BrandLabel.Content = $"Brand: {selectedBrand}";

                // Show the corresponding panel based on the selected brand
                switch (selectedBrand)
                {
                    case "Coastal":
                        CoastalPanel.Visibility = Visibility.Visible;
                        break;
                    case "Gaz Lite":
                        GazLitePanel.Visibility = Visibility.Visible;
                        break;
                    case "Solane":
                        SolanePanel.Visibility = Visibility.Visible;
                        break;
                    case "Regasco":
                        RegascoPanel.Visibility = Visibility.Visible;
                        break;
                }
            }
        }


        private void SizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                currentKgButton = button;

                // Get the size and display it in the label
                string selectedSize = button.Content.ToString();
                //SizeLabel.Content = $"Size: {selectedSize}";

                // Position the popup relative to the button
                KgPopup.PlacementTarget = button;
                KgPopup.Visibility = Visibility.Visible;
                KgPopup.IsOpen = true;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentKgButton != null)
            {
                string size = currentKgButton.Content.ToString();
                string brand = BrandLabel.Content.ToString();

                // Set a mock price (you can fetch the real price from a database)
                double price = 500.00; // Replace with actual price logic

                // Add to receipt list
                receiptItems.Add(new ReceiptItem
                {
                    Brand = brand,
                    Size = size,
                    Price = price
                });

                // Update Receipt Display
                UpdateReceiptDisplay();

                // Update Total Price
                UpdateTotalPrice();

                // Close the popup
                KgPopup.IsOpen = false;
                KgPopup.Visibility = Visibility.Collapsed;
            }
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

                itemPanel.Children.Add(brandLabel);
                itemPanel.Children.Add(sizeLabel);
                itemPanel.Children.Add(priceLabel);

                ReceiptItemsPanel.Children.Add(itemPanel);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the popup
            KgPopup.IsOpen = false;
            KgPopup.Visibility = Visibility.Collapsed;
        }

        //Payment Options
        private void cashBtn_Click_1(object sender, RoutedEventArgs e)
        {
            // Calculate the total price of receipt items
            double totalPrice = receiptItems.Sum(item => item.Price);

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
            double totalAmount = receiptItems.Sum(item => item.Price);
            Payment payment = new Payment(totalAmount);
            payment.ShowDialog();
        }

        //Calculatipons of payment
        private void UpdateTotalPrice()
        {
            double totalPrice = receiptItems.Sum(item => item.Price);
            TotalPriceLabel.Content = $"₱{totalPrice:F2}";
        }

        //products
        private void LoadProducts()
        {
            // Example data for products
            var products = new List<ProductModel>
            {
                new ProductModel { BrandName = "Solane", Price = "₱800", ImagePath = "/Images/solanesample.jpg" },
                //new ProductModel { BrandName = "Gaz Lite", Price = "₱600", ImagePath = "/Images/gazlite.jpg" },
                new ProductModel { BrandName = "Regasco", Price = "₱500", ImagePath = "/Images/RegascoSample.jpg" },
                new ProductModel { BrandName = "Regasco", Price = "₱800", ImagePath = "/Images/RegascoBig.png" }
            };

            // Create Product controls and add them to the WrapPanel
            foreach (var product in products)
            {
                var productControl = new Product();
                productControl.BrandLabel.Content = product.BrandName;
                productControl.PriceLabel.Content = product.Price;

                // Set the image source
                var image = new System.Windows.Media.Imaging.BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(product.ImagePath, UriKind.Relative);
                image.EndInit();
                productControl.ProductImage.Source = image;

                // Add to WrapPanel
                ProductsPanel.Children.Add(productControl);
            }
        }
    }

    public class ProductModel
    {
        public string BrandName { get; set; }
        public string Price { get; set; }
        public string ImagePath { get; set; }
    }
}
