using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bookshop.Pages.Orders
{
    public class OrderHistoryModel : PageModel
    {
        private readonly string _connectionString = "";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderHistoryModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();

        public void OnGet()
        {
            Orders = FetchOrderHistoryFromDatabase();
        }

        public IActionResult OnPostPrintInvoice(int orderId)
        {
            OrderViewModel? order = FetchOrderDetails(orderId);

            if (order == null)
            {
                return NotFound();
            }

            byte[] pdfBytes = GeneratePdfInvoice(order);

            // Return the PDF file directly without using the File method
            return new FileContentResult(pdfBytes, "application/pdf")
            {
                FileDownloadName = $"Invoice_{orderId}.pdf"
            };
        }

        private List<OrderViewModel> FetchOrderHistoryFromDatabase()
        {
            List<OrderViewModel> orders = new List<OrderViewModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM OrderHistory", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new OrderViewModel
                            {
                                OrderID = reader.GetInt32(0),
                                OrderDate = reader.GetDateTime(1),
                                TotalAmount = reader.GetDecimal(2),
                                CustomerName = reader.GetString(3),
                                StationeryName = reader.GetString(4),
                                BookName = reader.GetString(5),
                                SalesmanName = reader.GetString(6),
                                Quantity = reader.GetInt32(7)
                            });
                        }
                    }
                }
            }

            return orders;
        }

        private OrderViewModel? FetchOrderDetails(int orderId)
        {
            OrderViewModel? order = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM OrderHistory WHERE OrderID = @OrderId", connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            order = new OrderViewModel
                            {
                                OrderID = reader.GetInt32(0),
                                OrderDate = reader.GetDateTime(1),
                                TotalAmount = reader.GetDecimal(2),
                                CustomerName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                StationeryName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                BookName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                SalesmanName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                Quantity = reader.GetInt32(7)
                            };
                        }
                    }
                }
            }

            return order;
        }
        private byte[] GeneratePdfInvoice(OrderViewModel order)
{
    PdfDocument document = new PdfDocument();
    PdfPage page = document.AddPage();
    XGraphics gfx = XGraphics.FromPdfPage(page);

    XFont titleFont = new XFont("Arial", 24, XFontStyle.Bold);
    XFont textFont = new XFont("Arial", 14, XFontStyle.Bold);

    // Path to the logo image
    string logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "books+.jpg");

    // Add logo if the file exists
    if (System.IO.File.Exists(logoPath))
    {
        XImage logo = XImage.FromFile(logoPath);
        gfx.DrawImage(logo, 40, 40, 100, 100);
    }

    // Draw invoice title with different color
    string invoiceTitle = "Books +";
    XSize titleSize = gfx.MeasureString(invoiceTitle, titleFont);
    gfx.DrawString(invoiceTitle, titleFont, XBrushes.DarkRed, new XRect((page.Width - titleSize.Width) / 2, 20, page.Width, page.Height), XStringFormats.TopLeft);

    // Adjusted startY position
    int startY = (int)(20 + titleSize.Height + 20); // Close to the title

    // Define order details as a list of strings
    List<string> orderDetails = new List<string>
    {
        $"Order ID: {order.OrderID}",
        $"Order Date: {order.OrderDate.ToShortDateString()}",
        $"Total Amount: ${order.TotalAmount}",
        $"Customer Name: {order.CustomerName}",
        $"Stationery Name: {order.StationeryName}",
        $"Book Name: {order.BookName}",
        $"Salesman Name: {order.SalesmanName}",
        $"Quantity: {order.Quantity}"
    };

    // Calculate total height of the order details block
    double totalHeight = orderDetails.Count * textFont.Height;

    // Calculate the starting X position for right alignment
    double startX = page.Width - 40; // Right margin
    double maxWidth = page.Width - 80; // Maximum width for order details

    // Draw each order detail with right alignment and centered vertically
    for (int i = 0; i < orderDetails.Count; i++)
    {
        XSize size = gfx.MeasureString(orderDetails[i], textFont);
        double y = startY + (i * size.Height) + ((page.Height - startY - totalHeight) / 2); // Center vertically
        double x = startX - size.Width; // Right align
        gfx.DrawString(orderDetails[i], textFont, XBrushes.Black, new XRect(x, y, maxWidth, page.Height), XStringFormats.TopLeft);
    }

    // Save the document to a MemoryStream and return the byte array
    MemoryStream stream = new MemoryStream();
    document.Save(stream, false);
    byte[] pdfBytes = stream.ToArray();

    return pdfBytes;
}



        public class OrderViewModel
        {
            public int OrderID { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string CustomerName { get; set; } = "";
            public string StationeryName { get; set; } = "";
            public string BookName { get; set; } = "";
            public string SalesmanName { get; set; } = "";
            public int Quantity { get; set; }
        }
    }
}
