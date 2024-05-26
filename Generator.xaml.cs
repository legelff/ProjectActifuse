using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectActifuse
{
    /// <summary>
    /// Interaction logic for Generator.xaml
    /// </summary>
    public partial class Generator : Page
    {
        public string Username { get; private set; }

        public Generator(string username)
        {
            InitializeComponent();
            Loaded += Generator_Loaded;
            Username = username;
            LoadHistoryPreview();
        }

        // Loaded event handler to set default values
        private void Generator_Loaded(object sender, RoutedEventArgs e)
        {
            // Set default values for MaxPriceSlider and MaxPriceTextBox
            MaxPriceSlider.Value = 1.00;
            MaxPriceTextBox.Text = "1.00";

            // Set default values for MaxAccessibilitySlider and MaxAccessibilityTextBox
            MaxAccessibilitySlider.Value = 1.00;
            MaxAccessibilityTextBox.Text = "1.00";
        }

        // Method to load history items when the page loads
        private async void LoadHistoryPreview()
        {
            try
            {
                string connectionString = "datasource=127.0.0.1; port=3306; username = root; password=; database=actifuse;";

                // First, query the users table to get the UserId based on the Username
                string getUserIdQuery = "SELECT UserId FROM users WHERE Username = @username";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Create a MySqlCommand object with the query and connection
                    using (MySqlCommand getUserIdCommand = new MySqlCommand(getUserIdQuery, connection))
                    {
                        // Add parameters to the query
                        getUserIdCommand.Parameters.AddWithValue("@username", Username);

                        // Execute the query and get the UserId
                        object result = await getUserIdCommand.ExecuteScalarAsync();

                        if (result != null)
                        {
                            int userId = Convert.ToInt32(result);

                            // SQL query to fetch the latest 10 history items for the current user
                            string getHistoryQuery = @"
                                                    SELECT historys.*, activitys.Type, activitys.Name
                                                    FROM historys
                                                    INNER JOIN activitys ON historys.ActivityId = activitys.ActivityId
                                                    WHERE historys.UserId = @userId
                                                    ORDER BY historys.GeneratedDateTime DESC
                                                    LIMIT 10";

                            // Create a MySqlCommand object with the query and connection
                            using (MySqlCommand getHistoryCommand = new MySqlCommand(getHistoryQuery, connection))
                            {
                                // Add parameters to the query
                                getHistoryCommand.Parameters.AddWithValue("@userId", userId);

                                // Execute the query and get a MySqlDataReader
                                using (MySqlDataReader reader = await getHistoryCommand.ExecuteReaderAsync())
                                {
                                    // Iterate over the result set
                                    while (reader.Read())
                                    {
                                        int activityId = reader.GetInt32("ActivityId");
                                        string type = reader.GetString("Type");
                                        type = char.ToUpper(type[0]) + type.Substring(1);
                                        string description = reader.GetString("Name");

                                        // Create a Border dynamically
                                        Border activityBorder = CreateActivityBorder(type, description);

                                        // Add the dynamically created Border to the top of the HistoryPreviewContainer
                                        HistoryPreviewContainer.Children.Insert(0, activityBorder);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("User not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            // Reset Type filter to null
            TypeDropdownBox.SelectedItem = null;

            // Reset Participants filter to null
            NumberSelector.Value = null;

            // Reset Max Price filter to 1.00
            MaxPriceSlider.Value = 1.00;

            // Reset Max Accessibility filter to 1.00
            MaxAccessibilitySlider.Value = 1.00;
        }

        private async void RandomGen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Make API request to boredapi
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://www.boredapi.com/api/activity";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Parse the response JSON
                    dynamic activityData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);

                    HandleActivityData(activityData);

                    // Extract required details
                    string type = activityData.type;
                    string description = activityData.activity;
                    int participants = activityData.participants;
                    string price = activityData.price;
                    string accessibility = activityData.accessibility;
                    string key = activityData.key;
                    string link = activityData.link;

                    // Instantiate the custom popup window
                    ActivityPopup popup = new ActivityPopup();
                    // Set activity details in the popup
                    popup.SetActivityDetails(type, description, participants, price, accessibility, key, link);
                    // Show the popup
                    popup.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void FilterGen_Click(object sender, RoutedEventArgs e)
        {
            // Initialize the base URL
            string baseUrl = "https://www.boredapi.com/api/activity";

            // Initialize an empty list to store query parameters
            List<string> queryParams = new List<string>();

            // Add Type parameter if selected
            if (TypeDropdownBox.SelectedItem != null)
            {
                string type = ((ComboBoxItem)TypeDropdownBox.SelectedItem).Content.ToString().ToLower();
                queryParams.Add($"type={type}");
            }

            // Add Participants parameter if a value is selected
            if (NumberSelector.Value != null)
            {
                string participants = NumberSelector.Value.ToString();
                queryParams.Add($"participants={participants}");
            }

            // Add Max Price parameter
            string maxPrice = MaxPriceSlider.Value.ToString("F2", CultureInfo.InvariantCulture);
            queryParams.Add($"maxprice={maxPrice}");

            // Add Max Accessibility parameter
            string maxAccessibility = MaxAccessibilitySlider.Value.ToString("F2", CultureInfo.InvariantCulture);
            queryParams.Add($"maxaccessibility={maxAccessibility}");

            // Combine the parameters into a query string
            string queryString = string.Join("&", queryParams);

            // Combine the base URL with the query string
            string activityQuery = string.IsNullOrEmpty(queryString) ? baseUrl : $"{baseUrl}?{queryString}";

            //MessageBox.Show(activityQuery);

            try
            {
                // Make API request to boredapi
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = activityQuery;
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Parse the response JSON
                    dynamic activityData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);

                    // Check if the response contains an error
                    if (activityData.error != null)
                    {
                        MessageBox.Show($"No activity found, try fewer or different filters.");
                        return; // Exit the method if there's an error
                    }

                    HandleActivityData(activityData);

                    // Extract required details
                    string type = activityData.type;
                    string description = activityData.activity;
                    int participants = activityData.participants;
                    string price = activityData.price;
                    string accessibility = activityData.accessibility;
                    string key = activityData.key;
                    string link = activityData.link;

                    // Instantiate the custom popup window
                    ActivityPopup popup = new ActivityPopup();
                    // Set activity details in the popup
                    popup.SetActivityDetails(type, description, participants, price, accessibility, key, link);
                    // Show the popup
                    popup.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Define a method to insert activity data into the database
        private async Task InsertActivity(int activityId, string type, string description, int participants, decimal price, decimal accessibility, string link)
        {
            // Connection string to connect to your MySQL database
            string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";

            // Create a MySqlConnection object
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // Open the database connection
                await connection.OpenAsync();

                // Define the query to check if the activity already exists
                string checkQuery = "SELECT COUNT(*) FROM activitys WHERE ActivityId = @activityId";

                // Create a MySqlCommand object to check for existence
                using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@activityId", activityId);
                    int count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                    if (count > 0)
                    {
                        // Activity already exists, no need to insert
                        return;
                    }
                }

                // Define the INSERT query
                string insertQuery = "INSERT INTO activitys (ActivityId, Name, Type, Price, Accessibility, Participants, Link) " +
                                     "VALUES (@activityId, @description, @type, @price, @accessibility, @participants, @link)";

                // Create a MySqlCommand object with the query and connection
                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    // Add parameters to the query
                    insertCommand.Parameters.AddWithValue("@activityId", activityId);
                    insertCommand.Parameters.AddWithValue("@description", description);
                    insertCommand.Parameters.AddWithValue("@type", type);
                    insertCommand.Parameters.AddWithValue("@price", price);
                    insertCommand.Parameters.AddWithValue("@accessibility", accessibility);
                    insertCommand.Parameters.AddWithValue("@participants", participants);
                    insertCommand.Parameters.AddWithValue("@link", link);

                    // Execute the INSERT query
                    await insertCommand.ExecuteNonQueryAsync();
                }
            }
        }

        // Define a method to insert history data into the database
        private async Task InsertHistory(int userId, int activityId)
        {
            // Connection string to connect to your MySQL database
            string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";

            // Create a MySqlConnection object
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // Open the database connection
                await connection.OpenAsync();

                // Define the INSERT query for the history record
                string insertQuery = "INSERT INTO historys (UserId, ActivityId, IsFavorite, IsCompleted) " +
                                     "VALUES (@userId, @activityId, @isFavorite, @isCompleted)";

                // Create a MySqlCommand object with the query and connection
                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    // Add parameters to the query
                    insertCommand.Parameters.AddWithValue("@userId", userId);
                    insertCommand.Parameters.AddWithValue("@activityId", activityId);
                    insertCommand.Parameters.AddWithValue("@isFavorite", false);
                    insertCommand.Parameters.AddWithValue("@isCompleted", false);

                    // Execute the INSERT query
                    await insertCommand.ExecuteNonQueryAsync();
                }
            }
        }

        // Call this method after retrieving activity data
        private async void HandleActivityData(dynamic activityData)
        {
            try
            {
                // Extract required details from activityData
                int activityId = int.Parse((string)activityData.key);
                string type = activityData.type;
                string description = activityData.activity;
                int participants = activityData.participants;
                decimal price = decimal.Parse((string)activityData.price, CultureInfo.InvariantCulture);
                decimal accessibility = decimal.Parse((string)activityData.accessibility, CultureInfo.InvariantCulture);
                string link = activityData.link;

                // Insert the activity data into the database
                await InsertActivity(activityId, type, description, participants, price, accessibility, link);

                // Create Border dynamically
                Border activityBorder = CreateActivityBorder(type, description);

                // Check if the number of child elements exceeds 10
                if (HistoryPreviewContainer.Children.Count >= 10)
                {
                    // Remove the last child (most bottom one)
                    HistoryPreviewContainer.Children.RemoveAt(HistoryPreviewContainer.Children.Count - 1);
                }

                // Insert the dynamically created Border at the top of the StackPanel
                HistoryPreviewContainer.Children.Insert(0, activityBorder);

                // Connection string to connect to your MySQL database
                string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";

                // Define the query to retrieve UserId from users table based on Username
                string getUserQuery = "SELECT UserId FROM users WHERE Username = @username";

                // Create MySqlConnection object
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Open database connection
                    await connection.OpenAsync();

                    // Create MySqlCommand object with the query and connection
                    using (MySqlCommand getUserCommand = new MySqlCommand(getUserQuery, connection))
                    {
                        // Add parameters to the query
                        getUserCommand.Parameters.AddWithValue("@username", Username);

                        // Execute the query and retrieve the UserId
                        object result = await getUserCommand.ExecuteScalarAsync();

                        // Check if the result is not null and convert it to an integer
                        if (result != null && result != DBNull.Value)
                        {
                            int userId = Convert.ToInt32(result);

                            // Check if the activity already exists in the user's history
                            bool activityExists = await CheckActivityExistsInHistory(userId, activityId);

                            // If the activity doesn't exist in the history, insert it
                            if (!activityExists)
                            {
                                await InsertHistory(userId, activityId);
                            }
                        }
                        else
                        {
                            MessageBox.Show("User not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Method to check if the activity already exists in the user's history
        private async Task<bool> CheckActivityExistsInHistory(int userId, int activityId)
        {
            // Connection string to connect to your MySQL database
            string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";

            try
            {
                // Define the query to check if the activity exists in the user's history
                string checkQuery = "SELECT COUNT(*) FROM historys WHERE UserId = @userId AND ActivityId = @activityId";

                // Create MySqlConnection object
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Open database connection
                    await connection.OpenAsync();

                    // Create MySqlCommand object with the query and connection
                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        // Add parameters to the query
                        checkCommand.Parameters.AddWithValue("@userId", userId);
                        checkCommand.Parameters.AddWithValue("@activityId", activityId);

                        // Execute the query and check if the activity exists in the user's history
                        int count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }

        private Border CreateActivityBorder(string type, string description)
        {
            // Create a Border element
            Border border = new Border();
            border.CornerRadius = new CornerRadius(5);
            border.Background = new SolidColorBrush(Color.FromRgb(109, 93, 110));
            border.Width = 190;
            border.Height = 93;
            border.Margin = new Thickness(0, 12, 0, 0);
            border.Padding = new Thickness(5);
            border.Effect = new DropShadowEffect() { Color = Colors.Black, BlurRadius = 5, ShadowDepth = 0, Opacity = 0.3 };

            // Create a StackPanel inside the Border
            StackPanel stackPanel = new StackPanel();
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.VerticalAlignment = VerticalAlignment.Center;

            // Create a TextBlock for the Type
            TextBlock typeTextBlock = new TextBlock();
            typeTextBlock.Text = type;
            typeTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            typeTextBlock.VerticalAlignment = VerticalAlignment.Center;
            typeTextBlock.Padding = new Thickness(0);
            typeTextBlock.TextAlignment = TextAlignment.Center;
            typeTextBlock.TextWrapping = TextWrapping.Wrap;
            typeTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(244, 238, 224));
            typeTextBlock.FontSize = 20;
            typeTextBlock.FontWeight = FontWeights.Bold;
            typeTextBlock.Margin = new Thickness(0, 0, 0, 10);

            // Create a TextBlock for the Description
            TextBlock descriptionTextBlock = new TextBlock();
            descriptionTextBlock.Text = description;
            descriptionTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            descriptionTextBlock.VerticalAlignment = VerticalAlignment.Center;
            descriptionTextBlock.Padding = new Thickness(0);
            descriptionTextBlock.TextAlignment = TextAlignment.Center;
            descriptionTextBlock.TextWrapping = TextWrapping.Wrap;
            descriptionTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(244, 238, 224));
            descriptionTextBlock.Margin = new Thickness(0, 0, 0, 0);

            // Add TextBlocks to the StackPanel
            stackPanel.Children.Add(typeTextBlock);
            stackPanel.Children.Add(descriptionTextBlock);

            // Set the StackPanel as the Child of the Border
            border.Child = stackPanel;

            return border;
        }
    }
}
