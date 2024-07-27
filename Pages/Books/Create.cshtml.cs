using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;
        private readonly string _connectionString = "";

        public CreateModel(ILogger<CreateModel> logger)
        {
            _logger = logger;
            BookCreate = new BookCreateModel(); // Initialize BookCreate property
            Message = ""; // Initialize Message property
        }

        [BindProperty]
        public BookCreateModel BookCreate { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Check if the provided PublisherID exists
                    if (!CheckForeignKeyExistence("Publisher", "PublisherID", BookCreate.PublisherID))
                    {
                        ModelState.AddModelError("BookCreate.PublisherID", "Publisher with the provided ID does not exist.");
                        return Page();
                    }

                    // Check if the provided SupplierID exists
                    if (!CheckForeignKeyExistence("Supplier", "SupplierID", BookCreate.SupplierID))
                    {
                        ModelState.AddModelError("BookCreate.SupplierID", "Supplier with the provided ID does not exist.");
                        return Page();
                    }

                    // Check if the provided BookID exists
                    if (CheckBookIDExistence(BookCreate.BookID))
                    {
                        ModelState.AddModelError("BookCreate.BookID", "Book with the provided ID already exists.");
                        return Page();
                    }

                    // If the foreign keys and BookID are unique, proceed with insertion
                    string sql = @"INSERT INTO Book (BookID, Title, Author, Genre, ISBN, Price, StockQuantity, PublisherID, SupplierID) 
                                   VALUES (@BookID, @Title, @Author, @Genre, @ISBN, @Price, @StockQuantity, @PublisherID, @SupplierID)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BookID", BookCreate.BookID);
                        command.Parameters.AddWithValue("@Title", BookCreate.Title);
                        command.Parameters.AddWithValue("@Author", BookCreate.Author);
                        command.Parameters.AddWithValue("@Genre", BookCreate.Genre);
                        command.Parameters.AddWithValue("@ISBN", BookCreate.ISBN);
                        command.Parameters.AddWithValue("@Price", BookCreate.Price);
                        command.Parameters.AddWithValue("@StockQuantity", BookCreate.StockQuantity);
                        command.Parameters.AddWithValue("@PublisherID", BookCreate.PublisherID);
                        command.Parameters.AddWithValue("@SupplierID", BookCreate.SupplierID);
                        command.ExecuteNonQuery();
                    }
                }

                Message = "Book added successfully.";
                return RedirectToPage("./Books");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the book: " + ex.Message);
                return Page();
            }
        }

        private bool CheckForeignKeyExistence(string tableName, string columnName, int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @ID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private bool CheckBookIDExistence(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM Book WHERE BookID = @ID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }

    public class BookCreateModel
    {
        [Required(ErrorMessage = "Book ID is required")]
        public int BookID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; } = "";

        [Required(ErrorMessage = "Genre is required")]
        public string Genre { get; set; } = "";

        [Required(ErrorMessage = "ISBN is required")]
        public string ISBN { get; set; } = "";

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Stock quantity must be at least 1")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Publisher ID is required")]
        public int PublisherID { get; set; }

        [Required(ErrorMessage = "Supplier ID is required")]
        public int SupplierID { get; set; }
    }
}
