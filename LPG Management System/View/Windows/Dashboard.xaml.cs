using LPG_Management_System.View.UserControls;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LPG_Management_System
{
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();

        }


        private void inventoryBtn_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new inventoryUC();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new customersUC();

        }

        private void posBtn_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new pointofsaleUC();
        }

        private void reportsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new reportsUC();
        }

        private void settingsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new trySettingsUC();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new dashboardUC();

        }

        private void dashboardBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            dashboardBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8ecae6"));
        }

        private void dashboardBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            dashboardBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2C3E50"));

        }

        private void custumerBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            custumerBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8ecae6"));
        }

        private void custumerBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            custumerBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2C3E50"));
        }

        private void inventoryBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            inventoryBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8ecae6"));
        }

        private void inventoryBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            inventoryBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2C3E50"));
        }

        private void posBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            posBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8ecae6"));
        }

        private void posBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            posBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2C3E50"));
        }

        private void reportsBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            reportsBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8ecae6"));
        }

        private void reportsBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            reportsBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2C3E50"));
        }

        private void settingsBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            settingsBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8ecae6"));
        }

        private void settingBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            settingsBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2C3E50"));
        }

        private void dashboardUC_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset the Tag property for all buttons in the sidebar
            foreach (var child in SidebarPanel.Children)
            {
                if (child is Button btn)
                {
                    btn.Tag = null; // Clear active state
                }
            }


            // Reset other buttons
            custumerBtn.Tag = null;
            inventoryBtn.Tag = null;
            posBtn.Tag = null;
            reportsBtn.Tag = null;
            settingsBtn.Tag = null;

            // Set active button
            
            // Set the clicked button's Tag to "Active"
            if (sender is Button clickedButton)
                {
                    clickedButton.Tag = "Active";

                    // Perform navigation based on the clicked button
                    if (clickedButton.Name == "dashboardBtn")
                    {
                        MainContent.Content = new dashboardUC(); // Load the dashboard user control
                    }
                    else if (clickedButton.Name == "custumerBtn")
                    {
                        MainContent.Content = new customersUC(); // Load the customers user control
                    }
                    else if (clickedButton.Name == "inventoryBtn")
                    {
                        MainContent.Content = new inventoryUC(); // Load the inventory user control
                    }
                    else if (clickedButton.Name == "posBtn")
                    {
                        MainContent.Content = new pointofsaleUC(); // Load the point of sale user control
                    }
                    else if (clickedButton.Name == "reportsBtn")
                    {
                        MainContent.Content = new reportsUC(); // Load the reports user control
                    }
                    else if (clickedButton.Name == "settingsBtn")
                    {
                        MainContent.Content = new trySettingsUC(); // Load the settings user control
                    }
                }
        }
    }
}
