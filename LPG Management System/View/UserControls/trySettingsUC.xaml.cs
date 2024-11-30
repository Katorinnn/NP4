using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;

namespace LPG_Management_System.View.UserControls
{
    /// <summary>
    /// Interaction logic for trySettingsUC.xaml
    /// </summary>
    public partial class trySettingsUC : UserControl
    {
        public trySettingsUC()
        {
            InitializeComponent();

            //if (logoPlaceholder.Source = null)
            //{
            //    BitmapImage image = new BitmapImage();
            //    image.BeginInit();
            //    image.UriSource = new Uri("");
            //    image.EndInit();
            //    logoPlaceholder.Source = image;
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    //BitmapImage image = new BitmapImage(new Uri(openFileDialog.FileName));
                    logoPlaceholder.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    //byte[] imageData = File.ReadAllBytes(openFileDialog.Filename);
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}

