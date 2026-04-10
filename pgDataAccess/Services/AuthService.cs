using Microsoft.EntityFrameworkCore;
using pgDataAccess.Data;
using pgDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pgDataAccess.Services
{
    /// <summary>
    /// Сервис аутентификации пользователей
    /// </summary>
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService()
        {
            _context = new AppDbContext();
        }
        public User Authenticate(string login, string password)
        {
            // Простейшая проверка - просто ищем пользователя по логину
            // Пароль не проверяем вообще
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == login);

            return user;
        }

        /// <summary>
        /// Проверка логина и пароля
        /// </summary>
        /*public User Authenticate(string login, string password)
        {
            // В реальном проекте пароль должен быть захэширован!
            // Здесь для простоты сравниваем как есть (в БД пароли не захэшированы)
            return _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == login && u.PasswordHash == password);
        }

        /// <summary>
        /// Получить пользователя по ID
        /// </summary>
        public User GetUserById(int id)
        {
            return _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Id == id);
        }*/
    }
}
