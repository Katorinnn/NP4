using LPG_Management_System.View.UserControls;
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
    /// Interaction logic for customerCRUD.xaml
    /// </summary>
    public partial class customerCRUD : Window
    {
        customersUC customersUC = new customersUC();

        public customerCRUD()
        {
            InitializeComponent();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            // Collect user input
            string tankID = tankIDtxtBox.Text;
            string customerName = customertxtBox.Text;
            string contactNumber = contacttxtBox.Text;
            string address = addresstxtBox.Text;

            // Connection string (update according to your DB credentials)
            string connectionString = "server=localhost;database=db_lpgpos;user=root;";

            try
            {
                // Insert data into the database
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO tbl_customers (TankID, CustomerName, ContactNumber, Address) VALUES (@TankID, @CustomerName, @ContactNumber, @Address)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Use parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@tankID", tankID);
                        command.Parameters.AddWithValue("@customerName", customerName);
                        command.Parameters.AddWithValue("@contactNumber", contactNumber);
                        command.Parameters.AddWithValue("@address", address);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Customer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true;

                            //close form
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add customer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
