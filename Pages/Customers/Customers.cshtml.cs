using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Customers
{
    public class CustomersModel : PageModel
    {
        private readonly string _connectionString = "Server=DESKTOP-S9EO4OF\\SQLEXPRESS;Database=Touseef;Trusted_Connection=True;TrustServerCertificate=True";

        public List<Customer> Customers { get; set; } = new List<Customer>();

        public void OnGet()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Customer";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Customers.Add(new Customer
                            {
                                CustomerID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Phone = reader.GetString(3),
                                Address = reader.GetString(4)
                            });
                        }
                    }
                }
            }
        }

        public class Customer
        {
            public int CustomerID { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
        }
    }
}
