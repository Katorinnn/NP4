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
                PaymentAmount = amount; // Save the entered payment amount

                // Total cost of all items in the receipt (sum of quantity * price for each item)
                decimal totalCost = receiptItems.Sum(item => item.Quantity * item.Price);
                decimal changeGiven = (decimal)amount - totalCost;

                if (changeGiven < 0)
                {
                    MessageBox.Show("Insufficient payment amount.", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var context = new DataContext())
                {
                    try
                    {
                        // Generate a single transaction ID for the entire purchase
                        int transactionID = GenerateTransactionID();

                        // Process each item in the receipt
                        foreach (var receiptItem in receiptItems)
                        {
                            // Fetch the required number of tanks for this item
                            var availableTanks = context.tbl_inventory
                                                        .Where(i => i.ProductName == receiptItem.Brand && i.Size == receiptItem.Size && i.IsSold == false)
                                                        .Take(receiptItem.Quantity)
                                                        .ToList();

                            if (availableTanks.Count < receiptItem.Quantity)
                            {
                                MessageBox.Show($"Insufficient stock for {receiptItem.Brand} ({receiptItem.Size}).", "Stock Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            foreach (var tank in availableTanks)
                            {
                                tank.IsSold = true; // Mark each tank as sold

                                // Create a new transaction for each tank
                                var newTransaction = new ReportsTable
                                {
                                    TransactionID = transactionID,
                                    Date = DateTime.Now,
                                    ProductName = tank.ProductName,
                                    TankID = tank.TankID,
                                    Quantity = 1, // Each transaction corresponds to one tank
                                    UnitPrice = tank.Price,
                                    TotalPrice = tank.Price,
                                    PaymentMethod = "Cash",
                                    PaidAmount = (decimal)amount / receiptItems.Count, // Divide the payment proportionally
                                    ChangeGiven = changeGiven / receiptItems.Count, // Divide the change proportionally
                                    Status = TransactionStatus.Completed
                                };

                                context.tbl_reports.Add(newTransaction); // Add the transaction to the database
                            }
                        }

                        context.SaveChanges(); // Commit changes to the database
                        MessageBox.Show("Transaction completed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        this.DialogResult = true; // Close the payment window
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error processing transaction: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid payment amount.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }





        // Example method to generate a unique Tank I

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



        // Example method to generate a unique transaction ID
        //private int GenerateTransactionID()
        //{
        //    // Replace with logic to generate a unique ID
        //    return new Random().Next(100000, 999999);
        //}

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
