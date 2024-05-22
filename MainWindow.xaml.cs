using MySqlConnector;
using System.IO;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString = "datasource=127.0.0.1; port=3306; username = root; password=; database=actifuse;";

        public string Username { get; private set; }
        public MainWindow(string username)
        {
            InitializeComponent();
            Username = username;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Name, ProfilePicturePath FROM Users WHERE Username=@Username;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", username);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string name = reader.GetString("Name");
                            NameTextBlock.Text = name;

                            // Check if profile picture path is not null
                            if (!reader.IsDBNull(reader.GetOrdinal("ProfilePicturePath")))
                            {
                                string profilePicturePath = reader.GetString("ProfilePicturePath");
                                string fullProfilePicturePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), profilePicturePath);

                                // Set the Image source to the retrieved profile picture path
                                ImageContainer.Source = new BitmapImage(new Uri(fullProfilePicturePath, UriKind.RelativeOrAbsolute));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching user data: " + ex.Message);
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

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            Authentication autenticationWindow = new Authentication();
            autenticationWindow.Show();

            this.Close();
        }

        private void HomeNav_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Generator();
        }

        private void HistoryNav_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new History();
        }

        private void SupportNav_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Support(Username);
        }

        private void ProfileNav_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Profile(Username);
        }
    }
}