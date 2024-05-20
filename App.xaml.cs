using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace ARM
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        // поточний користувач
        public User CurrentUser { get; set; }

        // метод, що виконується при запуску програми
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // створення колекції сервісів
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // побудова постачальника послуг
            ServiceProvider = serviceCollection.BuildServiceProvider();

            // початкова форма
            var loginForm = ServiceProvider.GetRequiredService<LoginForm>();
            loginForm.Show();
        }

        // метод для конфігурації сервісів
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DatabaseService>(); // додавання сервісу бази даних
            services.AddSingleton<UserService>(); // додавання сервісу користувача
            services.AddTransient<LoginForm>(); // додавання форми для входу
        }
    }
}
