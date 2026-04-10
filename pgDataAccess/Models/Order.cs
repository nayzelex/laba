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
    /// Заказ
    /// </summary>
    [Table("orders")]
    public class Order
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("order_date")]
        public DateTime OrderDate { get; set; }      // Дата заказа
        [Column("delivery_date")]
        public DateTime? DeliveryDate { get; set; }  // Дата выдачи (может быть NULL)
        [Column("status")]
        public string Status { get; set; } = string.Empty;           // Статус заказа: "Новый", "Завершен"
        [Column("pickup_code")]
        public string PickupCode { get; set; } = string.Empty;       // Код получения заказа

        // Внешние ключи
        [Column("client_id")]
        public int ClientId { get; set; }
        [Column("pickup_point_id")]
        public int PickupPointId { get; set; }

        // Навигационные свойства
        public virtual User Client { get; set; } = null!;
        public virtual PickupPoint PickupPoint { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}