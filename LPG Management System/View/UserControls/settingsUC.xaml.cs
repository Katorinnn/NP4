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
    /// Interaction logic for settingsUC.xaml
    /// </summary>
    public partial class settingsUC : UserControl
    {
        public settingsUC()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}
