using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Publisher
{
    public class Publisher : PageModel
    {
        public List<PublisherInfo> PublisherList { get; set; } = new List<PublisherInfo>();

        public void OnGet()
        {
            try
            {
                string connectionString = "Server=DESKTOP-S9EO4OF\\SQLEXPRESS;Database=Touseef;Trusted_Connection=True;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Publisher ORDER BY PublisherID ASC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PublisherInfo publisherInfo = new PublisherInfo();
                            publisherInfo.PublisherID = reader.GetInt32(0);
                            publisherInfo.Name = reader.GetString(1);
                            publisherInfo.Email = reader.GetString(2);
                            publisherInfo.Phone = reader.GetString(3);
                            publisherInfo.Address = reader.GetString(4);

                            PublisherList.Add(publisherInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something Went Wrong :" + ex.Message);
            }
        }
    }

    public class PublisherInfo
    {
        public int PublisherID { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
    }
}
