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

namespace LPG_Management_System
{
    /// <summary>
    /// Interaction logic for PointOfSale.xaml
    /// </summary>
    public partial class PointOfSale : Window
    {
        public PointOfSale()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dashboard dashboard = new Dashboard();  
            dashboard.Show();
            this.Close();
        }

        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide all product panels (kg options)
            CoastalPanel.Visibility = Visibility.Collapsed;
            GazLitePanel.Visibility = Visibility.Collapsed;
            SolanePanel.Visibility = Visibility.Collapsed;
            RegascoPanel.Visibility = Visibility.Collapsed;

            // Check which product button was clicked and show the corresponding kg buttons
            switch ((sender as Button).Content.ToString())
            {
                case "Coastal":
                    // Show kg options for Coastal
                    CoastalPanel.Visibility = Visibility.Visible;
                    break;

                case "Gaz Lite":
                    // Show kg options for Gaz Lite
                    GazLitePanel.Visibility = Visibility.Visible;
                    break;

                case "Solane":
                    // Show kg options for Solane
                    SolanePanel.Visibility = Visibility.Visible;
                    break;

                case "Regasco":
                    // Show kg options for Regasco
                    RegascoPanel.Visibility = Visibility.Visible;
                    break;

                default:
                    // Default action in case of invalid selection (optional)
                    break;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void oneBtn_Click(object sender, RoutedEventArgs e)
        {
            displayText.Content = "1";
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
