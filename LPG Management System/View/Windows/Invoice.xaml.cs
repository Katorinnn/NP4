using LPG_Management_System.View.UserControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace LPG_Management_System.View.Windows
{
    public partial class Invoice : Window
    {
        private ObservableCollection<ReceiptItem> receiptItems = new ObservableCollection<ReceiptItem>();

        public class ReceiptItem
        {
            public string Brand { get; set; }
            public string Size { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
            public double Total => Price * Quantity;
        }

        public Invoice(List<pointofsaleUC.ReceiptItem> receiptItems, string customerAddress, double totalAmount, double amountPaid, double change)
        {
            InitializeComponent();

            // Bind receipt items to DataGrid
            InvoiceDataGrid.ItemsSource = receiptItems;

            // Display customer details
            CustomerAddressText.Text = $"Address: {customerAddress}";

            // Set totals
            TotalAmountText.Text = $"₱{totalAmount:F2}";
            AmountPaidText.Text = $"₱{amountPaid:F2}";
            ChangeText.Text = $"₱{change:F2}";
        }




        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            // Generate a FixedDocument
            FixedDocument fixedDoc = GenerateFixedDocument();

            // Show Print Preview Window
            PrintPreview previewWindow = new PrintPreview(fixedDoc);
            previewWindow.ShowDialog();

            // Optional: Print the FixedDocument after preview
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(fixedDoc.DocumentPaginator, "Invoice Print");
            }
        }

        private FixedDocument GenerateFixedDocument()
        {
            FixedDocument fixedDoc = new FixedDocument();

            // Create a page
            FixedPage page = new FixedPage
            {
                Width = 600,  // A4 width in px (96 DPI)
                Height = 700 // A4 height in px (96 DPI)
            };

            // Header section (Company and Invoice Details)
            Grid headerGrid = new Grid
            {
                Margin = new Thickness(20, 20, 20, 0)
            };
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Left side: Company Info
            TextBlock companyInfo = new TextBlock
            {
                Text = "NP4 GAS SERVICE\nC 3/4 KABIR NAGAR\nJODHPUR ROAD \"SHAHDARA,\"\nJODHPUR 302001",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Left
            };
            Grid.SetColumn(companyInfo, 0);
            headerGrid.Children.Add(companyInfo);

            // Right side: Invoice Info
            TextBlock invoiceInfo = new TextBlock
            {
                Text = $"Tax Invoice\nDate: {DateTime.Now:dd/MM/yyyy}\nInvoice No: 12345",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Right
            };
            Grid.SetColumn(invoiceInfo, 1);
            headerGrid.Children.Add(invoiceInfo);

            page.Children.Add(headerGrid);

            // Line separator
            Border separator = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(0, 1, 0, 0),
                Margin = new Thickness(20, 120, 20, 0)
            };
            page.Children.Add(separator);

            // Invoice Items Table
            Grid tableGrid = new Grid
            {
                Margin = new Thickness(20, 140, 20, 0)
            };

            // Define Columns
            tableGrid.ColumnDefinitions.Add(new ColumnDefinition());
            tableGrid.ColumnDefinitions.Add(new ColumnDefinition());
            tableGrid.ColumnDefinitions.Add(new ColumnDefinition());
            tableGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Add Table Header
            AddTableRow(tableGrid, 0, "Product", "Size", "Price", "Total", true);

            // Add Table Data
            int row = 1;
            var items = InvoiceDataGrid.ItemsSource as IEnumerable<pointofsaleUC.ReceiptItem>;
            if (items != null)
            {
                foreach (var receiptItem in items)
                {
                    AddTableRow(
                        tableGrid,
                        row++,
                        receiptItem.Brand,
                        receiptItem.Size,
                        $"₱{receiptItem.Price:F2}",
                        $"₱{receiptItem.Total:F2}",
                        false
                    );
                }
            }


            page.Children.Add(tableGrid);

            // Totals Section
            TextBlock totals = new TextBlock
            {
                Text = $"Total: {TotalAmountText.Text}\nAmount Paid: {AmountPaidText.Text}\nNet Due: {ChangeText.Text}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(20, 160 + (row * 30), 20, 0)
            };
            page.Children.Add(totals);

            // Footer
            TextBlock footer = new TextBlock
            {
                Text = "** This is a computer-generated document and needs no signatures.\n** Stay Home, Stay Safe.",
                FontSize = 12,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(20, 900, 20, 0),
                TextAlignment = TextAlignment.Center
            };
            page.Children.Add(footer);

            // Add the page to the FixedDocument
            PageContent pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(page);
            fixedDoc.Pages.Add(pageContent);

            return fixedDoc;
        }

        // Helper Method to Add Rows to Table
        private void AddTableRow(Grid grid, int row, string col1, string col2, string col3, string col4, bool isHeader)
        {
            grid.RowDefinitions.Add(new RowDefinition());

            var style = isHeader ? FontWeights.Bold : FontWeights.Normal;

            AddTextBlockToGrid(grid, col1, 0, row, style);
            AddTextBlockToGrid(grid, col2, 1, row, style);
            AddTextBlockToGrid(grid, col3, 2, row, style);
            AddTextBlockToGrid(grid, col4, 3, row, style);
        }

        private void AddTextBlockToGrid(Grid grid, string text, int column, int row, FontWeight fontWeight)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                FontSize = 14,
                FontWeight = fontWeight,
                Margin = new Thickness(5),
                TextAlignment = TextAlignment.Left
            };
            Grid.SetColumn(textBlock, column);
            Grid.SetRow(textBlock, row);
            grid.Children.Add(textBlock);
        }



    }

}
