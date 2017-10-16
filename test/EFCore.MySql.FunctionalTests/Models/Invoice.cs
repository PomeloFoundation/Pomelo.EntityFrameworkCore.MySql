using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models
{
    public static class InvoiceMeta
    {
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(x => new { x.Category, x.Id });

                entity.HasMany(x => x.Items)
                    .WithOne()
                    .HasForeignKey(x => new { x.Category, x.InvoiceId })
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.ToTable("InvoiceItems");
                entity.HasKey(x => new { x.Category, x.InvoiceId, x.Description });
            });
        }
    }

    public class Invoice
    {
        public string Category { get; set; }
        public int Id { get; set; }

        [ConcurrencyCheck]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }

    public class InvoiceItem
    {
        public string Category { get; set; }
        public int InvoiceId { get; set; }

        public string Description { get; set; }
        public bool IsPaid { get; set; }
    }
}
