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
    /// Interaction logic for inventoryUC.xaml
    /// </summary>
    public partial class inventoryUC : UserControl
    {
        private readonly string connectionString = "server=localhost;database=db_lpgpos;user=root;";
        public inventoryUC()
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
                    string query = "SELECT * FROM tbl_inventory";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind data to DataGrid
                    inventoryDG.ItemsSource = dataTable.DefaultView;
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
            inventoryCRUD inventoryCRUD = new inventoryCRUD();
            bool? dialogResult = inventoryCRUD.ShowDialog();

            // Check if the dialog was successful (e.g., a new record was added)
            if (dialogResult == true) // Assuming `true` is returned when a record is added
            {
                // Refresh the data in the DataGrid
                LoadCustomersData();
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
