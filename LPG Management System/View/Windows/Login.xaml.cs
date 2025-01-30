using LPG_Management_System.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LPG_Management_System
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = unametxtBox.Text;
            string password = pwordBox.Password;

            ResetValidation();

            if (ValidateLogin(username, password))
            {
                Dashboard dashboard = new Dashboard();
                dashboard.Show();
                this.Close();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    unametxtBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    unameError.Text = "Username cannot be empty";
                    unameError.Visibility = Visibility.Visible;
                }
                else
                {
                    unametxtBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    unameError.Text = "Invalid username";
                    unameError.Visibility = Visibility.Visible;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    pwordBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    pwordError.Text = "Password cannot be empty";
                    pwordError.Visibility = Visibility.Visible;
                }
                else
                {
                    pwordBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    pwordError.Text = "Invalid password";
                    pwordError.Visibility = Visibility.Visible;
                }
            }
        }

        private void ResetValidation()
        {
            unametxtBox.BorderBrush = System.Windows.Media.Brushes.Gray;
            unameError.Visibility = Visibility.Collapsed;
            pwordBox.BorderBrush = System.Windows.Media.Brushes.Gray;
            pwordError.Visibility = Visibility.Collapsed;
        }

        private bool ValidateLogin(string username, string password)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var admin = context.tbl_admin
                        .FirstOrDefault(a => a.username == username && a.password == password);

                    return admin != null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


        private void loginBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            loginBtn.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#8ecae6"));
        }

        private void loginBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            loginBtn.Background = System.Windows.Media.Brushes.Transparent;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

