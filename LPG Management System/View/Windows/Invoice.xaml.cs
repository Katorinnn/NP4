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

        private double _amountPaid;
        private double _change;


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

            _amountPaid = amountPaid;
            _change = change;

            InvoiceDataGrid.ItemsSource = receiptItems;

            CustomerAddressText.Text = $"Address: {customerAddress}";

            TotalAmountText.Text = $"₱ {totalAmount:F2}";
            AmountPaidText.Text = $"₱ {amountPaid:F2}";
            ChangeText.Text = $"₱ {change:F2}";
        
        }

        private string GenerateRandomOrderId()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();  
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            BaseFont customBaseFont = BaseFont.CreateFont("C:/Windows/Fonts/arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font customFont = new Font(customBaseFont, 10f, Font.NORMAL);
            try
            {
                var companyDetails = GetCompanyDetails();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    var items = InvoiceDataGrid.ItemsSource as IEnumerable<pointofsaleUC.ReceiptItem>;
                    int numberOfItems = items?.Count() ?? 0;

                    float itemHeight = 15f;  
                    float paddingHeight = 100f;  
                    float totalHeight = Math.Max(itemHeight * numberOfItems + paddingHeight, 500f);  

                    float width = 200f;
                    iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(width, totalHeight);

                    iTextSharp.text.Document doc = new iTextSharp.text.Document(pageSize, 5f, 5f, 5f, 5f);
                    PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                    writer.CloseStream = false;

                    doc.Open();

     
                    if (companyDetails != null && companyDetails.Logo != null && companyDetails.Logo.Length > 0)
                    {
                        using (MemoryStream logoStream = new MemoryStream(companyDetails.Logo))
                        {
                             using (var originalImage = System.Drawing.Image.FromStream(logoStream))
                            {
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
                                    using (var grayscaleStream = new MemoryStream())
                                    {
                                        grayscaleImage.Save(grayscaleStream, System.Drawing.Imaging.ImageFormat.Png);
                                        grayscaleStream.Position = 0;

                                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(grayscaleStream);
                                        logo.ScaleToFit(100f, 100f);
                                        logo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                                        doc.Add(logo);
                                    }
                                }
                            }
                        }
                    }

                    Font headerFont = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);
                    Font normalFont = new Font(Font.FontFamily.HELVETICA, 8f, Font.NORMAL);

                    if (companyDetails != null)
                    {
                        doc.Add(new iTextSharp.text.Paragraph(companyDetails.CompanyName, headerFont) { Alignment = Element.ALIGN_CENTER });
                        doc.Add(new iTextSharp.text.Paragraph(companyDetails.CompanyAddress, normalFont) { Alignment = Element.ALIGN_CENTER });
                        doc.Add(new iTextSharp.text.Paragraph($"Date: {DateTime.Now:yyyy-MM-dd}", normalFont) { Alignment = Element.ALIGN_CENTER });
                        doc.Add(new iTextSharp.text.Paragraph($"Time: {DateTime.Now:HH:mm:ss}", normalFont) { Alignment = Element.ALIGN_CENTER });
                        doc.Add(new iTextSharp.text.Paragraph(new string('-', 70), normalFont) { Alignment = Element.ALIGN_CENTER });
                    }

                    doc.Add(new iTextSharp.text.Paragraph($"Order ID: {GenerateRandomOrderId()}", normalFont) { Alignment = Element.ALIGN_CENTER });
                    doc.Add(new iTextSharp.text.Paragraph($"Cashier: Neil Agnes Pimentel", normalFont) { Alignment = Element.ALIGN_CENTER });
                    doc.Add(new iTextSharp.text.Paragraph($"Payment Mode: CASH", normalFont) { Alignment = Element.ALIGN_CENTER });
                    doc.Add(new iTextSharp.text.Paragraph(new string('-', 70), normalFont) { Alignment = Element.ALIGN_CENTER });

                    PdfPTable table = new PdfPTable(2)
                    {
                        WidthPercentage = 100
                    };

                    table.SetWidths(new float[] { 5f, 5f });  

                    PdfPCell itemHeaderCell = new PdfPCell(new Phrase("Items", headerFont))
                    {
                        Border = 0,
                        HorizontalAlignment = PdfPCell.ALIGN_LEFT,
                        PaddingLeft = 10f,  
                        PaddingRight = 10f  
                    };
                    table.AddCell(itemHeaderCell);

                    PdfPCell totalHeaderCell = new PdfPCell(new Phrase("Total", headerFont))
                    {
                        Border = 0,
                        HorizontalAlignment = PdfPCell.ALIGN_RIGHT,
                        PaddingLeft = 10f,  
                        PaddingRight = 10f  
                    };
                    table.AddCell(totalHeaderCell);

                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            PdfPCell itemCell = new PdfPCell(new Phrase($"{item.Quantity} {item.Brand} {item.Size} (₱ {item.Price:F2})", normalFont))
                            {
                                Border = 0,
                                HorizontalAlignment = PdfPCell.ALIGN_LEFT,
                                PaddingLeft = 10f,  
                                PaddingRight = 10f  
                            };
                            table.AddCell(itemCell);

                            PdfPCell totalCell = new PdfPCell(new Phrase($"₱ {item.Total:F2}", normalFont))
                            {
                                Border = 0,
                                HorizontalAlignment = PdfPCell.ALIGN_RIGHT,
                                PaddingLeft = 10f,  
                                PaddingRight = 10f  
                            };
                            table.AddCell(totalCell);
                        }
                    }

                    doc.Add(table);

                    PdfPTable totalsTable = new PdfPTable(2)
                    {
                        WidthPercentage = 100
                    };
                    totalsTable.SetWidths(new float[] { 6f, 4f });

                    doc.Add(new iTextSharp.text.Paragraph(new string('-', 70), normalFont) { Alignment = Element.ALIGN_CENTER });

                    totalsTable.AddCell(new PdfPCell(new Phrase("TOTAL", headerFont))
                    {
                        Border = 0,
                        HorizontalAlignment = PdfPCell.ALIGN_LEFT,
                        PaddingLeft = 10f,  
                        PaddingRight = 10f  
                    });

                    totalsTable.AddCell(new PdfPCell(new Phrase($"₱ {TotalAmountText.Text}", normalFont))
                    {
                        Border = 0,
                        HorizontalAlignment = PdfPCell.ALIGN_RIGHT,
                        PaddingLeft = 10f,  
                        PaddingRight = 10f  
                    });

                    
                    totalsTable.AddCell(new PdfPCell(new Phrase("AMOUNT PAID", normalFont))
                    {
                        Border = 0,
                        HorizontalAlignment = PdfPCell.ALIGN_LEFT,
                        PaddingLeft = 10f,  
                        PaddingRight = 10f  
                    });

                    totalsTable.AddCell(new PdfPCell(new Phrase($"₱ {_amountPaid:F2}", normalFont))
                    {
                        Border = 0,
                        HorizontalAlignment = PdfPCell.ALIGN_RIGHT,
                        PaddingLeft = 10f,  
                        PaddingRight = 10f  
                    });

                    totalsTable.AddCell(new PdfPCell(new Phrase("CHANGE", normalFont))
                    {
                        Border = 0,
                        HorizontalAlignment = PdfPCell.ALIGN_LEFT,
                        PaddingLeft = 10f,  
                        PaddingRight = 10f  
                    });

                    totalsTable.AddCell(new PdfPCell(new Phrase($"₱ {_change:F2}", normalFont))

                    {
                        Border = 0,
                        HorizontalAlignment = PdfPCell.ALIGN_RIGHT,
                        PaddingLeft = 10f,  
                        PaddingRight = 10f  
                    });

                    doc.Add(totalsTable);

                    doc.Add(new iTextSharp.text.Paragraph("Thank you! Please come again.", normalFont) { Alignment = Element.ALIGN_CENTER });
                    doc.Add(new iTextSharp.text.Paragraph("** This is a computer-generated receipt.", normalFont) { Alignment = Element.ALIGN_CENTER });

                    doc.Close();

                    memoryStream.Position = 0;

                    string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
                    File.WriteAllBytes(tempFilePath, memoryStream.ToArray());

                    Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });

                    Task.Run(() =>
                    {
                        Thread.Sleep(500000);
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
                return context.tbl_company.FirstOrDefault();
            }
        }






    }

}
