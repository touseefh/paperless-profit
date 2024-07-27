using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Stationery
{
    public class SearchModel : PageModel
    {
        [BindProperty]
        public int StationeryID { get; set; }

        public StationerySearch? StationerySearch { get; set; } // Declare StationerySearch as nullable

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            try
            {
                string connectionString = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Stationery WHERE StationeryID = @StationeryID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StationeryID", StationeryID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                StationerySearch = new StationerySearch
                                {
                                    StationeryID = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Type = reader.GetString(3),
                                    Price = reader.GetDecimal(4),
                                    StockQuantity = reader.GetInt32(5)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong: " + ex.Message);
            }

            return Page();
        }
    }

    public class StationerySearch
    {
        public int StationeryID { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
