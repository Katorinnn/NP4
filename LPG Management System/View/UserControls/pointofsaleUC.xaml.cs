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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
        }

        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            CoastalPanel.Visibility = Visibility.Collapsed;
            GazLitePanel.Visibility = Visibility.Collapsed;
            SolanePanel.Visibility = Visibility.Collapsed;
            RegascoPanel.Visibility = Visibility.Collapsed;

            // Show the corresponding panel
            if (sender is Button button)
            {
                switch (button.Content.ToString())
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void oneBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
