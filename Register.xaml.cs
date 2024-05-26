using Microsoft.Win32;
using MySqlConnector;
using ProjectActifuse.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
using System.Security.Cryptography;

namespace ProjectActifuse
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Page, INotifyPropertyChanged
    {
        public Register()
        {
            InitializeComponent();
            DataContext = this; // allow data binding
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        // Private field for the selected profile picture
        private BitmapImage _selectedProfilePicture;

        // Property to hold the selected profile picture
        public BitmapImage SelectedProfilePicture
        {
            get { return _selectedProfilePicture; }
            set
            {
                _selectedProfilePicture = value;
                OnPropertyChanged(nameof(SelectedProfilePicture));
            }
        }

        private void OpenFileExplorer_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                // Load the selected image into the SelectedProfilePicture property
                SelectedProfilePicture = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = NewUserTextBox.Text;
            string email = NewEmailTextBox.Text;
            string password = EncryptPassword(NewPasswordTextBox.Text);
            string name = NewNameTextBox.Text;
            string imagePath = SaveProfilePicture();

            // Check if username or email already exists
            if (CheckIfUserExists(username, email))
            {
                MessageBox.Show("Username or email already exists.");
                return;
            }

            // Insert the user data into the database
            string connectionString = "datasource=127.0.0.1; port=3306; username = root; password=; database=actifuse;";
            string query = "INSERT INTO Users (Username, Password, Email, Name, ProfilePicturePath) VALUES (@Username, @Password, @Email, @Name, @ProfilePicturePath)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@ProfilePicturePath", imagePath);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("User registered successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Failed to register user.");
                    }
                }
            }
        }

        private bool CheckIfUserExists(string username, string email)
        {
            // Query the database to check if the username or email already exists
            string connectionString = "datasource=127.0.0.1; port=3306; username = root; password=; database=actifuse;";
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username OR Email = @Email";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
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

        private string SaveProfilePicture()
        {
            if (SelectedProfilePicture != null)
            {
                string directoryPath = "Images"; // Set the path
                string fileName = Guid.NewGuid().ToString() + ".jpg"; // Generate a unique file name
                string fullPath = System.IO.Path.Combine(directoryPath, fileName);

                // Create directory if it doesn't exist
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Convert BitmapImage to Bitmap
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(SelectedProfilePicture));
                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                return fullPath; // Return the path where the image is saved
            }
            else
            {
                return null; // Return null if no image is selected
            }
        }
    }
}
