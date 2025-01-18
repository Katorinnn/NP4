using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPG_Management_System.Models
{
    class DataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data source = NP4POS.db");  // Connect to SQLite database
        }
        public DbSet<InventoryTable> tbl_inventory { get; set; }
        public DbSet<AdminTable> tbl_admin { get; set; }
        public DbSet<CustomersTable> tbl_customers { get; set; }
        public DbSet<ReportsTable> tbl_reports { get; set; }

        public DbSet<StocksTable> tbl_stocks { get; set; }
    }
}
