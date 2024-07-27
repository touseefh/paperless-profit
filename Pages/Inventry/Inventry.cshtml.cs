using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Bookshop.Pages.Inventory
{
    public class InventoryModel : PageModel
    {
        public List<BookInfo> BookList { get; set; } = new List<BookInfo>();
        public List<StationeryInfo> StationeryList { get; set; } = new List<StationeryInfo>();
        public List<string> BookTitles { get; set; } = new List<string>();
        public List<int> BookStockQuantities { get; set; } = new List<int>();
        public List<string> StationeryNames { get; set; } = new List<string>();
        public List<int> StationeryStockQuantities { get; set; } = new List<int>();

        public void OnGet()
        {
            try
            {
                string connectionString = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Load books
                    string bookSql = "SELECT * FROM Book ORDER BY BookID ASC";
                    using (SqlCommand bookCommand = new SqlCommand(bookSql, connection))
                    using (SqlDataReader bookReader = bookCommand.ExecuteReader())
                    {
                        while (bookReader.Read())
                        {
                            BookInfo bookInfo = new BookInfo
                            {
                                BookID = bookReader.GetInt32(0),
                                Title = bookReader.GetString(1),
                                Author = bookReader.GetString(2),
                                Genre = bookReader.GetString(3),
                                ISBN = bookReader.GetString(4),
                                Price = bookReader.GetDecimal(5),
                                StockQuantity = bookReader.GetInt32(6),
                                PublisherID = bookReader.GetInt32(7),
                                SupplierID = bookReader.GetInt32(8)
                            };
                            BookList.Add(bookInfo);
                            BookTitles.Add(bookInfo.Title);
                            BookStockQuantities.Add(bookInfo.StockQuantity);
                        }
                    }

                    // Load stationery
                    string stationerySql = "SELECT * FROM Stationery ORDER BY StationeryID ASC";
                    using (SqlCommand stationeryCommand = new SqlCommand(stationerySql, connection))
                    using (SqlDataReader stationeryReader = stationeryCommand.ExecuteReader())
                    {
                        while (stationeryReader.Read())
                        {
                            StationeryInfo stationeryInfo = new StationeryInfo
                            {
                                StationeryID = stationeryReader.GetInt32(0),
                                Name = stationeryReader.GetString(1),
                                Description = stationeryReader.GetString(2),
                                Type = stationeryReader.GetString(3),
                                Price = stationeryReader.GetDecimal(4),
                                StockQuantity = stationeryReader.GetInt32(5),
                                SupplierID = stationeryReader.GetInt32(6)
                            };
                            StationeryList.Add(stationeryInfo);
                            StationeryNames.Add(stationeryInfo.Name);
                            StationeryStockQuantities.Add(stationeryInfo.StockQuantity);
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

    public class BookInfo
    {
        public int BookID { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Genre { get; set; } = "";
        public string ISBN { get; set; } = "";
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int PublisherID { get; set; }
        public int SupplierID { get; set; }
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
