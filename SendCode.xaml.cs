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
    /// Interaction logic for SendCode.xaml
    /// </summary>
    public partial class SendCode : Page
    {
        public SendCode()
        {
            InitializeComponent();
        }

        private MySqlConnection connection;
        private string connectionString = "datasource=127.0.0.1; port=3306; username = root; password=; database=actifuse;";

        private async void SendCode_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;

            try
            {
                using (connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM Users WHERE Email=@Email;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);

                    int userCount = Convert.ToInt32(command.ExecuteScalar());

                    if (userCount > 0)
                    {
                        // Generate a random 6-digit code
                        Random rand = new Random();
                        int code = rand.Next(100000, 999999);

                        // Store the code and its expiry time
                        DateTime expiryTime = DateTime.Now.AddMinutes(5);

                        query = "UPDATE Users SET VerificationCode = @Code, VerificationCodeExpiry = @Expiry WHERE Email = @Email;";
                        using (MySqlCommand updateCommand = new MySqlCommand(query, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@Code", code);
                            updateCommand.Parameters.AddWithValue("@Expiry", expiryTime);
                            updateCommand.Parameters.AddWithValue("@Email", email);

                            await updateCommand.ExecuteNonQueryAsync();
                        }

                        // Send the code to the user's email address asynchronously
                        using (MailMessage mail = new MailMessage())
                        {
                            mail.From = new MailAddress("noreplyprojectactifuse@gmail.com", "Actifuse");
                            mail.To.Add(email);
                            mail.Subject = "Forgot Password Code";
                            mail.Body = $"Your verification code is: {code}. Code expires in 5 minutes.";

                            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                            {
                                smtpClient.Port = 587; // Port number for SMTP server
                                smtpClient.Credentials = new NetworkCredential("noreplyprojectactifuse@gmail.com", "jckkfdpfahygfhjh");
                                smtpClient.EnableSsl = true; // Enable SSL

                                await smtpClient.SendMailAsync(mail);
                            }
                        }

                        MessageBox.Show("Code sent to your e-mail!");
                    }
                    else
                    {
                        MessageBox.Show("Email not found!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = EmailTextBox.Text;
                int submittedCode = Int32.Parse(CodeTextBox.Text);

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT VerificationCode, VerificationCodeExpiry FROM Users WHERE Email = @Email;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int? savedCode = reader["VerificationCode"] as int?;
                            DateTime? expiryTime = reader["VerificationCodeExpiry"] as DateTime?;

                            // Check if savedCode and expiryTime are not null
                            if (savedCode.HasValue && expiryTime.HasValue)
                            {
                                // Check if the submitted code matches the saved code and if it's within the expiry time limit
                                if (submittedCode == savedCode && DateTime.Now < expiryTime)
                                {
                                    // Navigate to PasswordChange.xaml page in the frame of the parent window (ForgotPassword.xaml)
                                    if (this.Parent is ForgotPassword forgotPasswordWindow)
                                    {
                                        forgotPasswordWindow.ForgotPasswordFrame.Source = new Uri("PasswordChange.xaml", UriKind.Relative);
                                        MessageBox.Show("Development of this feature in progress!");
                                    }
                                }
                                else
                                {
                                    if (DateTime.Now > expiryTime)
                                    {
                                        MessageBox.Show("Code expired! Please request a new code.");
                                    }

                                    else if (submittedCode != savedCode)
                                    {
                                        MessageBox.Show("Incorrect code!");
                                    }

                                    else
                                    {
                                        MessageBox.Show("Something went wrong! Please try again later.");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Enter your e-mail and request a code first.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("The email was not found in our database.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}
