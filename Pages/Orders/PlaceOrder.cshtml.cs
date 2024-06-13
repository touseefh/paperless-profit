using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Bookshop.Pages.Orders
{
    public class PlaceOrderModel : PageModel
    {
        private readonly string _connectionString = "Server=DESKTOP-S9EO4OF\\SQLEXPRESS;Database=Touseef;Trusted_Connection=True;TrustServerCertificate=True;Connection Timeout=30;"; // Increased timeout

        [BindProperty]
        public OrderInputModel OrderInput { get; set; } = new OrderInputModel();

        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
        public List<SalesmanViewModel> Salesmen { get; set; } = new List<SalesmanViewModel>();
        public List<BookViewModel> Books { get; set; } = new List<BookViewModel>();
        public List<StationeryViewModel> Stationeries { get; set; } = new List<StationeryViewModel>();

        public IActionResult OnGet()
        {
            LoadDropdownData();
            return Page();
        }

        public IActionResult OnPostPlaceOrder()
        {
            if (!ModelState.IsValid)
            {
                LoadDropdownData();
                return Page();
            }

            if (OrderExists(OrderInput.OrderID))
            {
                ModelState.AddModelError(nameof(OrderInput.OrderID), "Order ID already exists.");
                LoadDropdownData();
                return Page();
            }

            // Validate the OrderDate
            if (OrderInput.OrderDate < (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue ||
                OrderInput.OrderDate > (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue)
            {
                ModelState.AddModelError(string.Empty, "Order Date is out of range.");
                LoadDropdownData();
                return Page();
            }

            decimal totalAmount = CalculateTotalAmount(OrderInput.BookID, OrderInput.StationeryID, OrderInput.BookQuantity, OrderInput.StationeryQuantity);

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("INSERT INTO OrderHistory (OrderID, OrderDate, TotalAmount, CustomerName, StationeryName, BookName, SalesmanName, Quantity) VALUES (@OrderID, @OrderDate, @TotalAmount, @CustomerName, @StationeryName, @BookName, @SalesmanName, @Quantity)", connection))
                    {
                        command.Parameters.AddWithValue("@OrderID", OrderInput.OrderID);
                        command.Parameters.AddWithValue("@OrderDate", OrderInput.OrderDate);
                        command.Parameters.AddWithValue("@TotalAmount", totalAmount);
                        command.Parameters.AddWithValue("@CustomerName", GetCustomerName(OrderInput.CustomerID));
                        command.Parameters.AddWithValue("@StationeryName", GetStationeryName(OrderInput.StationeryID));
                        command.Parameters.AddWithValue("@BookName", GetBookName(OrderInput.BookID));
                        command.Parameters.AddWithValue("@SalesmanName", GetSalesmanName(OrderInput.SalesmanID));
                        command.Parameters.AddWithValue("@Quantity", OrderInput.BookQuantity + OrderInput.StationeryQuantity);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log the exception here
                Console.Error.WriteLine(ex); // Logging the exception to the console or a file
                ModelState.AddModelError(string.Empty, "An error occurred while placing the order. Please try again later.");
                LoadDropdownData();
                return Page();
            }

            return RedirectToPage("./OrderHistory");
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("./Index");
        }

        private void LoadDropdownData()
        {
            Customers = FetchCustomers();
            Salesmen = FetchSalesmen();
            Books = FetchBooks();
            Stationeries = FetchStationeries();
        }

        private bool OrderExists(int orderId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(1) FROM OrderHistory WHERE OrderID = @OrderID", connection))
                {
                    command.Parameters.AddWithValue("@OrderID", orderId);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        private decimal CalculateTotalAmount(int bookId, int stationeryId, int bookQuantity, int stationeryQuantity)
        {
            decimal bookPrice = GetBookPrice(bookId);
            decimal stationeryPrice = GetStationeryPrice(stationeryId);

            return (bookPrice * bookQuantity) + (stationeryPrice * stationeryQuantity);
        }

        private decimal GetBookPrice(int bookId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT Price FROM Book WHERE BookID = @BookID", connection))
                {
                    command.Parameters.AddWithValue("@BookID", bookId);
                    return (decimal)command.ExecuteScalar();
                }
            }
        }

        private decimal GetStationeryPrice(int stationeryId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT Price FROM Stationery WHERE StationeryID = @StationeryID", connection))
                {
                    command.Parameters.AddWithValue("@StationeryID", stationeryId);
                    return (decimal)command.ExecuteScalar();
                }
            }
        }

        private string GetCustomerName(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT Name FROM Customer WHERE CustomerID = @CustomerID", connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", customerId);
                    return (string)command.ExecuteScalar();
                }
            }
        }

        private string GetSalesmanName(int salesmanId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT Name FROM Salesman WHERE SalesmanID = @SalesmanID", connection))
                {
                    command.Parameters.AddWithValue("@SalesmanID", salesmanId);
                    return (string)command.ExecuteScalar();
                }
            }
        }

        private string GetBookName(int bookId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT Title FROM Book WHERE BookID = @BookID", connection))
                {
                    command.Parameters.AddWithValue("@BookID", bookId);
                    return (string)command.ExecuteScalar();
                }
            }
        }

        private string GetStationeryName(int stationeryId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT Name FROM Stationery WHERE StationeryID = @StationeryID", connection))
                {
                    command.Parameters.AddWithValue("@StationeryID", stationeryId);
                    return (string)command.ExecuteScalar();
                }
            }
        }

        private List<CustomerViewModel> FetchCustomers()
        {
            List<CustomerViewModel> customers = new List<CustomerViewModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT CustomerID, Name FROM Customer", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new CustomerViewModel
                            {
                                CustomerID = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return customers;
        }

        private List<SalesmanViewModel> FetchSalesmen()
        {
            List<SalesmanViewModel> salesmen = new List<SalesmanViewModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT SalesmanID, Name FROM Salesman", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            salesmen.Add(new SalesmanViewModel
                            {
                                SalesmanID = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return salesmen;
        }

        private List<BookViewModel> FetchBooks()
        {
            List<BookViewModel> books = new List<BookViewModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT BookID, Title FROM Book", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books.Add(new BookViewModel
                            {
                                BookID = reader.GetInt32(0),
                                Title = reader                                .GetString(1)
                            });
                        }
                    }
                }
            }
            return books;
        }

        private List<StationeryViewModel> FetchStationeries()
        {
            List<StationeryViewModel> stationeries = new List<StationeryViewModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT StationeryID, Name FROM Stationery", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stationeries.Add(new StationeryViewModel
                            {
                                StationeryID = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return stationeries;
        }
    }

    public class OrderInputModel
    {
        [Required]
        public int OrderID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public int SalesmanID { get; set; }

        [Required]
        public int BookID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int BookQuantity { get; set; }

        [Required]
        public int StationeryID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int StationeryQuantity { get; set; }
    }

    public class CustomerViewModel
    {
        public int CustomerID { get; set; }
        public string Name { get; set; } = "";
    }

    public class SalesmanViewModel
    {
        public int SalesmanID { get; set; }
        public string Name { get; set; } = "";
    }

    public class BookViewModel
    {
        public int BookID { get; set; }
        public string Title { get; set; } = "";
    }

    public class StationeryViewModel
    {
        public int StationeryID { get; set; }
        public string Name { get; set; } = "";
    }
}

