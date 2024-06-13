using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Stationery
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;
        private readonly string _connectionString = "Server=DESKTOP-S9EO4OF\\SQLEXPRESS;Database=Touseef;Trusted_Connection=True;TrustServerCertificate=True";

        public CreateModel(ILogger<CreateModel> logger)
        {
            _logger = logger;
            StationeryCreate = new StationeryCreateModel(); // Initialize StationeryCreate property
            Message = ""; // Initialize Message property
        }

        [BindProperty]
        public StationeryCreateModel StationeryCreate { get; set; }

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
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Check if the provided SupplierID exists
                    if (!CheckForeignKeyExistence("Supplier", "SupplierID", StationeryCreate.SupplierID))
                    {
                        ModelState.AddModelError("StationeryCreate.SupplierID", "Supplier with the provided ID does not exist.");
                        return Page();
                    }

                    // Check if the provided StationeryID exists
                    if (CheckStationeryIDExistence(StationeryCreate.StationeryID))
                    {
                        ModelState.AddModelError("StationeryCreate.StationeryID", "Stationery with the provided ID already exists.");
                        return Page();
                    }

                    // If the foreign key and StationeryID are unique, proceed with insertion
                    string sql = @"INSERT INTO Stationery (StationeryID, Name, Description, Type, Price, StockQuantity, SupplierID) 
                                   VALUES (@StationeryID, @Name, @Description, @Type, @Price, @StockQuantity, @SupplierID)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StationeryID", StationeryCreate.StationeryID);
                        command.Parameters.AddWithValue("@Name", StationeryCreate.Name);
                        command.Parameters.AddWithValue("@Description", StationeryCreate.Description);
                        command.Parameters.AddWithValue("@Type", StationeryCreate.Type);
                        command.Parameters.AddWithValue("@Price", StationeryCreate.Price);
                        command.Parameters.AddWithValue("@StockQuantity", StationeryCreate.StockQuantity);
                        command.Parameters.AddWithValue("@SupplierID", StationeryCreate.SupplierID);
                        command.ExecuteNonQuery();
                    }
                }

                Message = "Stationery added successfully.";
                return RedirectToPage("./Stationery");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the stationery: " + ex.Message);
                return Page();
            }
        }

        private bool CheckForeignKeyExistence(string tableName, string columnName, int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @ID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private bool CheckStationeryIDExistence(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM Stationery WHERE StationeryID = @ID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }

    public class StationeryCreateModel
    {
        [Required(ErrorMessage = "Stationery ID is required")]
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

        [Required(ErrorMessage = "Supplier ID is required")]
        public int SupplierID { get; set; }
    }
}
