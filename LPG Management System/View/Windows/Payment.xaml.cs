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
using LPG_Management_System.Migrations;
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

        private DataContext _context;
        
        private double totalPrice;
        private ObservableCollection<pointofsaleUC.ReceiptItem> receiptItems;
        public int TotalQuantity { get; private set; }
        public double PaymentAmount { get; private set; }
        public Payment(double totalPrice, int totalQuantity, ObservableCollection<pointofsaleUC.ReceiptItem> receiptItems)
        {
            InitializeComponent();

            _context = new DataContext();

            NewCustomer_Checked(null, null);

            this.totalPrice = totalPrice;
            this.receiptItems = receiptItems;

            TotalQuantity = totalQuantity;
            TotalAmountLabel.Content = $"Total: ₱{totalPrice:F2}";

            GenerateCustomerID();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Auto-generate the Customer ID and display it
            GenerateCustomerID();
        }

        private void GenerateCustomerID()
        {
            Random random = new Random();
            int customerID = random.Next(100000, 999999);
            

            customerIDtxtBox.Text = customerID.ToString();
        }

        private void amountBtn_Click(object sender, RoutedEventArgs e)
        {

            string input = contacttxtBox.Text;
            int transactionID = GenerateTransactionID();

            // Check if the length is 11 and starts with "09"
            if (input.Length != 11 || !input.StartsWith("09"))
            {
                MessageBox.Show("The phone number must start with '09' and contain exactly 11 digits.");
                return; // Prevent proceeding if the validation fails
            }

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
                        var purchasedItems = receiptItems;

                        var newTransaction = new ReportsTable
                        {
                            TransactionID = transactionID,
                            Date = DateTime.Now,
                            ProductName = string.Join(", ", purchasedItems.Select(p => p.Brand)),
                            TankID = string.Join(", ", purchasedItems.Select(p => GenerateTankID().ToString())),
                            Quantity = TotalQuantity,
                            UnitPrice = string.Join(", ", purchasedItems.Select(p => p.Price.ToString("F2"))),
                            TotalPrice = purchasedItems.Sum(p => (decimal)p.Price),
                            PaymentMethod = "Cash",
                            PaidAmount = (decimal)amount,
                            ChangeGiven = changeGiven,
                            Status = TransactionStatus.Completed,
                            CustomerName = customertxtBox.Text
                        };

                        context.tbl_reports.Add(newTransaction);

                        // Add or update customer details
                        if (NewCustomer.IsChecked == true)
                        {
                            var newCustomer = new CustomersTable
                            {
                                CustomerID = int.Parse(customerIDtxtBox.Text),
                                CustomerName = customertxtBox.Text,
                                ContactNumber = contacttxtBox.Text,
                                Address = addresstxtBox.Text,
                                TankClassification = tankClassComboBox.Text,
                                TankID = transactionID
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
                                existingCustomer.TankID = transactionID;

                            }
                            else
                            {
                                throw new Exception("Customer not found. Please re-check the Customer ID.");
                            }
                        }

                        context.SaveChanges();

                        // Update the inventory after saving transaction and customer details
                        foreach (var item in purchasedItems)
                        {
                            var stock = context.tbl_inventory.FirstOrDefault(i => i.ProductName == item.Brand && i.Size == item.Size);
                            if (stock != null)
                            {
                                if (stock.Stocks >= item.Quantity)
                                {
                                    stock.Stocks -= item.Quantity;
                                }
                                else
                                {
                                    throw new Exception($"Insufficient stock for {item.Brand} ({item.Size}).");
                                }
                            }
                            else
                            {
                                throw new Exception($"Stock for {item.Brand} ({item.Size}) not found.");
                            }
                        }

                        // Notify the user of success
                        MessageBox.Show("Transaction completed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        // Handle any exception by showing an error message
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

            return new Random().Next(100000, 999999);
        }

        private int GenerateTransactionID()
        {
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
            customertxtBox.IsReadOnly = false;
            customerIDtxtBox.IsReadOnly = true;

            // Clear customer data fields
            customertxtBox.Clear();
            customerIDtxtBox.Clear();
            addresstxtBox.Clear();
            contacttxtBox.Clear();
            tankClassComboBox.SelectedIndex = 0;


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
            tankClassComboBox.SelectedIndex = 0;

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
            if (customerListBox.SelectedItem is CustomersTable selectedCustomer)
            {
                // Set the customer fields
                customertxtBox.Text = selectedCustomer.CustomerName;
                customerIDtxtBox.Text = selectedCustomer.CustomerID.ToString();
                contacttxtBox.Text = selectedCustomer.ContactNumber;
                addresstxtBox.Text = selectedCustomer.Address;

                // Properly set the selected item in the ComboBox
                if (tankClassComboBox.Items.Contains(selectedCustomer.TankClassification))
                {
                    tankClassComboBox.SelectedItem = selectedCustomer.TankClassification;
                }
                else
                {
                    // Add the tank classification if not already present in the ComboBox
                    tankClassComboBox.Items.Add(selectedCustomer.TankClassification);
                    tankClassComboBox.SelectedItem = selectedCustomer.TankClassification;
                }

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
            // Check if the entered character is a digit
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true; // Prevent non-digit characters from being typed
            }
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

        private void contacttxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string input = contacttxtBox.Text;

            // Check if the length is 11 and starts with "09"
            if (input.Length != 11 || !input.StartsWith("09"))
            {
                MessageBox.Show("The phone number must start with '09' and contain exactly 11 digits.");
                // Optionally focus back on the TextBox to correct the input
                contacttxtBox.Focus();
            }
            else
            {
                // Format the number with a space after the second character
                contacttxtBox.Text = input.Substring(0, 2) + " " + input.Substring(2);
            }
        }

        private void contacttxtBox_KeyUp(object sender, KeyEventArgs e)
        {
            string input = contacttxtBox.Text;

            if (input.Length == 11)
            {
                contacttxtBox.SelectionStart = contacttxtBox.Text.Length;// Keep the cursor at the end
            }
        }





    }
}
