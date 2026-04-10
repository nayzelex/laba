using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using pgDataAccess.Models;
using pgDataAccess.Services;
using WpfApp.Views;

namespace WpfApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;
        private readonly ProductService _productService;
        private readonly OrderService _orderService;
        private List<Product> _allProducts;
        private string _currentSearchText = "";
        private int? _currentSupplierFilter = null;
        private string _currentSortDirection = "";

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _productService = new ProductService();
            _orderService = new OrderService();

            // ВКЛЮЧАЕМ ПРОКРУТКУ
            lbProducts.PreviewMouseWheel += (s, e) =>
            {
                var scrollViewer = FindVisualChild<ScrollViewer>(lbProducts);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3);
                    e.Handled = true;
                }
            };

            InitializeUI();
            LoadProducts();
        }

        /// <summary>
        /// Настройка интерфейса в зависимости от роли пользователя
        /// </summary>
        private void InitializeUI()
        {
            if (_currentUser == null)
            {
                // Режим гостя
                txtUserInfo.Text = "Гость";
                btnLogout.Visibility = Visibility.Visible;
                panelFilters.Visibility = Visibility.Collapsed;
                btnAddProduct.Visibility = Visibility.Collapsed;
                btnOrders.Visibility = Visibility.Collapsed;
                txtStatus.Text = "Вы вошли как гость. Только просмотр товаров.";
            }
            else
            {
                // Отображаем ФИО пользователя
                txtUserInfo.Text = $"{_currentUser.FullName} ({_currentUser.Role.Name})";

                // Настройка прав доступа в зависимости от роли
                if (_currentUser.Role != null)
                {
                    switch (_currentUser.Role.Name)
                    {
                        case "Администратор":
                            panelFilters.Visibility = Visibility.Visible;
                            btnAddProduct.Visibility = Visibility.Visible;
                            btnOrders.Visibility = Visibility.Visible;
                            LoadSuppliersToFilter();
                            break;

                        case "Менеджер":
                            panelFilters.Visibility = Visibility.Visible;
                            btnAddProduct.Visibility = Visibility.Collapsed;
                            btnOrders.Visibility = Visibility.Visible;
                            LoadSuppliersToFilter();
                            break;

                        case "Авторизированный клиент":
                            panelFilters.Visibility = Visibility.Collapsed;
                            btnAddProduct.Visibility = Visibility.Collapsed;
                            btnOrders.Visibility = Visibility.Collapsed;
                            break;
                    }
                }
            }
        }



        /// <summary>
        /// Загрузка списка поставщиков для фильтрации
        /// </summary>
        private void LoadSuppliersToFilter()
        {
            var suppliers = _productService.GetAllSuppliers();
            cmbSupplier.Items.Clear();

            // Первый элемент - "Все поставщики"
            cmbSupplier.Items.Add(new { Id = 0, Name = "Все поставщики" });

            foreach (var supplier in suppliers)
            {
                cmbSupplier.Items.Add(new { Id = supplier.Id, Name = supplier.Name });
            }

            cmbSupplier.DisplayMemberPath = "Name";
            cmbSupplier.SelectedValuePath = "Id";
            cmbSupplier.SelectedIndex = 0;
        }

        /// <summary>
        /// Загрузка товаров с применением фильтров
        /// </summary>
        private void LoadProducts()
        {
            try
            {
                txtStatus.Text = "Загрузка товаров...";

                _allProducts = _productService.GetProductsFiltered(
                    _currentSearchText,
                    _currentSupplierFilter,
                    _currentSortDirection);

                foreach (var product in _allProducts)
                {
                    if (string.IsNullOrEmpty(product.PhotoPath))
                    {
                        product.PhotoPath = "/images/picture.png";
                    }
                }

                // Используем ItemsControl через lbProducts
                lbProducts.ItemsSource = _allProducts;
                txtStatus.Text = $"Загружено товаров: {_allProducts.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Ошибка загрузки";
            }
        }
        /// <summary>
        /// Поиск в реальном времени
        /// </summary>
        private void SearchText_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _currentSearchText = txtSearch.Text;
            LoadProducts();
        }

        /// <summary>
        /// Фильтрация по поставщику в реальном времени
        /// </summary>
        private void SupplierFilter_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbSupplier.SelectedItem != null)
            {
                dynamic selected = cmbSupplier.SelectedItem;
                int supplierId = selected.Id;
                _currentSupplierFilter = supplierId == 0 ? (int?)null : supplierId;
                LoadProducts();
            }
        }

        /// <summary>
        /// Сортировка в реальном времени
        /// </summary>
        private void Sort_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbSort.SelectedItem is System.Windows.Controls.ComboBoxItem selected)
            {
                _currentSortDirection = selected.Content.ToString() == "По возрастанию" ? "asc" :
                                        selected.Content.ToString() == "По убыванию" ? "desc" : "";
                LoadProducts();
            }
        }

        /// <summary>
        /// Двойной клик по товару для редактирования (только для администратора)
        /// </summary>
        private void Product_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_currentUser != null && _currentUser.Role?.Name == "Администратор")
            {
                var selectedProduct = lbProducts.SelectedItem as Product;
                if (selectedProduct != null)
                {
                    OpenProductEditor(selectedProduct);
                }
            }
        }

        /// <summary>
        /// Открытие окна редактирования/добавления товара
        /// </summary>
        private void OpenProductEditor(Product product = null)
        {
            try
            {
                var editorWindow = new ProductEditorWindow(_currentUser, product);
                editorWindow.Owner = this;

                // Блокируем открытие нескольких окон
                editorWindow.ShowDialog();

                // После закрытия редактора обновляем список товаров
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии редактора: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Добавление нового товара
        /// </summary>
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            OpenProductEditor(null);
        }

        /// <summary>
        /// Открытие окна заказов
        /// </summary>
        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ordersWindow = new OrdersWindow(_currentUser);
                ordersWindow.Owner = this;
                ordersWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии заказов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Выход из системы
        /// </summary>
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из системы?",
                "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
        // Вспомогательный метод для поиска ScrollViewer
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    return result;
                var descendant = FindVisualChild<T>(child);
                if (descendant != null)
                    return descendant;
            }
            return null;
        }
    }
}