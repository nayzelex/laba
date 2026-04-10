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
    /// Сервис для работы с товарами (фильтрация, сортировка, поиск)
    /// </summary>
    public class ProductService
    {
        private readonly AppDbContext _context;

        public ProductService()
        {
            _context = new AppDbContext();
        }

        /// <summary>
        /// Получить все товары с загрузкой связанных данных
        /// </summary>
        public List<Product> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .ToList();
        }

        /// <summary>
        /// Получить товары с фильтрацией, поиском и сортировкой
        /// </summary>
        public List<Product> GetProductsFiltered(string searchText, int? supplierId, string sortDirection)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .AsQueryable();

            // Фильтрация по поставщику (если выбран конкретный поставщик)
            if (supplierId.HasValue && supplierId.Value > 0)
            {
                query = query.Where(p => p.SupplierId == supplierId.Value);
            }

            // Поиск по текстовым полям (одновременный поиск по нескольким атрибутам)
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.ToLower();
                query = query.Where(p =>
                    p.Article.ToLower().Contains(searchText) ||
                    p.Name.ToLower().Contains(searchText) ||
                    p.Description.ToLower().Contains(searchText) ||
                    p.Category.Name.ToLower().Contains(searchText) ||
                    p.Manufacturer.Name.ToLower().Contains(searchText) ||
                    p.Supplier.Name.ToLower().Contains(searchText)
                );
            }

            // Сортировка по количеству на складе
            if (sortDirection == "asc")
                query = query.OrderBy(p => p.StockQuantity);
            else if (sortDirection == "desc")
                query = query.OrderByDescending(p => p.StockQuantity);
            else
                query = query.OrderBy(p => p.Name); // По умолчанию по имени

            return query.ToList();
        }

        /// <summary>
        /// Получить товар по артикулу
        /// </summary>
        public Product GetProductByArticle(string article)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .FirstOrDefault(p => p.Article == article);
        }

        /// <summary>
        /// Добавить новый товар
        /// </summary>
        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        /// <summary>
        /// Обновить существующий товар
        /// </summary>
        public void UpdateProduct(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
        }

        /// <summary>
        /// Удалить товар (только если нет в заказах)
        /// </summary>
        public bool DeleteProduct(string article)
        {
            var product = _context.Products.Find(article);
            if (product == null) return false;

            // Проверка: есть ли товар в заказах
            var isInOrders = _context.OrderItems.Any(oi => oi.ProductArticle == article);
            if (isInOrders) return false; // Нельзя удалить

            _context.Products.Remove(product);
            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Получить всех поставщиков (для фильтрации)
        /// </summary>
        public List<Supplier> GetAllSuppliers()
        {
            return _context.Suppliers.ToList();
        }

        /// <summary>
        /// Получить все категории
        /// </summary>
        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        /// <summary>
        /// Получить всех производителей
        /// </summary>
        public List<Manufacturer> GetAllManufacturers()
        {
            return _context.Manufacturers.ToList();
        }

        /// <summary>
        /// Проверить, существует ли товар в заказах
        /// </summary>
        public bool IsProductInOrders(string article)
        {
            return _context.OrderItems.Any(oi => oi.ProductArticle == article);
        }
    }
}