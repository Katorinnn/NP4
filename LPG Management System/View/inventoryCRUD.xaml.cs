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
    public partial class inventoryCRUD : Window
    {
        public string TankId { get; set; }
        public inventoryCRUD()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TankId))
            {
                // Example: Display the TankId or prefill data based on it
                MessageBox.Show($"Editing details for Tank ID: {TankId}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save logic here
            DialogResult = true; // Indicate success and close the dialog
            Close();
        }
        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            // Collect user input
            string tankID = tankIDtxtBox.Text;
            string brandname = brandtxtBox.Text;
            string size = sizetxtBox.Text;
            string price = pricetxtBox.Text;

            // Connection string (update according to your DB credentials)
            string connectionString = "server=localhost;database=db_lpgpos;user=root;";

            try
            {
                // Insert data into the database
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO tbl_inventory (tankID, brandName, size, price) VALUES (@tankID, @brandname, @size, @price)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Use parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@tankID", tankID);
                        command.Parameters.AddWithValue("@brandname", brandname);
                        command.Parameters.AddWithValue("@size", size);
                        command.Parameters.AddWithValue("@price", price);

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

        private void tankIDtxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
