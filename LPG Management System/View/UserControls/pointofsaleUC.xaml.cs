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
                        OnPropertyChanged(nameof(Total));
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
            
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
        }

        //Payment Options
        private void cashBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (receiptItems.Count == 0)
            {
                MessageBox.Show("Please select at least one product before proceeding with payment.",
                                "No Products Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double totalPrice = receiptItems.Sum(item => item.Total);
            int totalQuantity = receiptItems.Sum(item => item.Quantity);

            Payment paymentWindow = new Payment(totalPrice, totalQuantity, receiptItems);
                
            if (paymentWindow.ShowDialog() == true)
            {
                double paymentAmount = paymentWindow.PaymentAmount;
                double change = paymentAmount - totalPrice;

                if (change < 0)
                {
                    MessageBox.Show("Payment amount is less than the total price. Please enter a valid amount.",
                                    "Insufficient Payment", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    using (var dbContext = new DataContext())
                    {
                        foreach (var item in receiptItems)
                        {
                            var product = dbContext.tbl_inventory
                                .FirstOrDefault(p => p.ProductName == item.Brand &&
                                                     p.Size == item.Size &&
                                                     p.Price == (decimal)item.Price);

                            if (product != null)
                            {
                                product.Stocks -= item.Quantity;
                                if (product.Stocks <= 0)
                                {
                                    dbContext.tbl_inventory.Remove(product);
                                }
                            }
                        }

                        dbContext.SaveChanges();
                    }

                    LoadProducts();
                    var receiptItemsList = receiptItems.ToList();
                    var invoicePage = new Invoice(receiptItemsList, "Customer Address Here", totalPrice, paymentAmount, change);
                    invoicePage.Show();
                    receiptItems.Clear();
                    UpdateTotalPrice();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating inventory: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void UpdateTotalPrice()
        {
            double totalPrice = receiptItems.Sum(item => item.Total);
            TotalPriceLabel.Content = $"₱{totalPrice:F2}";
        }
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
            var products = GetProductsFromDatabase();

            ProductsPanel.Children.Clear();

            foreach (var product in products)
            {
                var productControl = new Product
                {
                    DataContext = this
                };
                productControl.BrandLabel.Content = product.ProductName;
                productControl.PriceLabel.Content = $"₱{product.Price:F2}";
                productControl.SizeLabel.Content = $"{product.Size}";

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
                item.Quantity = 1;
                receiptItems.Add(item);
            }

            UpdateTotalPrice();
        }



        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is ReceiptItem selectedItem)
            {
                selectedItem.Quantity++;
                dataGridItems.Items.Refresh();
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

                dataGridItems.Items.Refresh();
                UpdateTotalPrice();
            }
        }




        private void FilterByBrand_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Tag is string brand)
                {
                    if (brand.Equals("All", StringComparison.OrdinalIgnoreCase))
                    {
                        LoadProducts();
                    }
                    else
                    {
                        LoadProducts(brand);
                    }
                }
            }
        }


        private void LoadProducts(string filterBrand = null)
        {
            var products = GetProductsFromDatabase();

            if (!string.IsNullOrEmpty(filterBrand))
            {
                products = products.Where(p => p.ProductName.Equals(filterBrand, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var distinctProducts = products
                .GroupBy(p => new { p.ProductName, p.Size, p.Price })
                .Select(g => g.First())
                .ToList();

            ProductsPanel.Children.Clear();

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
