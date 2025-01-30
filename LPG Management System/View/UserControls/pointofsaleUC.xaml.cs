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
    public partial class pointofsaleUC : UserControl
    {
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
                        OnPropertyChanged(nameof(Total));
                    }
                }
            }
            public double Total => Price * Quantity;
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<ReceiptItem> receiptItems = new();
        public pointofsaleUC()
        {
            InitializeComponent();
            dataGridItems.ItemsSource = receiptItems;
            LoadProducts();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new Dashboard().Show();
        }

        private void cashBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!receiptItems.Any())
            {
                ShowWarning("Please select at least one product before proceeding with payment.");
                return;
            }

            double totalPrice = receiptItems.Sum(item => item.Total);
            int totalQuantity = receiptItems.Sum(item => item.Quantity);

            var paymentWindow = new Payment(totalPrice, totalQuantity, receiptItems);
            if (paymentWindow.ShowDialog() == true)
            {
                ProcessPayment(paymentWindow.PaymentAmount, totalPrice);
            }
        }
        private void ProcessPayment(double paymentAmount, double totalPrice)
        {
            double change = paymentAmount - totalPrice;
            if (change < 0)
            {
                ShowError("Payment amount is less than the total price. Please enter a valid amount.");
                return;
            }

            try
            {
                UpdateInventory();
                GenerateInvoice(paymentAmount, change, totalPrice);
            }
            catch (Exception ex)
            {
                ShowError($"Error updating inventory: {ex.Message}");
            }
        }
        private void UpdateInventory()
        {
            using var dbContext = new DataContext();
            var groupedItems = receiptItems
                .GroupBy(item => new { item.Brand, item.Size, item.Price })
                .Select(group => new
                {
                    group.Key.Brand,
                    group.Key.Size,
                    group.Key.Price,
                    TotalQuantity = group.Sum(item => item.Quantity)
                });

            foreach (var group in groupedItems)
            {
                var product = dbContext.tbl_inventory
                    .FirstOrDefault(p => p.ProductName == group.Brand &&
                                         p.Size == group.Size &&
                                         p.Price == (decimal)group.Price);

                if (product != null)
                {
                    product.Stocks -= group.TotalQuantity;
                    if (product.Stocks <= 0)
                    {
                        dbContext.tbl_inventory.Remove(product);
                    }
                }
            }

            dbContext.SaveChanges();
        }
        private void GenerateInvoice(double paymentAmount, double change, double totalPrice)
        {
            var receiptItemsList = receiptItems.ToList();
            new Invoice(receiptItemsList, "Customer Address Here", totalPrice, paymentAmount, change).Show();
            receiptItems.Clear();
            UpdateTotalPrice();
            LoadProducts();
        }

        private void UpdateTotalPrice()
        {
            TotalPriceLabel.Content = $"₱{receiptItems.Sum(item => item.Total):F2}";
        }

        private List<InventoryTable> GetProductsFromDatabase()
        {
            try
            {
                using var dbContext = new DataContext();
                return dbContext.tbl_inventory.ToList();
            }
            catch (Exception ex)
            {
                ShowError($"Error retrieving data: {ex.Message}");
                return new List<InventoryTable>();
            }
        }
        private void LoadProducts(string filterBrand = null)
        {
            var products = GetProductsFromDatabase();

            if (!string.IsNullOrEmpty(filterBrand))
            {
                products = products.Where(p => p.ProductName.Equals(filterBrand, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ProductsPanel.Children.Clear();
            foreach (var product in products.GroupBy(p => new { p.ProductName, p.Size, p.Price }).Select(g => g.First()))
            {
                var productControl = new Product
                {
                    DataContext = this,
                    BrandLabel = { Content = product.ProductName },
                    PriceLabel = { Content = $"₱{product.Price:F2}" },
                    SizeLabel = { Content = product.Size }
                };

                if (product.ProductImage?.Length > 0)
                {
                    productControl.ProductImage.Source = ConvertToImageSource(product.ProductImage);
                }

                ProductsPanel.Children.Add(productControl);
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
                ShowError($"Error loading image: {ex.Message}");
                return null;
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
            if (sender is Button { Tag: string brand })
            {
                LoadProducts(brand.Equals("All", StringComparison.OrdinalIgnoreCase) ? null : brand);
            }
        }
        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is ReceiptItem selectedItem)
            {
                selectedItem.Quantity++;
                UpdateTotalPrice();
            }
        }

        private void ReduceQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is ReceiptItem selectedItem)
            {
                if (selectedItem.Quantity > 1)
                {
                    selectedItem.Quantity--;
                }
                else
                {
                    receiptItems.Remove(selectedItem);
                }
                UpdateTotalPrice();
            }
        }
        private void ShowWarning(string message) => MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        private void ShowError(string message) => MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }


}
