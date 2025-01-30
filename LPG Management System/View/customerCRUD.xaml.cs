using LPG_Management_System.Models;
using System;
using System.Windows;

namespace LPG_Management_System.View
{
    public partial class customerCRUD : Window
    {
        public customerCRUD()
        {
            InitializeComponent();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            string tankIDText = tankIDtxtBox.Text;
            string customerName = customertxtBox.Text;
            string contactNumber = contacttxtBox.Text;
            string address = addresstxtBox.Text;

            if (string.IsNullOrWhiteSpace(tankIDText) ||
                string.IsNullOrWhiteSpace(customerName) ||
                string.IsNullOrWhiteSpace(contactNumber) ||
                string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(tankIDText, out int tankID))
            {
                MessageBox.Show("Tank ID must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new DataContext())
                {
                    var customer = new CustomersTable
                    {
                        TankID = tankID,
                        CustomerName = customerName,
                        ContactNumber = contactNumber,
                        Address = address
                    };

                    db.tbl_customers.Add(customer);
                    db.SaveChanges();

                    MessageBox.Show("Customer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.DialogResult = true;
                    this.Close();
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
    }
}
