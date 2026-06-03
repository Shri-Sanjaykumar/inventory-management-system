using Microsoft.EntityFrameworkCore;
using InternInventory.Models;

namespace InternInventory.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Vendor> Vendors { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<StockReceipt> StockReceipts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Entity-specific rules and relational mapping
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.HasIndex(e => e.FirstName);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasIndex(e => e.ProjectName);
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasIndex(e => e.ItemName);
            });

            // Stock Receipt Relationships Mapping
            modelBuilder.Entity<StockReceipt>(entity =>
            {
                entity.HasKey(e => e.StockReceiptID);
                entity.HasIndex(e => e.ReceiptDate);

                entity.HasOne(d => d.Vendor)
                    .WithMany()
                    .HasForeignKey(d => d.VendorID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Project)
                    .WithMany()
                    .HasForeignKey(d => d.ProjectID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Item)
                    .WithMany()
                    .HasForeignKey(d => d.ItemID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
