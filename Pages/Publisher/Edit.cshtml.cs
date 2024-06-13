using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Publisher
{
    public class Edit : PageModel
    {
        private readonly ILogger<Edit> _logger;

        public Edit(ILogger<Edit> logger)
        {
            _logger = logger;
            PublisherEdit = new PublisherEdit(); // Initialize PublisherEdit
            SuccessMessage = ""; // Initialize SuccessMessage
        }

        [BindProperty]
        public PublisherEdit PublisherEdit { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        public IActionResult OnGet(int? PublisherID)
        {
            if (PublisherID.HasValue)
            {
                try
                {
                    string connectionString = "Server=DESKTOP-S9EO4OF\\SQLEXPRESS;Database=Touseef;Trusted_Connection=True;TrustServerCertificate=True";
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
                                    PublisherEdit = new PublisherEdit
                                    {
                                        PublisherID = reader.GetInt32(0),
                                        Name = reader.GetString(1),
                                        Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                                        Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                                        Address = reader.GetString(4)
                                    };
                                    return Page();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred while retrieving the publisher for editing: " + ex.Message);
                }
            }

            return RedirectToPage("./Publisher");
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                string connectionString ="Server=DESKTOP-S9EO4OF\\SQLEXPRESS;Database=Touseef;Trusted_Connection=True;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"UPDATE Publisher SET Name = @Name, Email = @Email, Phone = @Phone, Address = @Address WHERE PublisherID = @PublisherID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PublisherID", PublisherEdit.PublisherID);
                        command.Parameters.AddWithValue("@Name", PublisherEdit.Name);
                        command.Parameters.AddWithValue("@Email", PublisherEdit.Email ?? (object)DBNull.Value); // Handle null values
                        command.Parameters.AddWithValue("@Phone", PublisherEdit.Phone ?? (object)DBNull.Value); // Handle null values
                        command.Parameters.AddWithValue("@Address", PublisherEdit.Address);
                        command.ExecuteNonQuery();
                    }
                }

                SuccessMessage = "Publisher information updated successfully!";
                return RedirectToPage("./Publisher");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating the publisher: " + ex.Message);
                return Page();
            }
        }
    }

    public class PublisherEdit
    {
        public int PublisherID { get; set; }

        public string Name { get; set; } = "";

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; } // Nullable

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; } // Nullable

        public string Address { get; set; } = "";
    }
}
