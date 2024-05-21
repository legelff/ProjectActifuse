using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using MySqlConnector;

namespace ProjectActifuse
{
    /// <summary>
    /// Interaction logic for ForgotPasswordStep2.xaml
    /// </summary>
    public partial class ForgotPasswordStep2 : Window
    {
        private string Email;
        public ForgotPasswordStep2(string email)
        {
            InitializeComponent();
            Email = email;
        }

        private void TitleBarMinimize(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        private void TitleBarMaximize(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void TitleBarClose(object sender, RoutedEventArgs e)
        {
            // go back to login page
            Authentication authenticationWindow = new Authentication();
            authenticationWindow.Show();

            // Close the current window
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private MySqlConnection connection;
        private string connectionString = "datasource=127.0.0.1; port=3306; username = root; password=; database=actifuse;";

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            if (NewPasswordTextBox.Text == ConfirmPasswordTextBox.Text)
            {
                using (connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string newPassword = EncryptPassword(NewPasswordTextBox.Text);

                        // Prepare the SQL query to update the password
                        string query = "UPDATE users SET password = @NewPassword WHERE email = @Email";

                        // Create and configure the command
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@NewPassword", newPassword);
                            command.Parameters.AddWithValue("@Email", Email);

                            // Execute the command
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Password reset successfully!");

                                // go back to login page
                                Authentication authenticationWindow = new Authentication();
                                authenticationWindow.Show();

                                // Close the current window
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Password could not be updated.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }

            else
            {
                MessageBox.Show("Passwords do not match!");
            }
        }

        private string EncryptPassword(string password)
        {
            // salt
            string salt = "l145";

            // Concatenate the salt with the password
            string saltedPassword = password + salt;

            // Implement encryption logic
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }
    }
}
