using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Books
{
    public class BooksModel : PageModel
    {
        public List<BookInfo> BookList { get; set; } = new List<BookInfo>();

        public void OnGet()
        {
            try
            {
                string connectionString = "Server=DESKTOP-S9EO4OF\\SQLEXPRESS;Database=Touseef;Trusted_Connection=True;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Book ORDER BY BookID ASC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BookInfo bookInfo = new BookInfo();
                            bookInfo.BookID = reader.GetInt32(0);
                            bookInfo.Title = reader.GetString(1);
                            bookInfo.Author = reader.GetString(2);
                            bookInfo.Genre = reader.GetString(3);
                            bookInfo.ISBN = reader.GetString(4);
                            bookInfo.Price = reader.GetDecimal(5);
                            bookInfo.StockQuantity = reader.GetInt32(6);
                            bookInfo.PublisherID = reader.GetInt32(7);
                            bookInfo.SupplierID = reader.GetInt32(8);

                            BookList.Add(bookInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong: " + ex.Message);
            }
        }
    }

    public class BookInfo
    {
        public int BookID { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Genre { get; set; } = "";
        public string ISBN { get; set; } = "";
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int PublisherID { get; set; }
        public int SupplierID { get; set; }
    }
}
