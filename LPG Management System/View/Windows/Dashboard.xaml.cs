using LPG_Management_System.Models;
using LPG_Management_System.View.UserControls;
using System.IO;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LPG_Management_System
{
    public partial class Dashboard : Window
    {

        public class Company
        {
            public BitmapImage Logo { get; set; }
        }

        public Dashboard()
        {
            InitializeComponent();

            SetCompanyLogo();  // Set the logo initially when the window is created
        }

        private void SetCompanyLogo()
        {
            var company = GetCompanyFromDatabase();
            this.DataContext = company;  // Set the data context to bind the LogoImagePath
        }

        private Company GetCompanyFromDatabase()
        {
            var company = new CompanyTable
            {
                Logo = GetLogoFromDatabase() // Fetch the logo byte array from the database
            };

            return new Company
            {
                Logo = ConvertByteArrayToBitmapImage(company.Logo) // Convert byte[] to BitmapImage
            };
        }

        private byte[] GetLogoFromDatabase()
        {
            // Fetch the logo byte array from the database
            var dbContext = new DataContext(); // Replace with your actual DbContext
            var company = dbContext.tbl_company.FirstOrDefault(); // Fetch the company

            return company?.Logo;
        }

        private BitmapImage ConvertByteArrayToBitmapImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null;

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
{
    // Reload the logo when any button is clicked
    SetCompanyLogo();

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
            MainContent.Content = new settingsUC(); // Load the settings user control
        }
    }
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
            MainContent.Content = new settingsUC();

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
        
    }
}
