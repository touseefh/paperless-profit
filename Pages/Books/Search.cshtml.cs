using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Books
{
    public class SearchModel : PageModel
    {
        [BindProperty]
        public int BookID { get; set; }

        public BookSearch? BookSearch { get; set; } // Declare BookSearch as nullable

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            try
            {
                string connectionString ="";
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
                                BookSearch = new BookSearch
                                {
                                    BookID = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Author = reader.GetString(2),
                                    Genre = reader.GetString(3),
                                    ISBN = reader.GetString(4),
                                    Price = reader.GetDecimal(5),
                                    StockQuantity = reader.GetInt32(6)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something Went Wrong :" + ex.Message);
            }

            return Page();
        }
    }

    public class BookSearch
    {
        public int BookID { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Genre { get; set; } = "";
        public string ISBN { get; set; } = "";
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
