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
    /// Interaction logic for reportsUC.xaml
    /// </summary>
    public partial class reportsUC : UserControl
    {
        private readonly string connectionString = "server=localhost;database=db_lpgpos;user=root;";
        public reportsUC()
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
                    string query = "SELECT * FROM tbl_reports";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind data to DataGrid
                    reportsDG.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
        }

        private void reportsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}
