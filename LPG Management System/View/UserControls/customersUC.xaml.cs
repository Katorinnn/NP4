using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LPG_Management_System.View.UserControls
{
    /// <summary>
    /// Interaction logic for customersUC.xaml
    /// </summary>
    public partial class customersUC : UserControl
    {

        private readonly string connectionString = "server=localhost;database=db_lpgpos;user=root;";
        public customersUC()
        {
            InitializeComponent();
            LoadCustomersData();
        }

        private void LoadCustomersData()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Query to fetch customer data
                    string query = "SELECT * FROM tbl_customers";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind data to DataGrid
                    customersDG.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            customerCRUD cg = new customerCRUD();
            bool? dialogResult = cg.ShowDialog();

            // Check if the dialog was successful (e.g., a new record was added)
            if (dialogResult == true) // Assuming `true` is returned when a record is added
            {
                // Refresh the data in the DataGrid
                LoadCustomersData();
            }
        }

        //SearchBar
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Get the text from the search TextBox
            string searchText = (sender as TextBox)?.Text;

            if (!string.IsNullOrEmpty(searchText))
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        // Use a parameterized query to prevent SQL injection
                        string query = "SELECT * FROM tbl_customers WHERE customerName LIKE @SearchText OR contactNumber LIKE @SearchText";

                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Bind filtered data to DataGrid
                        customersDG.ItemsSource = dataTable.DefaultView;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching data: " + ex.Message);
                }
            }
            else
            {
                // If the search box is empty, reload all data
                LoadCustomersData();
            }
        }

        private void customersDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
