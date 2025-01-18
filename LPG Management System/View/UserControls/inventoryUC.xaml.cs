using LPG_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LPG_Management_System.View.UserControls
{
    public partial class inventoryUC : UserControl
    {
        private readonly DataContext _context;

        private ContextMenu FilterContextMenu = new ContextMenu();
        public inventoryUC()
        {
            InitializeComponent();
            _context = new DataContext();
            LoadCustomersData();
            PopulateFilterMenu();
        }

        private void LoadCustomersData()
        {
            try
            {
                var inventoryData = _context.tbl_inventory.ToList();  // Correct DbSet reference
                inventoryDG.ItemsSource = inventoryData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = (sender as TextBox)?.Text;

            if (!string.IsNullOrEmpty(searchText))
            {
                try
                {
                    var filteredData = _context.tbl_inventory
                        .Where(i => i.TankID.ToString().Contains(searchText) || i.ProductName.Contains(searchText))
                        .ToList();
                    inventoryDG.ItemsSource = filteredData;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching data: " + ex.Message);
                }
            }
            else
            {
                LoadCustomersData();
            }
        }

        private void PopulateFilterMenu()
        {
            FilterContextMenu.Items.Clear();

            try
            {
                // Filter by Name
                MenuItem filterByNameItem = new MenuItem { Header = "Filter by Name" };
                var brands = _context.tbl_inventory.Select(i => i.ProductName).Distinct().ToList();
                foreach (var brand in brands)
                {
                    MenuItem brandItem = new MenuItem { Header = brand };
                    brandItem.Click += (sender, args) => ApplyFilter("BrandName", brand);
                    filterByNameItem.Items.Add(brandItem);
                }

                // Filter by Size
                MenuItem filterBySizeItem = new MenuItem { Header = "Filter by Size" };
                var sizes = _context.tbl_inventory.Select(i => i.Size).Distinct().ToList();
                foreach (var size in sizes)
                {
                    MenuItem sizeItem = new MenuItem { Header = size };
                    sizeItem.Click += (sender, args) => ApplyFilter("Size", size);
                    filterBySizeItem.Items.Add(sizeItem);
                }

                FilterContextMenu.Items.Add(filterByNameItem);
                FilterContextMenu.Items.Add(filterBySizeItem);
                FilterButton.ContextMenu = FilterContextMenu;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error populating filter menu: " + ex.Message);
            }
        }

        private void ApplyFilter(string column, string value)
        {
            try
            {
                var filteredData = column switch
                {
                    "BrandName" => _context.tbl_inventory.Where(i => i.ProductName == value).ToList(),
                    "Size" => _context.tbl_inventory.Where(i => i.Size == value).ToList(),
                    _ => throw new ArgumentException("Invalid filter column")
                };
                inventoryDG.ItemsSource = filteredData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying filter: " + ex.Message);
            }
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            inventoryCRUD inventoryCRUD = new inventoryCRUD(); // No parameter needed
            bool? dialogResult = inventoryCRUD.ShowDialog();

            if (dialogResult == true)
            {
                LoadCustomersData();
                PopulateFilterMenu();
            }
        }


        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int selectedTankID = Convert.ToInt32(btn.Tag);
                inventoryUpdate inventoryUpdate = new inventoryUpdate(selectedTankID);
                inventoryUpdate.ShowDialog();
                LoadCustomersData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int id = Convert.ToInt32(btn.Tag);

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var itemToDelete = _context.tbl_inventory.FirstOrDefault(i => i.TankID == id);
                    if (itemToDelete != null)
                    {
                        _context.tbl_inventory.Remove(itemToDelete);
                        _context.SaveChanges();
                        LoadCustomersData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting item: " + ex.Message);
                }
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCustomersData();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBox.Text == "Search here")
            {
                TextBox.Text = string.Empty;
                TextBox.Foreground = Brushes.Black; // Set text color to normal
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox.Text))
            {
                TextBox.Text = "Search here";
                TextBox.Foreground = Brushes.Gray; // Set text color to placeholder style
            }
        }

        public class InventoryTable
        {
            public int TankID { get; set; }
            public string ProductName { get; set; }
            public string Size { get; set; }
            public decimal Price { get; set; }
            public byte[] ProductImage { get; set; }
            public int Stock { get; set; } // New stock property
        }


    }
}
