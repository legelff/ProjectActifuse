using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
using MySqlConnector;

namespace ProjectActifuse
{
    /// <summary>
    /// Interaction logic for Support.xaml
    /// </summary>
    public partial class Support : Page
    {
        public string Username { get; private set; }

        public Support(string username)
        {
            InitializeComponent();
            Username = username;
        }

        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            // Get the message from the MessageBox
            string message = UserMessageBox.Text;

            try
            {
                // Connection string to connect to your MySQL database
                string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";

                // Define the query to retrieve user's email and name based on username
                string getUserDetailsQuery = "SELECT Email, Name FROM Users WHERE Username = @username";

                // Create MySqlConnection object
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Open database connection
                    await connection.OpenAsync();

                    // Create MySqlCommand object with the query and connection
                    using (MySqlCommand getUserDetailsCommand = new MySqlCommand(getUserDetailsQuery, connection))
                    {
                        // Add parameters to the query
                        getUserDetailsCommand.Parameters.AddWithValue("@username", Username);

                        // Execute the query and retrieve the user's email and name
                        using (MySqlDataReader reader = await getUserDetailsCommand.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                // Get user's email and name from the database
                                string email = reader.GetString("Email");
                                string name = reader.GetString("Name");

                                // Create and send the email
                                using (MailMessage mail = new MailMessage())
                                {
                                    mail.From = new MailAddress("legend1409pc@gmail.com", "Actifuse Support");
                                    mail.To.Add("r0984834@student.thomasmore.be");
                                    mail.To.Add("r0975016@student.thomasmore.be");
                                    mail.Subject = "Actifuse Support Request";

                                    // Set IsBodyHtml to true to allow HTML formatting
                                    mail.IsBodyHtml = true;

                                    // Use HTML formatting in the email body
                                    mail.Body = $"<p>Hello Team Actifuse</p><br><p>{message}</p><br><p>Regards<br><br>{name} aka {Username}<br>{email}</p>";

                                    using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                                    {
                                        smtpClient.Port = 587; // Port number for SMTP server
                                        smtpClient.Credentials = new NetworkCredential("legend1409pc@gmail.com", "oquqetfunielktzy");
                                        smtpClient.EnableSsl = true; // Enable SSL

                                        await smtpClient.SendMailAsync(mail);
                                    }
                                }

                                MessageBox.Show("Your message has been sent successfully!");
                            }
                            else
                            {
                                MessageBox.Show("User details not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while sending the message: " + ex.Message);
            }
        }

    }
}
