using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Books
{
    public class DeleteModel : PageModel
    {
        private readonly string _connectionString = "";

        [BindProperty(SupportsGet = true)]
        public int BookID { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "DELETE FROM Book WHERE BookID = @BookID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BookID", BookID);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return RedirectToPage("./Books");
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                Console.WriteLine("An error occurred while deleting the book: " + ex.Message);
                return RedirectToPage("./Books"); // Redirect to books page even if there's an error
            }
        }
    }
}
