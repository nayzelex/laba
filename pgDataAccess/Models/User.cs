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
    /// Модель пользователя системы
    /// </summary>
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("login")]
        public string Login { get; set; } = string.Empty;          // Email пользователя
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;   // Хэш пароля (в реальном проекте хэшировать)
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;       // ФИО пользователя

        // Внешний ключ к таблице ролей
        [Column("role_id")]
        public int RoleId { get; set; }

        // Навигационные свойства
        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
