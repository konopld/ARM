using System;
using System.Windows;
using System.Windows.Controls;

namespace ARM
{
    public partial class LoginForm : Window
    {
        private readonly DatabaseService _dbService;
        private readonly UserService _userService;

        // конструктор, що приймає DatabaseService та UserService як залежності
        public LoginForm(DatabaseService dbService, UserService userService)
        {
            InitializeComponent();
            // перевірка на null для dbService та userService
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // обробник події натискання кнопки входу
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // отримання введеного імені користувача та пароля
            string username = UsernameTextBox.Text;
            string password = PasswordFieldBox.Password;

            // аутентифікація користувача
            var user = _userService.AuthenticateUser(username, password);
            if (user != null)
            {
                // отримання поточного додатку та встановлення поточного користувача
                var app = (App)Application.Current;
                app.CurrentUser = user;

                // показ повідомлення про успішний вхід
                MessageBox.Show($"Login successful! Welcome, {user.Username}. Admin status: {user.IsAdmin}");

                // відкриваємо головне вікно
                var mainForm = new MainForm(_dbService, _userService);
                mainForm.Show();

                // закриваємо вікно логіну
                Close();
            }
            else
            {
                // показ повідомлення про невдалий вхід
                MessageBox.Show("Invalid username or password.");
            }
        }
    }
}
