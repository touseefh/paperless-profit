using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Stationery
{
    public class StationeryModel : PageModel
    {
        public List<StationeryInfo> StationeryList { get; set; } = new List<StationeryInfo>();

        public void OnGet()
        {
            try
            {
                string connectionString = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Stationery ORDER BY StationeryID ASC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StationeryInfo stationeryInfo = new StationeryInfo();
                            stationeryInfo.StationeryID = reader.GetInt32(0);
                            stationeryInfo.Name = reader.GetString(1);
                            stationeryInfo.Description = reader.GetString(2);
                            stationeryInfo.Type = reader.GetString(3);
                            stationeryInfo.Price = reader.GetDecimal(4);
                            stationeryInfo.StockQuantity = reader.GetInt32(5);
                            stationeryInfo.SupplierID = reader.GetInt32(6);

                            StationeryList.Add(stationeryInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong: " + ex.Message);
            }
        }
    }

    public class StationeryInfo
    {
        public int StationeryID { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int SupplierID { get; set; }
    }
}
