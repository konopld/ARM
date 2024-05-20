using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARM
{
    public class UserService
    {
        private readonly DatabaseService _dbService;

        public UserService(DatabaseService dbService)
        {
            // перевірка на null для dbService
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        }

        // метод для аутентифікації користувача
        public User? AuthenticateUser(string username, string password)
        {
            User? user = _dbService.DatabaseAuthenticateUser(username, password);

            return user;
        }
    }

    public class User
    {
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
    }
}
