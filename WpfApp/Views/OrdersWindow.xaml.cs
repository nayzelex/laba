using System;
using System.Windows;
using System.Windows.Input;
using pgDataAccess.Models;
using pgDataAccess.Services;

namespace WpfApp.Views
{
    public partial class OrdersWindow : Window
    {
        private readonly User _currentUser;
        private readonly OrderService _orderService;
        private readonly bool _isAdmin;

        public OrdersWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _orderService = new OrderService();
            _isAdmin = (user != null && user.Role.Name == "Администратор");

            if (_isAdmin)
            {
                btnAddOrder.Visibility = Visibility.Visible;
            }

            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                txtStatus.Text = "Загрузка заказов...";
                var orders = _orderService.GetAllOrders();
                lbOrders.ItemsSource = orders;
                txtStatus.Text = $"Заказов: {orders.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Order_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_isAdmin)
            {
                var selectedOrder = lbOrders.SelectedItem as Order;
                if (selectedOrder != null)
                {
                    var editorWindow = new OrderEditorWindow(_currentUser, selectedOrder);
                    editorWindow.Owner = this;
                    editorWindow.ShowDialog();
                    LoadOrders();
                }
            }
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            var editorWindow = new OrderEditorWindow(_currentUser, null);
            editorWindow.Owner = this;
            editorWindow.ShowDialog();
            LoadOrders();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }
    }
}