using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pgDataAccess.Data;
using pgDataAccess.Models;

namespace pgDataAccess.Services
{
    /// <summary>
    /// Сервис для работы с заказами
    /// </summary>
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService()
        {
            _context = new AppDbContext();
        }

        /// <summary>
        /// Получить все заказы с загрузкой связанных данных
        /// </summary>
        public List<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.Client)
                .Include(o => o.PickupPoint)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        /// <summary>
        /// Получить заказ по ID
        /// </summary>
        public Order GetOrderById(int id)
        {
            return _context.Orders
                .Include(o => o.Client)
                .Include(o => o.PickupPoint)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == id);
        }

        /// <summary>
        /// Добавить новый заказ
        /// </summary>
        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        /// <summary>
        /// Обновить заказ
        /// </summary>
        public void UpdateOrder(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
            _context.SaveChanges();
        }

        /// <summary>
        /// Удалить заказ
        /// </summary>
        public void DeleteOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Получить все пункты выдачи
        /// </summary>
        public List<PickupPoint> GetAllPickupPoints()
        {
            return _context.PickupPoints.ToList();
        }

        /// <summary>
        /// Получить всех клиентов (пользователи с ролью "Авторизированный клиент")
        /// </summary>
        public List<User> GetClients()
        {
            return _context.Users
                .Where(u => u.Role.Name == "Авторизированный клиент")
                .ToList();
        }
    }
}