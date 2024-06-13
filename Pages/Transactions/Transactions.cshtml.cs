using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace Bookshop.Pages.Transactions
{
    public class TransactionsModel : PageModel
    {
        private readonly string _connectionString = "Server=DESKTOP-S9EO4OF\\SQLEXPRESS;Database=Touseef;Trusted_Connection=True;TrustServerCertificate=True";

        public List<TransactionViewModel> Transactions { get; set; } = new List<TransactionViewModel>();
        public int TotalOrders { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalAmount { get; set; }

        public void OnGet()
        {
            Transactions = FetchTransactionsFromDatabase();
            TotalOrders = Transactions.Count;
            TotalTransactions = Transactions.Sum(t => t.Quantity);
            TotalAmount = Transactions.Sum(t => t.TotalAmount);
        }

        public IActionResult OnGetDownloadReport()
        {
            byte[] reportBytes = GenerateExcelReport(Transactions);
            string fileName = "TransactionsReport.xlsx";

            return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private List<TransactionViewModel> FetchTransactionsFromDatabase()
        {
            List<TransactionViewModel> transactions = new List<TransactionViewModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT OrderID, OrderDate, Quantity, TotalAmount FROM OrderHistory", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(new TransactionViewModel
                            {
                                OrderID = reader.GetInt32(0),
                                OrderDate = reader.GetDateTime(1),
                                Quantity = reader.GetInt32(2),
                                TotalAmount = reader.GetDecimal(3)
                            });
                        }
                    }
                }
            }

            return transactions;
        }

        private byte[] GenerateExcelReport(List<TransactionViewModel> transactions)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Transactions");

                worksheet.Cells[1, 1].Value = "OrderID";
                worksheet.Cells[1, 2].Value = "OrderDate";
                worksheet.Cells[1, 3].Value = "Quantity";
                worksheet.Cells[1, 4].Value = "TotalAmount";

                for (int i = 0; i < transactions.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = transactions[i].OrderID;
                    worksheet.Cells[i + 2, 2].Value = transactions[i].OrderDate.ToShortDateString();
                    worksheet.Cells[i + 2, 3].Value = transactions[i].Quantity;
                    worksheet.Cells[i + 2, 4].Value = transactions[i].TotalAmount;
                }

                return package.GetAsByteArray();
            }
        }

        public class TransactionViewModel
        {
            public int OrderID { get; set; }
            public DateTime OrderDate { get; set; }
            public int Quantity { get; set; }
            public decimal TotalAmount { get; set; }
        }
    }
}
