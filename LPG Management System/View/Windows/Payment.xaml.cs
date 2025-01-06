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


namespace LPG_Management_System.View.Windows
{
    /// <summary>
    /// Interaction logic for Payment.xaml
    /// </summary>
    public partial class Payment : Window
    {


        private double totalPrice;
        public double PaymentAmount { get; private set; } // Holds the entered amount
        public Payment(double totalPrice)
        {
            InitializeComponent();
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
                        MessageBox.Show($"Payment successful!\nChange Given: ₱{changeGiven:F2}",
                                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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




        //public void UpdateTotalPrice(double newTotalPrice)
        //{
        //    totalPrice = newTotalPrice; // Update the internal price
        //    TotalAmountLabel.Content = $"Total: ₱{totalPrice:F2}"; // Update the label to reflect the new total
        //}

        private void amounttxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
