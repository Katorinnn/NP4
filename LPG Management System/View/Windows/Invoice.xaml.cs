using LPG_Management_System.View.UserControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using iTextSharp.text;
using iTextSharp.text.pdf;
using static LPG_Management_System.Dashboard;
using System;
using LPG_Management_System.Models;

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
            try
            {
                // Fetch company details
                var companyDetails = GetCompanyDetails();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    var items = InvoiceDataGrid.ItemsSource as IEnumerable<pointofsaleUC.ReceiptItem>;
                    int numberOfItems = items?.Count() ?? 0;

                    // Set a fixed height per item and add padding
                    float itemHeight = 15f;  // Height for each item in the receipt
                    float paddingHeight = 100f;  // Padding for header, footer, etc.
                    float totalHeight = Math.Max(itemHeight * numberOfItems + paddingHeight, 320f);  // Ensure a minimum height

                    float width = 200f;
                    iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(width, totalHeight);

                    iTextSharp.text.Document doc = new iTextSharp.text.Document(pageSize, 5f, 5f, 5f, 5f);
                    PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                    writer.CloseStream = false;

                    doc.Open();

                    // Add Logo (fetched from database)
                    if (companyDetails != null && companyDetails.Logo != null && companyDetails.Logo.Length > 0)
                    {
                        using (MemoryStream logoStream = new MemoryStream(companyDetails.Logo))
                        {
                            // Load the image into a Bitmap
                            using (var originalImage = System.Drawing.Image.FromStream(logoStream))
                            {
                                // Convert the image to grayscale
                                using (var grayscaleImage = new System.Drawing.Bitmap(originalImage.Width, originalImage.Height))
                                {
                                    using (var g = System.Drawing.Graphics.FromImage(grayscaleImage))
                                    {
                                        var colorMatrix = new System.Drawing.Imaging.ColorMatrix(
                                            new float[][]
                                            {
                            new float[] {0.3f, 0.3f, 0.3f, 0, 0},
                            new float[] {0.59f, 0.59f, 0.59f, 0, 0},
                            new float[] {0.11f, 0.11f, 0.11f, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {0, 0, 0, 0, 1}
                                            });

                                        var attributes = new System.Drawing.Imaging.ImageAttributes();
                                        attributes.SetColorMatrix(colorMatrix);

                                        g.DrawImage(
                                            originalImage,
                                            new System.Drawing.Rectangle(0, 0, grayscaleImage.Width, grayscaleImage.Height),
                                            0, 0, originalImage.Width, originalImage.Height,
                                            System.Drawing.GraphicsUnit.Pixel,
                                            attributes
                                        );
                                    }

                                    // Save the grayscale image into a MemoryStream for iTextSharp
                                    using (var grayscaleStream = new MemoryStream())
                                    {
                                        grayscaleImage.Save(grayscaleStream, System.Drawing.Imaging.ImageFormat.Png);
                                        grayscaleStream.Position = 0;

                                        // Add the grayscale image to the PDF
                                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(grayscaleStream);
                                        logo.ScaleToFit(100f, 100f);
                                        logo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                                        doc.Add(logo);
                                    }
                                }
                            }
                        }
                    }



                    // Header (dynamic data)
                    Font headerFont = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);
                    Font normalFont = new Font(Font.FontFamily.HELVETICA, 8f, Font.NORMAL);

                    if (companyDetails != null)
                    {
                        doc.Add(new iTextSharp.text.Paragraph(companyDetails.CompanyName, headerFont) { Alignment = Element.ALIGN_CENTER });
                        doc.Add(new iTextSharp.text.Paragraph(companyDetails.CompanyAddress, normalFont) { Alignment = Element.ALIGN_CENTER });
                        doc.Add(new iTextSharp.text.Paragraph(companyDetails.CompanyEmail, normalFont) { Alignment = Element.ALIGN_CENTER });
                        doc.Add(new iTextSharp.text.Paragraph("---------------------------------------------", normalFont) { Alignment = Element.ALIGN_CENTER });
                    }

                    // Order Details
                    doc.Add(new iTextSharp.text.Paragraph($"Order ID: {12345}", normalFont) { Alignment = Element.ALIGN_CENTER });
                    doc.Add(new iTextSharp.text.Paragraph($"Cashier: John Doe", normalFont) { Alignment = Element.ALIGN_CENTER });
                    doc.Add(new iTextSharp.text.Paragraph($"Payment Mode: CASH", normalFont) { Alignment = Element.ALIGN_CENTER });
                    doc.Add(new iTextSharp.text.Paragraph("---------------------------------------------", normalFont) { Alignment = Element.ALIGN_CENTER });

                    // Items
                    PdfPTable table = new PdfPTable(2)
                    {
                        WidthPercentage = 100
                    };
                    table.SetWidths(new float[] { 6f, 4f });

                    table.AddCell(new PdfPCell(new Phrase("Items", headerFont)) { Border = 0, HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase("Total", headerFont)) { Border = 0, HorizontalAlignment = PdfPCell.ALIGN_CENTER });

                    //var items = InvoiceDataGrid.ItemsSource as IEnumerable<pointofsaleUC.ReceiptItem>;
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            table.AddCell(new PdfPCell(new Phrase($"{item.Quantity} {item.Brand} {item.Size} ({item.Price:F2})", normalFont)) { Border = 0, HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                            table.AddCell(new PdfPCell(new Phrase($"{item.Total:F2}", normalFont)) { Border = 0, HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        }
                    }

                    doc.Add(table);

                    // Totals
                    doc.Add(new iTextSharp.text.Paragraph("---------------------------------------------", normalFont) { Alignment = Element.ALIGN_CENTER });
                    doc.Add(new iTextSharp.text.Paragraph($"TOTAL: {TotalAmountText.Text}", headerFont) { Alignment = Element.ALIGN_CENTER });
                    doc.Add(new iTextSharp.text.Paragraph($"Date: {DateTime.Now:yyyy-MM-dd}", normalFont) { Alignment = Element.ALIGN_CENTER });

                    // Footer
                    doc.Add(new iTextSharp.text.Paragraph("Thank you! Please come again.", normalFont) { Alignment = Element.ALIGN_CENTER });
                    doc.Add(new iTextSharp.text.Paragraph("** This is a computer-generated receipt.", normalFont) { Alignment = Element.ALIGN_CENTER });

                    doc.Close();
                    memoryStream.Position = 0;

                    // Save the PDF to a temporary location
                    string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
                    File.WriteAllBytes(tempFilePath, memoryStream.ToArray());

                    // Open the PDF for preview
                    Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });

                    Task.Run(() =>
                    {
                        Thread.Sleep(50000);
                        if (File.Exists(tempFilePath))
                        {
                            File.Delete(tempFilePath);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating receipt: {ex.Message}");
            }
        }


        private CompanyTable GetCompanyDetails()
        {
            using (var context = new DataContext())
            {
                // Assuming there is only one company record
                return context.tbl_company.FirstOrDefault();
            }
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
