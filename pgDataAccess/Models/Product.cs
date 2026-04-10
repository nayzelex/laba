using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pgDataAccess.Models
{
    /// <summary>
    /// Товар (обувь)
    /// </summary>
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("article")]
        public string Article { get; set; } = string.Empty;      // Первичный ключ - артикул товара
        [Column("name")]
        public string Name { get; set; } = string.Empty;        // Наименование товара
        [Column("unit")]
        public string Unit { get; set; } = string.Empty;         // Единица измерения (шт., пара)
        [Column("price")]
        public decimal Price { get; set; }       // Цена
        [Column("discount_percent")]
        public int DiscountPercent { get; set; } // Скидка в процентах
        [Column("stock_quantity")]
        public int StockQuantity { get; set; }   // Количество на складе
        [Column("description")]
        public string Description { get; set; } = string.Empty; // Описание товара
        [Column("photo_path")]
        public string? PhotoPath { get; set; }    // Путь к изображению

        // Внешние ключи
        [Column("category_id")]
        public int CategoryId { get; set; }
        [Column("manufacturer_id")]
        public int ManufacturerId { get; set; }
        [Column("supplier_id")]
        public int SupplierId { get; set; }

        // Навигационные свойства
        public virtual Category Category { get; set; } = null!;
        public virtual Manufacturer Manufacturer { get; set; } = null!;
        public virtual Supplier Supplier { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        /// <summary>
        /// Вычисляемое свойство: цена со скидкой
        /// </summary>
        public decimal FinalPrice => Price * (100 - DiscountPercent) / 100;
    }
}