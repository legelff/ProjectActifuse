using Microsoft.Win32;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Page, INotifyPropertyChanged
    {
        public string Username { get; private set; }

        public Profile(string username)
        {
            InitializeComponent();
            DataContext = this; // Set DataContext to allow data binding
            Username = username;

            // Connect to the database
            string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";
            string query = "SELECT Email, Name FROM Users WHERE Username = @Username";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", Username);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string email = reader.GetString(0);
                                string name = reader.GetString(1);

                                // Set the values in the UI elements
                                EmailStaticBlock.Text = email;
                                NameChangeBox.Text = name;
                            }
                            else
                            {
                                MessageBox.Show("User not found in the database.");
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Handle any MySQL exceptions
                MessageBox.Show($"MySQL Exception: {ex.Message}");
            }

            // Set UsernameChangeBox text
            UsernameChangeBox.Text = username;
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

        private async void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the new values from the UI
            string newName = NameChangeBox.Text;
            string newPassword = NewPasswordBox.Text;
            string currentPassword = CurrentPasswordBox.Text;

            // Connect to the database
            string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";

            try
            {
                // Retrieve the user's current information from the database
                string query = "SELECT Password, ProfilePicturePath FROM Users WHERE Username = @Username";
                string storedPassword;
                string storedProfilePicturePath;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", Username);

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                storedPassword = reader.GetString(0);
                                storedProfilePicturePath = reader.IsDBNull(1) ? null : reader.GetString(1);
                            }
                            else
                            {
                                MessageBox.Show("User not found in the database.");
                                return;
                            }
                        }
                    }
                }

                // Validate the old password if provided
                if (!string.IsNullOrWhiteSpace(currentPassword) && EncryptPassword(currentPassword) != storedPassword)
                {
                    MessageBox.Show("Current password is incorrect.");
                    return;
                }

                // Prepare the SQL query to update user's information
                StringBuilder updateQuery = new StringBuilder("UPDATE Users SET");
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    updateQuery.Append(" Name = @NewName,");
                }
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    updateQuery.Append(" Password = @NewPassword,");
                }
                if (SelectedProfilePicture != null)
                {
                    updateQuery.Append(" ProfilePicturePath = @ProfilePicturePath,");
                }
                updateQuery.Remove(updateQuery.Length - 1, 1); // Remove the last comma

                updateQuery.Append(" WHERE Username = @Username");

                // Connect to the database again to execute the update
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (MySqlCommand updateCommand = new MySqlCommand(updateQuery.ToString(), connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Username", Username);
                        if (!string.IsNullOrWhiteSpace(newName))
                        {
                            updateCommand.Parameters.AddWithValue("@NewName", newName);
                        }
                        if (!string.IsNullOrWhiteSpace(newPassword))
                        {
                            updateCommand.Parameters.AddWithValue("@NewPassword", EncryptPassword(newPassword));
                        }
                        if (SelectedProfilePicture != null)
                        {
                            string profilePicturePath = SaveProfilePicture();
                            if (!string.IsNullOrEmpty(profilePicturePath))
                            {
                                updateCommand.Parameters.AddWithValue("@ProfilePicturePath", profilePicturePath);
                            }
                        }

                        int rowsAffected = await updateCommand.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User information updated successfully!");
                            CurrentPasswordBox.Text = null;
                            NewPasswordBox.Text = null;
                        }
                        else
                        {
                            MessageBox.Show("Failed to update user information.");
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Handle any MySQL exceptions
                MessageBox.Show($"MySQL Exception: {ex.Message}");
            }
        }


        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = CurrentPasswordBox.Text;

            // Connect to the database
            string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";
            string query = "SELECT Password FROM Users WHERE Username = @Username";

            try
            {
                string storedPassword;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", Username);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                storedPassword = reader.GetString(0);
                            }
                            else
                            {
                                MessageBox.Show("User not found in the database.");
                                return;
                            }
                        }
                    }
                }

                // Validate the current password
                if (EncryptPassword(currentPassword) == storedPassword)
                {
                    // Prompt the user for confirmation
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete your account?", "Confirmation", MessageBoxButton.OKCancel);

                    if (result == MessageBoxResult.OK)
                    {
                        // Connect again to the database for deletion
                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            connection.Open();

                            string deleteQuery = "DELETE FROM Users WHERE Username = @Username";

                            using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                            {
                                deleteCommand.Parameters.AddWithValue("@Username", Username);
                                int rowsAffected = deleteCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Your account has been successfully deleted. To avoid errors, please log out.");
                                }
                                else
                                {
                                    MessageBox.Show("Failed to delete your account.");
                                }
                            }
                        }
                    }
                    else
                    {
                        // User canceled deletion
                        MessageBox.Show("Account deletion canceled.");
                    }
                }
                else
                {
                    MessageBox.Show("Enter current password to delete account / Current password is incorrect.");
                }
            }
            catch (MySqlException ex)
            {
                // Handle any MySQL exceptions
                MessageBox.Show($"MySQL Exception: {ex.Message}");
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
