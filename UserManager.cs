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
    }
}
