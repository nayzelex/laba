using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pgDataAccess.Models;

namespace pgDataAccess.Data
{
    /// <summary>
    /// Контекст базы данных для работы с PostgreSQL
    /// </summary>
    public class AppDbContext : DbContext
    {
        // DbSet - коллекции, соответствующие таблицам в БД
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PickupPoint> PickupPoints { get; set; }

        /// <summary>
        /// Настройка подключения к БД
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Строка подключения: фамилия студента в нижнем регистре
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=sadiullina;Username=app;Password=123456789");
        }

        /// <summary>
        /// Настройка моделей и связей
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка составного первичного ключа для OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderId, oi.ProductArticle });

            // Настройка связей
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // При удалении заказа удаляются его позиции

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductArticle)
                .OnDelete(DeleteBehavior.Restrict); // Нельзя удалить товар, если он в заказе

            // Настройка связей для User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Уникальность логина
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            // Настройка Product
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);
        }
    }
}