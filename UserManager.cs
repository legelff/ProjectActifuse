using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectActifuse
{
    public static class UserManager
    {
        public static string Username { get; private set; }
        public static int UserId { get; private set; } 

        public static void SetUsername(string username)
        {
            Username = username;
        }

        public static async Task<int> GetUserId()
        {
            try
            {
                // Connection string to connect to your MySQL database
                string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";

                // Define the query to retrieve UserId from users table based on Username
                string query = "SELECT UserId FROM users WHERE Username = @username";

                // Create MySqlConnection object
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Open database connection
                    await connection.OpenAsync();

                    // Create MySqlCommand object with the query and connection
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Add parameters to the query
                        command.Parameters.AddWithValue("@username", Username);

                        // Execute the query and retrieve the UserId
                        object result = await command.ExecuteScalarAsync();

                        // Check if the result is not null and convert it to an integer
                        if (result != null && result != DBNull.Value)
                        {
                            UserId = Convert.ToInt32(result);
                            return UserId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Return -1 if UserId retrieval fails
            return -1;
        }

        // New methods for managing users (admin):
        public static async Task<List<User>> GetAllUsers()
        {
            var users = new List<User>();
            try
            {
                string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";
                string query = "SELECT UserId, Username, Password, Email, IsAdmin FROM users";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                users.Add(new User
                                {
                                    UserId = reader.GetInt32("UserId"),
                                    Username = reader.GetString("Username"),
                                    Password = reader.GetString("Password"),
                                    IsAdmin = reader.GetBoolean("IsAdmin"),
                                    Email = reader.GetString("Email")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return users;
        }

        public static async Task<bool> AddUser(User user)
        {
            try
            {
                string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";
                string query = "INSERT INTO users (Username, Password, Email, IsAdmin) VALUES (@username, @password, @email, @isAdmin)";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", user.Username);
                        command.Parameters.AddWithValue("@password", user.Password);
                        command.Parameters.AddWithValue("@isAdmin", user.IsAdmin);
                        command.Parameters.AddWithValue("@email", user.Email);

                        int result = await command.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return false;
        }

        public static async Task<bool> UpdateUser(User user)
        {
            try
            {
                string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";
                string query = "UPDATE users SET Username = @username, Password = @password, Email = @email, IsAdmin = @isAdmin WHERE UserId = @userId";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", user.Username);
                        command.Parameters.AddWithValue("@password", user.Password);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@isAdmin", user.IsAdmin);
                        command.Parameters.AddWithValue("@userId", user.UserId);

                        int result = await command.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return false;
        }

        public static async Task<bool> DeleteUser(int userId)
        {
            try
            {
                string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";
                string query = "DELETE FROM users WHERE UserId = @userId";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        int result = await command.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return false;
        }
    }
}