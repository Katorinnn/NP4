using LPG_Management_System.Models;
using System;
using System.Linq;
using System.Windows;

namespace LPG_Management_System.View
{
    public partial class customerUpdate : Window
    {
        public int CustomerId { get; set; }

        public customerUpdate(int customerID)
        {
            InitializeComponent();
            CustomerId = customerID;
            LoadItemData(customerID);
        }

        private void LoadItemData(int customerId)
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    var customer = dbContext.tbl_customers.FirstOrDefault(c => c.CustomerID == customerId);
                    if (customer != null)
                    {
                        customerIDtxtBox.Text = customer.CustomerID.ToString();
                        tankIDtxtBox.Text = customer.TankID.ToString();
                        cuNametxtBox.Text = customer.CustomerName;
                        contacttxtBox.Text = customer.ContactNumber;
                        addresstxtBox.Text = customer.Address;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading item data: " + ex.Message);
            }
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            string tankID = tankIDtxtBox.Text;
            string customerName = cuNametxtBox.Text;
            string contactNumber = contacttxtBox.Text;
            string address = addresstxtBox.Text;

            if (string.IsNullOrEmpty(tankID) || string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(contactNumber) || string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Please fill in all the fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var dbContext = new DataContext())
                {
                    var customer = dbContext.tbl_customers.FirstOrDefault(c => c.CustomerID == CustomerId);
                    if (customer != null)
                    {
                        customer.TankID = int.Parse(tankID);
                        customer.CustomerName = customerName;
                        customer.ContactNumber = contactNumber;
                        customer.Address = address;

                        dbContext.SaveChanges();  

                        MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Customer not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
