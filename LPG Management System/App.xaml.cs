using LPG_Management_System.Models;
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

            try
            {
                using (var context = new DataContext())
                {
                    DatabaseFacade facade = new DatabaseFacade(context);
                    facade.EnsureCreated(); // Creates the database if it doesn't exist
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
