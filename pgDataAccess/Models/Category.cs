using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace pgDataAccess.Models
{
    /// <summary>
    /// Категория товара (Женская обувь, Мужская обувь)
    /// </summary>
    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        // Навигационное свойство: один ко многим с товарами
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
