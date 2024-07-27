using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Publisher
{
    public class Search : PageModel
    {
        [BindProperty]
        public int PublisherID { get; set; }

        public PublisherSearch? PublisherSearch { get; set; } // Declare PublisherInfo as nullable

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
                    string sql = "SELECT * FROM Publisher WHERE PublisherID = @PublisherID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PublisherID", PublisherID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                PublisherSearch = new PublisherSearch
                                {
                                    PublisherID = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Phone = reader.GetString(3),
                                    Address = reader.GetString(4)
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

    public class PublisherSearch
    {
        public int PublisherID { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
    }
}
