using FishStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FishStore.Service
{
    internal class ReportService
    {
        private readonly ShopBanCaContext db;

        public ReportService(ShopBanCaContext dbContext)
        {
            db = dbContext;
        }

        public class MonthlyFishReport
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public string FishName { get; set; }
            public int ImportedQuantity { get; set; }
            public int SoldQuantity { get; set; }
            public decimal TotalRevenue { get; set; } // Giá bán * SL
            public decimal TotalCost { get; set; }    // Giá nhập * SL
            public decimal Profit => TotalRevenue - TotalCost;
        }

        public List<MonthlyFishReport> GetMonthlyReport()
        {
            var reports = new List<MonthlyFishReport>();

            var allFish = db.Fish.ToList();

            foreach (var fish in allFish)
            {
                var sales = db.OrderDetails
                    .Include(od => od.Order)
                    .Where(od => od.FishId == fish.FishId && od.Order.OrderDate.HasValue)
                    .ToList();

                var imports = db.InventoryTransactions
                    .Where(t => t.FishId == fish.FishId && t.TransactionDate.HasValue)
                    .ToList();

                var months = imports
                    .Select(t => GetMonthYear(t.TransactionDate.Value))
                    .Union(sales.Select(s => GetMonthYear(s.Order.OrderDate.Value)))
                    .Distinct()
                    .ToList();

                foreach (var (year, month) in months)
                {
                    var monthlyImports = imports
                        .Where(t => GetMonthYear(t.TransactionDate.Value) == (year, month))
                        .ToList();

                    var monthlySales = sales
                        .Where(s => GetMonthYear(s.Order.OrderDate.Value) == (year, month))
                        .ToList();

                    var importedQuantity = monthlyImports.Sum(t => t.Quantity);
                    var soldQuantity = monthlySales.Sum(s => s.Quantity);
                    var importPriceAvg = monthlyImports.Any() ? monthlyImports.Average(t => t.UnitCost) : 0;
                    var revenue = monthlySales.Sum(s => s.Quantity * s.UnitPrice);
                    var cost = soldQuantity * importPriceAvg;

                    reports.Add(new MonthlyFishReport
                    {
                        Year = year,
                        Month = month,
                        FishName = fish.FishName,
                        ImportedQuantity = importedQuantity,
                        SoldQuantity = soldQuantity,
                        TotalRevenue = revenue,
                        TotalCost = cost
                    });
                }
            }

            return reports.OrderBy(r => r.Year).ThenBy(r => r.Month).ToList();
        }

        private (int year, int month) GetMonthYear(DateTime date)
        {
            return (date.Year, date.Month);
        }
    }
}
