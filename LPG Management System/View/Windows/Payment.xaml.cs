using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using LPG_Management_System.View.UserControls;
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
        private ObservableCollection<pointofsaleUC.ReceiptItem> receiptItems; // Add this line
        public int TotalQuantity { get; private set; }
        public double PaymentAmount { get; private set; } // Holds the entered amount
        public Payment(double totalPrice, int totalQuantity, ObservableCollection<pointofsaleUC.ReceiptItem> receiptItems) // Modify constructor
        {
            InitializeComponent();

            _context = new DataContext();

            NewCustomer_Checked(null, null);

            this.totalPrice = totalPrice;
            this.receiptItems = receiptItems; // Assign to the class variable

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
                    try
                    {
                        // Pass the receipt items here for processing
                        var purchasedItems = receiptItems;

                        // If you have a limit on the quantity of available stock, validate here
                        if (purchasedItems.Count < TotalQuantity)
                        {
                            MessageBox.Show($"Insufficient stock. Only {purchasedItems.Count} items are available.", "Stock Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // 🔥 Combine brand names into a single string
                        string combinedBrands = string.Join(", ", purchasedItems.Select(p => p.Brand));

                        // 🔥 Combine unit prices into a single string (formatted to 2 decimal places)
                        string combinedUnitPrices = string.Join(", ", purchasedItems.Select(p => p.Price.ToString("F2")));


                        // 🔥 Generate tank IDs (assuming GenerateTankID() is defined)
                        string combinedTankIDs = string.Join(", ", purchasedItems.Select(p => GenerateTankID().ToString()));

                        // 🔥 Calculate the total price from all selected items
                        decimal totalUnitPrice = purchasedItems.Sum(p => (decimal)p.Price);

                        // Save transaction
                        var newTransaction = new ReportsTable
                        {
                            TransactionID = GenerateTransactionID(),
                            Date = DateTime.Now,
                            ProductName = combinedBrands,
                            TankID = combinedTankIDs,
                            Quantity = TotalQuantity,
                            UnitPrice = combinedUnitPrices,
                            TotalPrice = totalUnitPrice,  // Total price for all products
                            PaymentMethod = "Cash",
                            PaidAmount = (decimal)amount,
                            ChangeGiven = changeGiven,
                            Status = TransactionStatus.Completed
                        };

                        context.tbl_reports.Add(newTransaction);

                        // Add or update customer information
                        if (NewCustomer.IsChecked == true)
                        {
                            var newCustomer = new CustomersTable
                            {
                                CustomerID = int.Parse(customerIDtxtBox.Text),
                                CustomerName = customertxtBox.Text,
                                ContactNumber = contacttxtBox.Text,
                                Address = addresstxtBox.Text,
                                TankClassification = tankClassComboBox.Text
                            };
                            context.tbl_customers.Add(newCustomer);
                        }
                        else if (OldCustomer.IsChecked == true)
                        {
                            var existingCustomer = context.tbl_customers
                                .FirstOrDefault(c => c.CustomerID == int.Parse(customerIDtxtBox.Text));

                            if (existingCustomer != null)
                            {
                                existingCustomer.CustomerName = customertxtBox.Text;
                                existingCustomer.ContactNumber = contacttxtBox.Text;
                                existingCustomer.Address = addresstxtBox.Text;
                                existingCustomer.TankClassification = tankClassComboBox.Text;
                            }
                            else
                            {
                                MessageBox.Show("Customer not found. Please re-check the Customer ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }

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
            // Disable fields for new customer and allow search by customer name for old customer
            contacttxtBox.IsEnabled = false;
            addresstxtBox.IsEnabled = false;
            customertxtBox.IsReadOnly = false; // Allow typing for search functionality

            // Enable customer ID for search only, to prevent changes
            customerIDtxtBox.IsReadOnly = true;

            // If no customer is selected or typed in, clear customer data fields
            customertxtBox.Clear();
            customerIDtxtBox.Clear();
            addresstxtBox.Clear();
            contacttxtBox.Clear();
        }

        private void NewCustomer_Checked(object sender, RoutedEventArgs e)
        {
            if (contacttxtBox == null || addresstxtBox == null || customertxtBox == null || customerIDtxtBox == null)
                return;

            // Enable contact and address fields for new customer
            contacttxtBox.IsEnabled = true;
            addresstxtBox.IsEnabled = true;

            // Clear the fields for new customer
            customertxtBox.Clear();
            customerIDtxtBox.Clear();
            contacttxtBox.Clear();
            addresstxtBox.Clear();

            // Allow customer name to be entered freely
            customertxtBox.IsReadOnly = false;

            // Set the customer ID box to allow entry (for new customer)
            customerIDtxtBox.IsReadOnly = false;
        }

        private void customertxtBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string searchQuery = customertxtBox.Text;

            // Only show customer names for "Old Customer" selection and when there's input
            if (OldCustomer.IsChecked == true && searchQuery.Length > 0)
            {
                var customers = _context.tbl_customers
                                        .Where(c => c.CustomerName.Contains(searchQuery))
                                        .ToList();

                if (customers.Any())
                {
                    customerListBox.ItemsSource = customers; // Set the data source for the list box
                    customerListBox.Visibility = Visibility.Visible; // Show the list box
                }
                else
                {
                    customerListBox.ItemsSource = null; // Clear the list if no matches
                    customerListBox.Visibility = Visibility.Collapsed; // Hide the list box if no customers found
                }
            }
            else
            {
                // Hide the list box if no search query or if new customer is selected
                customerListBox.ItemsSource = null;
                customerListBox.Visibility = Visibility.Collapsed;
            }
        }

        private void customerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If an item (customer) is selected from the list
            if (customerListBox.SelectedItem is CustomersTable selectedCustomer)
            {
                // Fill in the remaining fields with the selected customer's details
                customertxtBox.Text = selectedCustomer.CustomerName;
                customerIDtxtBox.Text = selectedCustomer.CustomerID.ToString();
                contacttxtBox.Text = selectedCustomer.ContactNumber;
                addresstxtBox.Text = selectedCustomer.Address;

                // Hide the list box after selection
                customerListBox.Visibility = Visibility.Collapsed;
            }
        }



        private void customertxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only letters and spaces for the customer name input (search for old customers)
            e.Handled = !char.IsLetter(e.Text, 0) && e.Text != " ";
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if a selection was made other than the first item
            if (tankClassComboBox.SelectedIndex > 0)
            {
                // Remove the first item (Tank Class) when another item is selected
                tankClassComboBox.Items.RemoveAt(0);
            }
        }
    }
}
