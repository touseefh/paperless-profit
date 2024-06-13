using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Publisher
{
    public class Create : PageModel
    {
        private readonly ILogger<Create> _logger;

        public Create(ILogger<Create> logger)
        {
            _logger = logger;
            PublisherCreate = new PublisherCreate(); // Initializing non-nullable property
            Message = ""; // Initializing non-nullable property
        }

        [BindProperty]
        public PublisherCreate PublisherCreate { get; set; }

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
                string connectionString = "Server=DESKTOP-S9EO4OF\\SQLEXPRESS;Database=Touseef;Trusted_Connection=True;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the provided PublisherID already exists
                    string checkSql = "SELECT COUNT(*) FROM Publisher WHERE PublisherID = @PublisherID";
                    using (SqlCommand checkCommand = new SqlCommand(checkSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@PublisherID", PublisherCreate.PublisherID);
                        int existingRecordsCount = (int)checkCommand.ExecuteScalar();

                        if (existingRecordsCount > 0)
                        {
                            ModelState.AddModelError("PublisherCreate.PublisherID", "Publisher with the provided ID already exists.");
                            return Page();
                        }
                    }

                    // If the PublisherID is unique, proceed with insertion
                    string sql = @"INSERT INTO Publisher (PublisherID, Name, Email, Phone, Address) 
                                   VALUES (@PublisherID, @Name, @Email, @Phone, @Address)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PublisherID", PublisherCreate.PublisherID);
                        command.Parameters.AddWithValue("@Name", PublisherCreate.Name);
                        command.Parameters.AddWithValue("@Email", PublisherCreate.Email);
                        command.Parameters.AddWithValue("@Phone", PublisherCreate.Phone);
                        command.Parameters.AddWithValue("@Address", PublisherCreate.Address);
                        command.ExecuteNonQuery();
                    }
                }

                Message = "Publisher added successfully.";
                return RedirectToPage("./Publisher");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the publisher: " + ex.Message);
                return Page();
            }
        }
    }

    public class PublisherCreate
    {
        public int PublisherID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }= "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; } = "";

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = "";
    }
}
