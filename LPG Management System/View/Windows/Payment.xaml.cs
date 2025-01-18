using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LPG_Management_System.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


namespace LPG_Management_System.View.Windows
{
    /// <summary>
    /// Interaction logic for Payment.xaml
    /// </summary>
    public partial class Payment : Window
    {

        private DataContext _context; // Declare the context
        
        private double totalPrice;
        public int TotalQuantity { get; private set; }
        public double PaymentAmount { get; private set; } // Holds the entered amount
        public Payment(double totalPrice, int totalQuantity)
        {
            InitializeComponent();

            _context = new DataContext(); // Initialize the context

            NewCustomer_Checked(null, null);

            this.totalPrice = totalPrice;

            TotalQuantity = totalQuantity;

            TotalAmountLabel.Content = $"Total: ₱{totalPrice:F2}";
        }

        private void amountBtn_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(amounttxtBox.Text, out double amount) && amount > 0)
            {
                PaymentAmount = amount;

                decimal changeGiven = (decimal)amount - (decimal)totalPrice;
                if (changeGiven < 0)
                {
                    MessageBox.Show("Insufficient payment amount.", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var context = new DataContext())
                {
                    // Fetch all purchased items
                    var purchasedItems = context.tbl_inventory
                                                .Where(i => i.Stocks > 0)
                                                .Take(TotalQuantity)
                                                .ToList();

                    if (purchasedItems.Count < TotalQuantity)
                    {
                        MessageBox.Show($"Insufficient stock. Only {purchasedItems.Count} items are available.", "Stock Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    try
                    {
                        // 🔥 Combine brand names into a single string
                        string combinedBrands = string.Join(", ", purchasedItems.Select(p => p.ProductName));

                        decimal totalUnitPrice = purchasedItems.Sum(p => p.Price);

                        string combinedUnitPrices = string.Join(", ", purchasedItems.Select(p => p.Price.ToString("F2")));

                        string combinedTankIDs = string.Join(", ", purchasedItems.Select(p => GenerateTankID().ToString()));


                        // 🔥 Save all brands in one row
                        var newTransaction = new ReportsTable
                        {
                            TransactionID = GenerateTransactionID(),
                            Date = DateTime.Now,
                            ProductName = combinedBrands,   // Combined brands
                            TankID = combinedTankIDs,
                            Quantity = TotalQuantity,
                            UnitPrice = combinedUnitPrices,
                            TotalPrice = totalUnitPrice,
                            PaymentMethod = "Cash",
                            PaidAmount = (decimal)amount,
                            ChangeGiven = changeGiven,
                            Status = TransactionStatus.Completed
                        };

                        context.tbl_reports.Add(newTransaction);
                        context.SaveChanges();

                        MessageBox.Show("Transaction completed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        this.DialogResult = true;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving transaction: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid payment amount.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private int GenerateTankID()
        {
            // Replace this logic with a more robust method if necessary
            return new Random().Next(100000, 999999); // Generates a random integer ID
        }

        // Example method to generate a unique transaction ID
        private int GenerateTransactionID()
        {
            // Replace this logic with an actual Transaction ID generation method
            return new Random().Next(100000, 999999);
        }

        private void amounttxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OldCustomer_Checked(object sender, RoutedEventArgs e)
        {
            contacttxtBox.IsEnabled = false;
            addresstxtBox.IsEnabled = false;

            customertxtBox.IsReadOnly = false; // Allow typing for search functionality
        }

        private void NewCustomer_Checked(object sender, RoutedEventArgs e)
        {
            if (contacttxtBox == null || addresstxtBox == null || customertxtBox == null || customerIDtxtBox == null)
                return;

            contacttxtBox.IsEnabled = true;
            addresstxtBox.IsEnabled = true;

            customertxtBox.Text = "";
            customerIDtxtBox.Text = "";
            customertxtBox.IsReadOnly = false;
        }


        private void customertxtBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string searchQuery = customertxtBox.Text;

            var customers = _context.tbl_customers
                                    .Where(c => c.CustomerName.Contains(searchQuery))
                                    .ToList();

            // Optional: Use an autocomplete control to show customer suggestions
            // For now, select the first match for demonstration
            if (customers.Any())
            {
                var customer = customers.First();
                customertxtBox.Text = customer.CustomerName;
                customerIDtxtBox.Text = customer.CustomerID.ToString();
                addresstxtBox.Text = customer.Address;
            }
        }

        

        private void customerIDtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void customerIDtxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numeric input
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void customertxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only letters and spaces
            e.Handled = !char.IsLetter(e.Text, 0) && e.Text != " ";
        }

        private void contacttxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numeric input for contact number
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void addresstxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow any input for address
            e.Handled = false; // No restriction, but you can add length validation if necessary
        }

        private void amounttxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numeric input and decimal points for amount
            e.Handled = !char.IsDigit(e.Text, 0) && e.Text != ".";
        }

    }
}
