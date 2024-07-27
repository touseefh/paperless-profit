using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Stationery
{
    public class EditModel : PageModel
    {
        private readonly ILogger<EditModel> _logger;
        private readonly string _connectionString = "";

        public EditModel(ILogger<EditModel> logger)
        {
            _logger = logger;
            StationeryEdit = new StationeryEditModel(); // Initialize StationeryEdit
        }

        [BindProperty]
        public StationeryEditModel StationeryEdit { get; set; }

        public IActionResult OnGet(int? StationeryID)
        {
            if (StationeryID.HasValue)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
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
                                    StationeryEdit = new StationeryEditModel
                                    {
                                        StationeryID = reader.GetInt32(0),
                                        Name = reader.GetString(1),
                                        Description = reader.GetString(2),
                                        Type = reader.GetString(3),
                                        Price = reader.GetDecimal(4),
                                        StockQuantity = reader.GetInt32(5)
                                    };
                                    return Page();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred while retrieving the stationery for editing: " + ex.Message);
                }
            }

            return RedirectToPage("./Stationery");
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"UPDATE Stationery 
                                   SET Name = @Name, 
                                       Description = @Description, 
                                       Type = @Type, 
                                       Price = @Price, 
                                       StockQuantity = @StockQuantity
                                   WHERE StationeryID = @StationeryID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StationeryID", StationeryEdit.StationeryID);
                        command.Parameters.AddWithValue("@Name", StationeryEdit.Name);
                        command.Parameters.AddWithValue("@Description", StationeryEdit.Description);
                        command.Parameters.AddWithValue("@Type", StationeryEdit.Type);
                        command.Parameters.AddWithValue("@Price", StationeryEdit.Price);
                        command.Parameters.AddWithValue("@StockQuantity", StationeryEdit.StockQuantity);
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToPage("./Stationery");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating the stationery: " + ex.Message);
                ModelState.AddModelError("", "An error occurred while updating the stationery. Please try again later.");
                return Page();
            }
        }
    }

    public class StationeryEditModel
    {
        public int StationeryID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; } = "";

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Stock quantity must be at least 1")]
        public int StockQuantity { get; set; }
    }
}
