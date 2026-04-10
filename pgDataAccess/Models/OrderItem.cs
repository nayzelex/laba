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
    /// Состав заказа (связь многие ко многим с дополнительным полем quantity)
    /// </summary>
    [Table("order_items")]
    public class OrderItem
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }
        [Column("product_article")]
        public string ProductArticle { get; set; } = string.Empty;
        [Column("quantity")]
        public int Quantity { get; set; }     // Количество товара в заказе

        // Навигационные свойства
        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}