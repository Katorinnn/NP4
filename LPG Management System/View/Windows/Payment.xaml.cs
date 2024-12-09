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

namespace LPG_Management_System.View.Windows
{
    /// <summary>
    /// Interaction logic for Payment.xaml
    /// </summary>
    public partial class Payment : Window
    {
        private double totalPrice;
        public double PaymentAmount { get; private set; } // Holds the entered amount
        public Payment(double totalPrice)
        {
            InitializeComponent();
            this.totalPrice = totalPrice;
            TotalAmountLabel.Content = $"Total: ₱{totalPrice:F2}";
        }

        private void amountBtn_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(amounttxtBox.Text, out double amount) && amount > 0)
            {
                PaymentAmount = amount;
                this.DialogResult = true; // Indicate successful payment entry
                this.Close(); // Close the payment window
            }
            else
            {
                MessageBox.Show("Please enter a valid payment amount.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        //public void UpdateTotalPrice(double newTotalPrice)
        //{
        //    totalPrice = newTotalPrice; // Update the internal price
        //    TotalAmountLabel.Content = $"Total: ₱{totalPrice:F2}"; // Update the label to reflect the new total
        //}

        private void amounttxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
