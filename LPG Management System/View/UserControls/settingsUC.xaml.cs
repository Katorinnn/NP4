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
using LPG_Management_System.Models;
using Microsoft.Win32;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LPG_Management_System.View.UserControls
{
    /// <summary>
    /// Interaction logic for trySettingsUC.xaml
    /// </summary>
    /// 

    public partial class settingsUC : UserControl
    {
        private CompanyTable originalCompanyData;
        private AdminTable originalAdminData;
        public settingsUC()
        {
            InitializeComponent();
            LoadCompanyLogo();
            LoadCompanyData();
            LoadPrivacySettings();
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                // Debug your logic here
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Update the logoPlaceholder with the selected image
                    string selectedFilePath = openFileDialog.FileName;
                    logoPlaceholder.Source = new BitmapImage(new Uri(selectedFilePath));

                    // Convert the image to byte[] for storing in the database
                    byte[] imageBytes = File.ReadAllBytes(selectedFilePath);

                    // Update the logo in the company table using Entity Framework
                    using (var context = new DataContext())  // Replace with your actual DbContext
                    {
                        var company = context.tbl_company.FirstOrDefault(c => c.CompanyID == 1); // Replace with dynamic company ID
                        if (company != null)
                        {
                            company.Logo = imageBytes;  // Save the byte[] in the Logo field
                            context.SaveChanges(); // Save the changes to the database
                        }
                    }

                    // Refresh the logo to reflect changes immediately after save
                    LoadCompanyLogo();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void LoadCompanyLogo()
        {
            try
            {
                using (var context = new DataContext())  // Replace with your actual DbContext
                {
                    var company = context.tbl_company.FirstOrDefault(c => c.CompanyID == 1); // Replace with dynamic admin ID
                    if (company != null)
                    {
                        if (company.Logo != null && company.Logo.Length > 0)
                        {
                            // Convert byte[] to BitmapImage
                            using (var ms = new MemoryStream(company.Logo))
                            {
                                var bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.StreamSource = ms;
                                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Ensure the image is fully loaded
                                bitmap.EndInit();
                                logoPlaceholder.Source = bitmap;  // Set the image source to the logoPlaceholder
                            }
                        }
                        else
                        {
                            MessageBox.Show("Logo data is empty or null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Admin not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading logo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCompanyData()
        {
            try
            {
                using (var context = new DataContext())
                {
                    originalCompanyData = context.tbl_company.FirstOrDefault();
                    if (originalCompanyData != null)
                    {
                        // Populate the fields with existing data
                        NameTextBox.Text = originalCompanyData.CompanyName;
                        AddressTextBox.Text = originalCompanyData.CompanyAddress;
                        ContactTextBox.Text = originalCompanyData.CompanyContact;
                        EmailTextBox.Text = originalCompanyData.CompanyEmail;
                    }
                    else
                    {
                        // If no data exists, clear the fields
                        NameTextBox.Clear();
                        AddressTextBox.Clear();
                        ContactTextBox.Clear();
                        EmailTextBox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading company data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            NameTextBox.IsReadOnly = false;
            AddressTextBox.IsReadOnly = false;
            ContactTextBox.IsReadOnly = false;
            SaveButton.IsEnabled = true;
            CancelButton.IsEnabled = true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var updatedCompany = new CompanyTable
            {
                CompanyName = NameTextBox.Text,
                CompanyAddress = AddressTextBox.Text,
                CompanyContact = ContactTextBox.Text,
                CompanyEmail = EmailTextBox.Text
            };

            if (originalCompanyData != null)
            {
                // If data already exists, update it
                SaveCompanyDataToDatabase(updatedCompany);
            }
            else
            {
                // If no data exists, insert new data into the database
                AddCompanyDataToDatabase(updatedCompany);
            }

            // Reload the company data to reflect changes
            LoadCompanyData();

            // Disable editing controls
            DisableEditing();
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (originalCompanyData != null)
            {
                // Revert to the original data if available
                NameTextBox.Text = originalCompanyData.CompanyName;
                AddressTextBox.Text = originalCompanyData.CompanyAddress;
                ContactTextBox.Text = originalCompanyData.CompanyContact;
                EmailTextBox.Text = originalCompanyData.CompanyEmail;
            }
            else
            {
                // Clear the fields if no data exists
                NameTextBox.Clear();
                AddressTextBox.Clear();
                ContactTextBox.Clear();
                EmailTextBox.Clear();
            }

            DisableEditing();
        }


        private void SaveCompanyDataToDatabase(CompanyTable company)
        {
            using (var context = new DataContext())
            {
                var existingCompany = context.tbl_company.FirstOrDefault();
                if (existingCompany != null)
                {
                    existingCompany.CompanyName = company.CompanyName;
                    existingCompany.CompanyAddress = company.CompanyAddress;
                    existingCompany.CompanyContact = company.CompanyContact;
                    existingCompany.CompanyEmail = company.CompanyEmail;
                    context.SaveChanges();
                }
            }
        }

        private void AddCompanyDataToDatabase(CompanyTable company)
        {
            using (var context = new DataContext())
            {
                context.tbl_company.Add(company);
                context.SaveChanges();
            }
        }

        private void DisableEditing()
        {
            NameTextBox.IsReadOnly = true;
            AddressTextBox.IsReadOnly = true;
            ContactTextBox.IsReadOnly = true;
            EmailTextBox.IsReadOnly = true;
            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
        }
        //privacy settings
        private void LoadPrivacySettings()
        {
            try
            {
                using (var context = new DataContext())
                {
                    originalAdminData = context.tbl_admin.FirstOrDefault();
                    if (originalAdminData != null)
                    {
                        // Populate the fields with existing data
                        UsernameTextBox.Text = originalAdminData.username;
                        PasswordTextBox.Password = originalAdminData.password;
                    }
                    else
                    {
                        // If no data exists, clear the fields
                        UsernameTextBox.Clear();
                        PasswordTextBox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading privacy settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EditPrivacyButton_Click(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.IsReadOnly = false;
            PasswordTextBox.IsEnabled = true;
            SavePrivacyButton.IsEnabled = true;
            CancelPrivacyButton.IsEnabled = true;
        }
        private void SavePrivacyButton_Click(object sender, RoutedEventArgs e)
        {
            var updatedAdmin = new AdminTable
            {
                username = UsernameTextBox.Text,
                password = PasswordTextBox.Password
            };

            if (originalAdminData != null)
            {
                // If data already exists, update it
                SaveAdminDataToDatabase(updatedAdmin);
            }
            else
            {
                // If no data exists, insert new data into the database
                AddAdminDataToDatabase(updatedAdmin);
            }

            // Reload the privacy settings to reflect changes
            LoadPrivacySettings();

            // Disable privacy editing controls
            DisablePrivacyEditing();
        }

        private void CancelPrivacyButton_Click(object sender, RoutedEventArgs e)
        {
            if (originalAdminData != null)
            {
                // Revert to the original data if available
                UsernameTextBox.Text = originalAdminData.username;
                PasswordTextBox.Password = originalAdminData.password;
            }
            else
            {
                // Clear the fields if no data exists
                UsernameTextBox.Clear();
                PasswordTextBox.Clear();
            }

            DisablePrivacyEditing();
        }
        private void SaveAdminDataToDatabase(AdminTable admin)
        {
            using (var context = new DataContext())
            {
                var existingAdmin = context.tbl_admin.FirstOrDefault();
                if (existingAdmin != null)
                {
                    existingAdmin.username = admin.username;
                    existingAdmin.password = admin.password;
                    context.SaveChanges();
                }
            }
        }
        private void AddAdminDataToDatabase(AdminTable admin)
        {
            using (var context = new DataContext())
            {
                context.tbl_admin.Add(admin);
                context.SaveChanges();
            }
        }
        private void DisablePrivacyEditing()
        {
            UsernameTextBox.IsReadOnly = true;
            PasswordTextBox.IsEnabled = false;
            SavePrivacyButton.IsEnabled = false;
            CancelPrivacyButton.IsEnabled = false;
        }

    }
}

