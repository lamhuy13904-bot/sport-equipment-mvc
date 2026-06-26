using Microsoft.EntityFrameworkCore;
using SportEquipment.Mvc.Models;

namespace SportEquipment.Mvc.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Equipment> Equipments => Set<Equipment>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().Property(c => c.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Equipment>().Property(e => e.Name).IsRequired().HasMaxLength(150);
            modelBuilder.Entity<Equipment>().Property(e => e.Code).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Equipment>().Property(e => e.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OrderItem>().Property(o => o.UnitPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            // 1. Cấu hình RowVersion cho Concurrency Check
            modelBuilder.Entity<Equipment>().Property(e => e.RowVersion).IsRowVersion();
            // 2. Global Query Filter: Tự động loại bỏ các sản phẩm đã xóa mềm (IsDeleted = true)
            modelBuilder.Entity<Equipment>().HasQueryFilter(e => !e.IsDeleted);

            // Seed Data mẫu
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Giày thể thao" },
                new Category { Id = 2, Name = "Bóng thi đấu" },
                new Category { Id = 3, Name = "Tạ & Thể hình" }
            );

            modelBuilder.Entity<Equipment>().HasData(
                new Equipment { Id = 1, Code = "SHOE-NK-01", Name = "Nike Air Zoom Pegasus", Price = 3200000, Quantity = 12, CategoryId = 1, CreatedAt = new DateTime(2026, 1, 1), IsDeleted = false, RowVersion = Array.Empty<byte>() },
                new Equipment { Id = 2, Code = "BALL-AD-01", Name = "Adidas Al Rihla Pro", Price = 2500000, Quantity = 3, CategoryId = 2, CreatedAt = new DateTime(2026, 1, 1), IsDeleted = false, RowVersion = Array.Empty<byte>() },
                new Equipment { Id = 3, Code = "DUMB-10KG", Name = "Tạ tay cao su 10kg", Price = 450000, Quantity = 0, CategoryId = 3, CreatedAt = new DateTime(2026, 1, 1), IsDeleted = false, RowVersion = Array.Empty<byte>() }
            );
        }
    }
}