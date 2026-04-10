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
    /// Пункт выдачи заказов
    /// </summary>
    [Table("pickup_points")]
    public class PickupPoint
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("address")]
        public string Address { get; set; } = string.Empty;      // Адрес пункта выдачи

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}