using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Publisher
{
    public class Delete : PageModel
    {
        private readonly string _connectionString = "";

        [BindProperty(SupportsGet = true)]
        public int PublisherID { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "DELETE FROM Publisher WHERE PublisherID = @PublisherID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PublisherID", PublisherID);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return RedirectToPage("./Publisher");
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                Console.WriteLine("An error occurred while deleting the publisher: " + ex.Message);
                return RedirectToPage("./Publisher"); // Redirect to publisher page even if there's an error
            }
        }
    }
}
