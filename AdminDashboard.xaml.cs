using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace ProjectActifuse
{
    public partial class AdminDashboard : Window
    {
        private User _currentUser;

        public AdminDashboard()
        {
            InitializeComponent();
            LoadUsers();
        }

        private async void LoadUsers()
        {
            List<User> users = await UserManager.GetAllUsers();
            UsersDataGrid.ItemsSource = users;
        }

        private void ClearForm()
        {
            UsernameTextBox.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            EmailTextBox.Text = string.Empty;
            IsAdminCheckBox.IsChecked = false;
            _currentUser = null;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = EncryptPassword(PasswordBox.Password);
            var email = EmailTextBox.Text;
            var isAdmin = IsAdminCheckBox.IsChecked ?? false;

            if (_currentUser == null)
            {
                var newUser = new User { Username = username, Password = password, Email = email, IsAdmin = isAdmin };
                bool success = await UserManager.AddUser(newUser);
                if (!success)
                {
                    ErrorMessage.Text = "Error saving user.";
                    ErrorMessage.Visibility = Visibility.Visible;
                    return;
                }
            }
            else
            {
                _currentUser.Username = username;
                _currentUser.Password = password;
                _currentUser.Email = email;
                _currentUser.IsAdmin = isAdmin;
                await UserManager.UpdateUser(_currentUser);
            }

            ClearForm();
            LoadUsers();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var user = (User)((FrameworkElement)sender).DataContext;
            _currentUser = user;

            UsernameTextBox.Text = user.Username;
            PasswordBox.Password = user.Password;
            EmailTextBox.Text = user.Email;
            IsAdminCheckBox.IsChecked = user.IsAdmin;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var user = (User)((FrameworkElement)sender).DataContext;
            var result = MessageBox.Show($"Are you sure you want to delete {user.Username}?", "Confirm Delete", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                await UserManager.DeleteUser(user.UserId);
                LoadUsers();
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
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }
                return stringBuilder.ToString();
            }
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
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
