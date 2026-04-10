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
    /// Модель роли пользователя
    /// </summary>
    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; } = string.Empty;  // Название роли: "Авторизированный клиент", "Менеджер", "Администратор"

        // Навигационное свойство: один ко многим с пользователями
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}