using System;
using System.Windows;
using System.Windows.Controls;

namespace LPG_Management_System.View.UserControls
{
    /// <summary>
    /// Interaction logic for pointofsaleUC.xaml
    /// </summary>
    public partial class pointofsaleUC : UserControl
    {
        public pointofsaleUC()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the Dashboard window when this button is clicked
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
        }

        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide all panels initially
            CoastalPanel.Visibility = Visibility.Collapsed;
            GazLitePanel.Visibility = Visibility.Collapsed;
            SolanePanel.Visibility = Visibility.Collapsed;
            RegascoPanel.Visibility = Visibility.Collapsed;

            if (sender is Button button)
            {
                string selectedBrand = button.Content.ToString();
                BrandLabel.Content = $"Brand: {selectedBrand}";

                // Show the corresponding panel based on the selected brand
                switch (selectedBrand)
                {
                    case "Coastal":
                        CoastalPanel.Visibility = Visibility.Visible;
                        break;
                    case "Gaz Lite":
                        GazLitePanel.Visibility = Visibility.Visible;
                        break;
                    case "Solane":
                        SolanePanel.Visibility = Visibility.Visible;
                        break;
                    case "Regasco":
                        RegascoPanel.Visibility = Visibility.Visible;
                        break;
                }
            }
        }


        private void SizeButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the sender is a button
            if (sender is Button button)
            {
                string selectedSize = button.Content.ToString();
                SizeLabel.Content = $"Size: {selectedSize}";  // Update the size label based on the selected button
            }
        }
    }
}
