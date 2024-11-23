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
            MainContent.Content = new settingsUC();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new dashboardUC();
        }
    }
}