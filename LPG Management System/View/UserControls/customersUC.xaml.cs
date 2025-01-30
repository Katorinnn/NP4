using LPG_Management_System.Models;
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
    public partial class customersUC : UserControl
    {
        public customersUC()
        {
            InitializeComponent();
            LoadCustomersData();
            customersDG.CanUserReorderColumns = false;
        }



        private void LoadCustomersData()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var customers = context.tbl_customers.ToList(); 

                    if (customersDG != null)
                    {
                        customersDG.ItemsSource = customers; 
                    }
                    else
                    {
                        Console.WriteLine("customersDG is null.");
                    }
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

            if (dialogResult == true) 
            {
                LoadCustomersData();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string searchText = textBox.Text.Trim().ToLower();

            if (!string.IsNullOrEmpty(searchText) && searchText != "search here")
            {
                try
                {
                    using (var context = new DataContext())
                    {
                        var filteredCustomers = context.tbl_customers
                    .Where(c =>
                            c.CustomerName.ToLower().Contains(searchText) ||
                            c.ContactNumber.ToLower().Contains(searchText) ||
                            c.Address.ToLower().Contains(searchText) ||
                            c.TankClassification.ToLower().Contains(searchText))
                        .ToList();

                        customersDG.ItemsSource = filteredCustomers; 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching data: " + ex.Message);
                }
            }
            else
            {
                LoadCustomersData();
            }
        }
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int selectedcustomersID = Convert.ToInt32(btn.Tag);

                customerUpdate update = new customerUpdate(selectedcustomersID);
                update.ShowDialog(); 
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int id = Convert.ToInt32(btn.Tag);  

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new DataContext())
                    {
                        var customer = context.tbl_customers.FirstOrDefault(c => c.CustomerID == id);
                        if (customer != null)
                        {
                            context.tbl_customers.Remove(customer); 
                            context.SaveChanges(); 
                            LoadCustomersData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting item: " + ex.Message);
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBox.Text == "Search here")
            {
                TextBox.Text = string.Empty;
                TextBox.Foreground = Brushes.Black; 
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox.Text))
            {
                TextBox.Text = "Search here";
                TextBox.Foreground = Brushes.Gray; 
            }
        }

    }
}