using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Bookshop.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly string _connectionString = "";

        [BindProperty]
        public CustomerInputModel Customer { get; set; } = new CustomerInputModel();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Check if CustomerID already exists
                string checkSql = "SELECT COUNT(*) FROM Customer WHERE CustomerID = @CustomerID";
                using (SqlCommand checkCommand = new SqlCommand(checkSql, connection))
                {
                    checkCommand.Parameters.AddWithValue("@CustomerID", Customer.CustomerID);
                    int count = (int)checkCommand.ExecuteScalar();
                    if (count > 0)
                    {
                        ModelState.AddModelError("Customer.CustomerID", "Customer ID already exists.");
                        return Page();
                    }
                }

                // Insert new customer
                string sql = "INSERT INTO Customer (CustomerID, Name, Email, Phone, Address) VALUES (@CustomerID, @Name, @Email, @Phone, @Address)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", Customer.CustomerID);
                    command.Parameters.AddWithValue("@Name", Customer.Name);
                    command.Parameters.AddWithValue("@Email", Customer.Email);
                    command.Parameters.AddWithValue("@Phone", Customer.Phone);
                    command.Parameters.AddWithValue("@Address", Customer.Address);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToPage("./Customers");
        }

        public class CustomerInputModel
        {
            [Required(ErrorMessage = "Customer ID is required")]
            public int CustomerID { get; set; }

            [Required(ErrorMessage = "Name is required")]
            [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Phone is required")]
            [Phone(ErrorMessage = "Invalid phone number")]
            public string Phone { get; set; } = string.Empty;

            [Required(ErrorMessage = "Address is required")]
            [StringLength(255, ErrorMessage = "Address cannot be longer than 255 characters")]
            public string Address { get; set; } = string.Empty;
        }
    }
}
