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
        public double PaymentAmount { get; private set; } // Holds the entered amount
        public Payment(double totalPrice)
        {
            InitializeComponent();

            _context = new DataContext(); // Initialize the context

            NewCustomer_Checked(null, null);

            this.totalPrice = totalPrice;

            TotalAmountLabel.Content = $"Total: ₱{totalPrice:F2}";
        }

        private void amountBtn_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(amounttxtBox.Text, out double amount) && amount > 0)
            {
                PaymentAmount = amount; // Save entered payment amount

                // Calculate change if overpayment occurs
                decimal changeGiven = (decimal)amount - (decimal)totalPrice;
                if (changeGiven < 0)
                {
                    MessageBox.Show("Insufficient payment amount.", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Retrieve ProductName and TankID from InventoryTable
                int tankId = int.TryParse(tankIDtxtBox.Text, out int parsedTankId) ? parsedTankId : 0;

                using (var context = new DataContext())
                {
                    var inventoryItem = context.tbl_inventory.FirstOrDefault(i => i.TankID == tankId);
                    if (inventoryItem == null)
                    {
                        MessageBox.Show("Tank ID not found in inventory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string productName = inventoryItem.ProductName;

                    // Create a new transaction record
                    var newTransaction = new ReportsTable
                    {
                        TransactionID = GenerateTransactionID(),
                        Date = DateTime.Now,
                        ProductName = productName,
                        TankID = tankId,
                        Quantity = 1, // Replace with the actual quantity if needed
                        UnitPrice = inventoryItem.Price,
                        TotalPrice = (decimal)totalPrice,
                        PaymentMethod = "Cash", // Replace with actual payment method if needed
                        PaidAmount = (decimal)amount,
                        ChangeGiven = changeGiven,
                        Status = TransactionStatus.Completed // Directly use the enum
                    };

                    try
                    {
                        context.tbl_reports.Add(newTransaction); // Save transaction
                        context.SaveChanges(); // Persist changes
                        this.DialogResult = true; // Close the window with a success result
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

        // Example method to generate a unique transaction ID
        private int GenerateTransactionID()
        {
            // Replace with logic to generate a unique ID
            return new Random().Next(1000, 9999);
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
            if (contacttxtBox == null || addresstxtBox == null || customertxtBox == null || tankIDtxtBox == null)
                return;

            contacttxtBox.IsEnabled = true;
            addresstxtBox.IsEnabled = true;

            customertxtBox.Text = "";
            tankIDtxtBox.Text = "";
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
                tankIDtxtBox.Text = customer.TankID.ToString();
                addresstxtBox.Text = customer.Address;
            }
        }
    }
}
