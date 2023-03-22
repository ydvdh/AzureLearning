using AzureFunction.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunction.Data
{
    public class AzureFunctionDbContext : DbContext
    {
        public AzureFunctionDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<SalesRequest> SalesRequests { get; set; }
        public DbSet<GroceryItem> GroceryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SalesRequest>(entity =>
            {
                entity.HasKey(c => c.Id);
            });
            modelBuilder.Entity<GroceryItem>(entity =>
            {
                entity.HasKey(c => c.Id);
            });
        }
    }
}
