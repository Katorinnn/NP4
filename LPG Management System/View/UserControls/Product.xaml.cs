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
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class Product : UserControl
    {
        public Product()
        {
            InitializeComponent();
        }

        private void Product_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is pointofsaleUC mainControl)
            {
                // Pass the product details to the parent control
                mainControl.AddToReceipt(new pointofsaleUC.ReceiptItem
                {
                    Brand = BrandLabel.Content.ToString(),
                    Price = double.Parse(PriceLabel.Content.ToString().Replace("₱", "")),
                    Size = SizeLabel?.Content?.ToString() // Optional: Include Size if applicable
                });
            }
        }

    }
}
