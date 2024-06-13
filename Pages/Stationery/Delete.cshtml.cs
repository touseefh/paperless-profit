using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Stationery
{
    public class DeleteModel : PageModel
    {
        private readonly string _connectionString = "Server=DESKTOP-S9EO4OF\\SQLEXPRESS;Database=Touseef;Trusted_Connection=True;TrustServerCertificate=True";

        [BindProperty(SupportsGet = true)]
        public int StationeryID { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "DELETE FROM Stationery WHERE StationeryID = @StationeryID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StationeryID", StationeryID);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return RedirectToPage("./Stationery");
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                Console.WriteLine("An error occurred while deleting the stationery: " + ex.Message);
                return RedirectToPage("./Stationery"); // Redirect to stationery page even if there's an error
            }
        }
    }
}
