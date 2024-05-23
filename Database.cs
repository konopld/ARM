using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace ARM
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            // рядок підключення до бази даних MySQL
            _connectionString = "";
        }

        // метод для отримання даних з бази даних та відображення їх у DataGrid, буде використовуватися у MainWindow
        public void FetchData(DataGrid dataGrid)
        {
            try
            {
                using MySqlConnection conn = new(_connectionString);
                conn.Open();
                string sql = "SELECT * FROM employees";
                MySqlCommand cmd = new(sql, conn);
                MySqlDataAdapter adapter = new(cmd);

                DataTable dt = new();
                adapter.Fill(dt);

                // установка джерела даних для DataGrid
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                // показ повідомлення про помилку
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public DataRow? GetRecordById(string id)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(_connectionString);
                conn.Open();
                string sql = "SELECT * FROM employees WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // повідомлення про помилку
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }
        public void UpdateRecordById(string id, Dictionary<string, string> updatedData)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(_connectionString);
                conn.Open();
                string sql = "UPDATE employees SET " +
                             "surname = @Surname, " +
                             "department = @Department, " +
                             "birth_year = @BirthYear, " +
                             "employment_year = @EmploymentYear, " +
                             "position = @Position, " +
                             "academic_degree = @AcademicDegree, " +
                             "academic_title = @AcademicTitle " +
                             "WHERE id = @id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Surname", updatedData["Surname"]);
                cmd.Parameters.AddWithValue("@Department", updatedData["Department"]);
                cmd.Parameters.AddWithValue("@BirthYear", updatedData["BirthYear"]);
                cmd.Parameters.AddWithValue("@EmploymentYear", updatedData["EmploymentYear"]);
                cmd.Parameters.AddWithValue("@Position", updatedData["Position"]);
                cmd.Parameters.AddWithValue("@AcademicDegree", updatedData["AcademicDegree"]);
                cmd.Parameters.AddWithValue("@AcademicTitle", updatedData["AcademicTitle"]);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // повідомлення про помилку
                MessageBox.Show($"Error: {ex.Message}");
            }
        }



        // метод для аутентифікації користувача у базі даних
        public User? DatabaseAuthenticateUser(string username, string password)
        {
            try
            {
                using MySqlConnection conn = new(_connectionString);
                conn.Open();

                string sql = "SELECT username, is_admin FROM users WHERE username = @username AND password = @password";
                using MySqlCommand cmd = new(sql, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", PasswordHelper.HashPassword(password));

                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // повернення об'єкта User, якщо користувача знайдено
                    return new User
                    {
                        Username = reader.GetString("username"),
                        IsAdmin = reader.GetBoolean("is_admin")
                    };
                }
                else
                {
                    // повернення null, якщо користувача не знайдено
                    return null;
                }
            }
            catch (Exception ex)
            {
                // показ повідомлення про помилку
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }
    }

    public class PasswordHelper
    {
        // метод для хешування паролю за допомогою SHA256
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
