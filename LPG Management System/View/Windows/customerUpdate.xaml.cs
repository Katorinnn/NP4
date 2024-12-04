using MySql.Data.MySqlClient;
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

namespace LPG_Management_System.View
{
    /// <summary>
    /// Interaction logic for inventoryCRUD.xaml
    /// </summary>
    public partial class customerUpdate : Window
    {

        string connectionString = "server=localhost;database=db_lpgpos;user=root;";
        public string customerId { get; set; }
        public customerUpdate(int customerID)
        {
            InitializeComponent();
            LoadItemData(customerID);
        }

        private void LoadItemData(int customerId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM tbl_customers WHERE customerID = @customerID"; // Assuming tankID is a column
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@customerID", customerId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate the controls (e.g., TextBoxes) with the data from the database
                            customerIDtxtBox.Text = reader["customerID"].ToString();
                            tankIDtxtBox.Text = reader["tankID"].ToString();
                            cuNametxtBox.Text = reader["customerName"].ToString();
                            contacttxtBox.Text = reader["contactNumber"].ToString();
                            addresstxtBox.Text = reader["address"].ToString();
                        }
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
            string customerID = customerIDtxtBox.Text;
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
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE tbl_customers SET customerName = @customerName, contactNumber = @contactNumber, address = @address WHERE customerID = @customerID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@tankID", tankID);
                        command.Parameters.AddWithValue("@customerName", customerName);
                        command.Parameters.AddWithValue("@contactNumber", contactNumber);
                        command.Parameters.AddWithValue("@address", address);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("No records were updated.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void tankIDtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
