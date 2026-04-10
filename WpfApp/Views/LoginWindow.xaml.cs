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
using pgDataAccess.Services;
using WpfApp.Views;

namespace WpfApp.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;

        public LoginWindow()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        /// <summary>
        /// Обработчик кнопки "Войти"
        /// </summary>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем введенные данные
                string login = txtLogin.Text.Trim();
                string password = txtPassword.Password;

                // Проверка на пустые поля
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Пожалуйста, введите логин и пароль",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Аутентификация пользователя
                var user = _authService.Authenticate(login, password);

                if (user != null)
                {
                    // Успешный вход - открываем главное окно
                    var mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль",
                        "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}",
                    "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Войти как гость"
        /// </summary>
        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Гость - передаем null вместо пользователя
                var mainWindow = new MainWindow(null);
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}