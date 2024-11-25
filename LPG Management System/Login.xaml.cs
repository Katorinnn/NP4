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
using static System.Net.Mime.MediaTypeNames;

namespace LPG_Management_System
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = unametxtBox.Text;
            string password = pwordBox.Password;

            if (ValidateLogin(username, password))
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                Dashboard dashboard = new Dashboard();
                dashboard.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateLogin(string username, string password)
        {
            string connectionString = "server=localhost;database=db_lpgpos;user=root;";
            string query = "SELECT COUNT(*) FROM tbl_admin WHERE username = @username AND password = @password";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        int result = Convert.ToInt32(cmd.ExecuteScalar());
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void unametxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void loginBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            loginBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8ecae6"));
        }

        private void loginBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            loginBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("white"));
        }
    }
}
