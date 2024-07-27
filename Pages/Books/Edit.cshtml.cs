using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Books
{
    public class EditModel : PageModel
    {
        private readonly ILogger<EditModel> _logger;

        public EditModel(ILogger<EditModel> logger)
        {
            _logger = logger;
            BookEdit = new BookEditModel(); // Initialize BookEdit
        }

        [BindProperty]
        public BookEditModel BookEdit { get; set; }

        public IActionResult OnGet(int? BookID)
        {
            if (BookID.HasValue)
            {
                try
                {
                    string connectionString = "";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "SELECT * FROM Book WHERE BookID = @BookID";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@BookID", BookID);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    BookEdit = new BookEditModel
                                    {
                                        BookID = reader.GetInt32(0),
                                        Title = reader.GetString(1),
                                        Author = reader.GetString(2),
                                        Genre = reader.GetString(3),
                                        ISBN = reader.GetString(4),
                                        Price = reader.GetDecimal(5),
                                        StockQuantity = reader.GetInt32(6)
                                    };
                                    return Page();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred while retrieving the book for editing: " + ex.Message);
                }
            }

            return RedirectToPage("./Books");
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                string connectionString = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"UPDATE Book 
                                   SET Title = @Title, 
                                       Author = @Author, 
                                       Genre = @Genre, 
                                       ISBN = @ISBN, 
                                       Price = @Price, 
                                       StockQuantity = @StockQuantity 
                                   WHERE BookID = @BookID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BookID", BookEdit.BookID);
                        command.Parameters.AddWithValue("@Title", BookEdit.Title);
                        command.Parameters.AddWithValue("@Author", BookEdit.Author);
                        command.Parameters.AddWithValue("@Genre", BookEdit.Genre);
                        command.Parameters.AddWithValue("@ISBN", BookEdit.ISBN);
                        command.Parameters.AddWithValue("@Price", BookEdit.Price);
                        command.Parameters.AddWithValue("@StockQuantity", BookEdit.StockQuantity);
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToPage("./Books");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating the book: " + ex.Message);
                return Page();
            }
        }
    }

    public class BookEditModel
    {
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
    }
}
