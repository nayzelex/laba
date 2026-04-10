using pgDataAccess.Data;
using pgDataAccess.Models;
using pgDataAccess.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp.Views
{
    public partial class OrderEditorWindow : Window
    {
        private readonly User _currentUser;
        private readonly Order _editingOrder;
        private readonly OrderService _orderService;
        private readonly bool _isEditing;

        public OrderEditorWindow(User user, Order order = null)
        {
            InitializeComponent();
            _currentUser = user;
            _editingOrder = order;
            _orderService = new OrderService();
            _isEditing = (order != null);

            LoadComboBoxes();

            if (_isEditing)
            {
                LoadOrderData();
                btnDelete.Visibility = Visibility.Visible;
                Title = "Редактирование заказа";
                txtOrderId.IsEnabled = false;
            }
            else
            {
                Title = "Добавление нового заказа";
                dpOrderDate.SelectedDate = DateTime.Today;
                txtOrderId.Text = "Авто";
                txtOrderId.IsEnabled = false;
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                var pickupPoints = _orderService.GetAllPickupPoints();
                cmbPickupPoint.ItemsSource = pickupPoints;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пунктов выдачи: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrderData()
        {
            if (_editingOrder == null) return;

            txtOrderId.Text = _editingOrder.Id.ToString();

            // ИСПРАВЛЕНИЕ: преобразуем UTC в локальное время для отображения
            dpOrderDate.SelectedDate = _editingOrder.OrderDate.ToLocalTime();

            if (_editingOrder.DeliveryDate.HasValue)
            {
                dpDeliveryDate.SelectedDate = _editingOrder.DeliveryDate.Value.ToLocalTime();
            }
            else
            {
                dpDeliveryDate.SelectedDate = null;
            }

            if (_editingOrder.PickupPointId > 0)
            {
                cmbPickupPoint.SelectedValue = _editingOrder.PickupPointId;
            }

            if (cmbStatus.Items.OfType<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == _editingOrder.Status) is ComboBoxItem statusItem)
            {
                cmbStatus.SelectedItem = statusItem;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                var order = _isEditing ? _editingOrder : new Order();

                order.PickupPointId = (int)cmbPickupPoint.SelectedValue;

                // Исправление дат
                var orderDate = dpOrderDate.SelectedDate.Value;
                order.OrderDate = DateTime.SpecifyKind(orderDate, DateTimeKind.Utc);

                if (dpDeliveryDate.SelectedDate.HasValue)
                {
                    var deliveryDate = dpDeliveryDate.SelectedDate.Value;
                    order.DeliveryDate = DateTime.SpecifyKind(deliveryDate, DateTimeKind.Utc);
                }

                order.Status = ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString();
                order.PickupCode = "0000";

                if (_isEditing)
                {
                    order.ClientId = _editingOrder.ClientId;
                    _orderService.UpdateOrder(order);
                    MessageBox.Show("Заказ обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    using (var db = new pgDataAccess.Data.AppDbContext())
                    {
                        var firstClient = db.Users.FirstOrDefault(u => u.RoleId == 1);
                        if (firstClient != null)
                        {
                            order.ClientId = firstClient.Id;

                            // ===============================================
                            // ВОТ ЗДЕСЬ - ВРУЧНУЮ ПРИСВАИВАЕМ ID (обход авто-генерации)
                            // ===============================================
                            int maxId = db.Orders.Any() ? db.Orders.Max(o => o.Id) : 0;
                            order.Id = maxId + 1;  // Присваиваем ID вручную!
                                                   // ===============================================

                            db.Orders.Add(order);
                            db.SaveChanges();
                            MessageBox.Show($"Заказ добавлен! ID: {order.Id}", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Нет клиентов в БД!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}\n\n{ex.InnerException?.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить заказ №{_editingOrder.Id}?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _orderService.DeleteOrder(_editingOrder.Id);
                    MessageBox.Show("Заказ успешно удален!", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (cmbPickupPoint.SelectedValue == null)
            {
                MessageBox.Show("Выберите пункт выдачи!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!dpOrderDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Укажите дату заказа!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (dpDeliveryDate.SelectedDate.HasValue && dpDeliveryDate.SelectedDate.Value < dpOrderDate.SelectedDate.Value)
            {
                MessageBox.Show("Дата доставки не может быть раньше даты заказа!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус заказа!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}