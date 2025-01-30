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

            SetCompanyLogo();  
        }

        private void SetCompanyLogo()
        {
            var company = GetCompanyFromDatabase();
            this.DataContext = company; 
        }

        private Company GetCompanyFromDatabase()
        {
            var company = new CompanyTable
            {
                Logo = GetLogoFromDatabase() 
            };

            return new Company
            {
                Logo = ConvertByteArrayToBitmapImage(company.Logo) 
            };
        }

        private byte[] GetLogoFromDatabase()
        {
            var dbContext = new DataContext(); 
            var company = dbContext.tbl_company.FirstOrDefault(); 

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
        SetCompanyLogo();

        foreach (var child in SidebarPanel.Children)
        {
            if (child is Button btn)
            {
                btn.Tag = null; 
            }
        }
            custumerBtn.Tag = null;
            inventoryBtn.Tag = null;
            posBtn.Tag = null;
            reportsBtn.Tag = null;
            settingsBtn.Tag = null;

        if (sender is Button clickedButton)
        {
            clickedButton.Tag = "Active";

            if (clickedButton.Name == "dashboardBtn")
            {
                MainContent.Content = new dashboardUC(); 
            }
            else if (clickedButton.Name == "custumerBtn")
            {
                MainContent.Content = new customersUC(); 
            }
            else if (clickedButton.Name == "inventoryBtn")
            {
                MainContent.Content = new inventoryUC(); 
            }
            else if (clickedButton.Name == "posBtn")
            {
                MainContent.Content = new pointofsaleUC(); 
            }
            else if (clickedButton.Name == "reportsBtn")
            {
                MainContent.Content = new reportsUC(); 
            }
            else if (clickedButton.Name == "settingsBtn")
            {
                MainContent.Content = new settingsUC(); 
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
        
    }
}
