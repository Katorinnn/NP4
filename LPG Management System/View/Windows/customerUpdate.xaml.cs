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
        public string TankId { get; set; }    
        public customerUpdate(int tankID)
        {
            InitializeComponent();
            LoadItemData(tankID);
        }

        private void LoadItemData(int tankId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM tbl_inventory WHERE tankID = @tankID"; // Assuming tankID is a column
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@tankID", tankId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate the controls (e.g., TextBoxes) with the data from the database
                            tankIDtxtBox.Text = reader["tankID"].ToString();
                            brandtxtBox.Text = reader["brandName"].ToString();
                            sizetxtBox.Text = reader["size"].ToString();
                            pricetxtBox.Text = reader["price"].ToString();
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
            string tankID = tankIDtxtBox.Text;
            string brandname = brandtxtBox.Text;
            string size = sizetxtBox.Text;
            string price = pricetxtBox.Text;

            if (string.IsNullOrEmpty(tankID) || string.IsNullOrEmpty(brandname) || string.IsNullOrEmpty(size) || string.IsNullOrEmpty(price))
            {
                MessageBox.Show("Please fill in all the fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE tbl_inventory SET brandName = @brandname, size = @size, price = @price WHERE tankID = @tankID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@tankID", tankID);
                        command.Parameters.AddWithValue("@brandname", brandname);
                        command.Parameters.AddWithValue("@size", size);
                        command.Parameters.AddWithValue("@price", price);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Brand updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update brand.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
