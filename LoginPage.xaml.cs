using MySqlConnector;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectActifuse
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private Authentication authenticationWindow;

        public LoginPage(Authentication authenticationWindow)
        {
            InitializeComponent();
            this.authenticationWindow = authenticationWindow;
        }

        private MySqlConnection connection;
        private string connectionString = "datasource=127.0.0.1; port=3306; username = root; password=; database=actifuse;";

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = EncryptPassword(PasswordTextBox.Password);

            try
            {
                using (connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM User WHERE Username=@Username AND Password=@Password";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    int userCount = Convert.ToInt32(command.ExecuteScalar());

                    if (userCount > 0)
                    {
                        // Open the MainWindow
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();

                        // Close the Authentication window
                        authenticationWindow.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private string EncryptPassword(string password)
        {
            // Define your salt
            string salt = "l145";

            // Concatenate the salt with the password
            string saltedPassword = password + salt;

            // Implement encryption logic here
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
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
