using LPG_Management_System.View.UserControls;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

        private void settingBtn_Click(object sender, RoutedEventArgs e)
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

        private void settingBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            settingBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8ecae6"));
        }

        private void settingBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            settingBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2C3E50"));
        }

        private void dashboardUC_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
