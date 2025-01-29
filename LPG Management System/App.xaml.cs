using LPG_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Configuration;
using System.Data;
using System.Windows;

namespace LPG_Management_System
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var iconUri = new Uri("pack://application:,,,/Resources/NP4LOGO.ico", UriKind.Absolute);
            var iconImage = new System.Windows.Media.Imaging.BitmapImage(iconUri);

            foreach (Window window in Application.Current.Windows)
            {
                window.Icon = iconImage;
            }

            try
            {
                using (var context = new DataContext())
                {
                    context.Database.Migrate(); // Applies pending migrations
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing the database: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(); // Exit the application if the database setup fails
            }

            try
            {
                using (var context = new DataContext())
                {
                    context.Database.Migrate(); // Applies pending migrations
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing the database: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(); // Exit the application if the database setup fails
            }
        }
    }

}
