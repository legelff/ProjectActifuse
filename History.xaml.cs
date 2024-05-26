using MySqlConnector;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media.Effects;
using System.Windows.Media;
using FontAwesome.WPF;
using System;
using System.Data;
using System.Windows.Input;

namespace ProjectActifuse
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : Page
    {
        public string Username { get; set; }

        public History(string username)
        {
            InitializeComponent();
            Username = username;
            LoadFavorites();
            LoadAll();
        }

        private async void LoadFavorites()
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

                            // SQL query to fetch the favorites
                            string getFavoritesQuery = @"
                                                        SELECT historys.*, activitys.Name
                                                        FROM historys
                                                        INNER JOIN activitys ON historys.ActivityId = activitys.ActivityId
                                                        WHERE historys.UserId = @userId
                                                        AND historys.IsFavorite = 1
                                                        ORDER BY historys.GeneratedDateTime DESC;";

                            // Create a MySqlCommand object with the query and connection
                            using (MySqlCommand getFavoritesCommand = new MySqlCommand(getFavoritesQuery, connection))
                            {
                                // Add parameters to the query
                                getFavoritesCommand.Parameters.AddWithValue("@userId", userId);

                                // Execute the query and get a MySqlDataReader
                                using (MySqlDataReader reader = await getFavoritesCommand.ExecuteReaderAsync())
                                {
                                    // Iterate over the result set
                                    while (reader.Read())
                                    {
                                        int activityId = reader.GetInt32("ActivityId");
                                        string description = reader.GetString("Name");
                                        bool isCompleted = reader.GetBoolean("IsCompleted");
                                        int historyId = reader.GetInt32("HistoryId");

                                        // Create a Border dynamically
                                        Border activityBorder = CreateFavoriteBorder(description, activityId, isCompleted, historyId);

                                        // Add the dynamically created Border to the top of the Favorites
                                        FavoritesContainer.Children.Insert(0, activityBorder);
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

        private async void LoadAll()
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

                            // SQL query to fetch all items
                            string getFavoritesQuery = @"
                                                        SELECT historys.*, activitys.Name
                                                        FROM historys
                                                        INNER JOIN activitys ON historys.ActivityId = activitys.ActivityId
                                                        WHERE historys.UserId = @userId
                                                        AND historys.IsFavorite = 0
                                                        ORDER BY historys.GeneratedDateTime DESC;";

                            // Create a MySqlCommand object with the query and connection
                            using (MySqlCommand getFavoritesCommand = new MySqlCommand(getFavoritesQuery, connection))
                            {
                                // Add parameters to the query
                                getFavoritesCommand.Parameters.AddWithValue("@userId", userId);

                                // Execute the query and get a MySqlDataReader
                                using (MySqlDataReader reader = await getFavoritesCommand.ExecuteReaderAsync())
                                {
                                    // Iterate over the result set
                                    while (reader.Read())
                                    {
                                        int activityId = reader.GetInt32("ActivityId");
                                        string description = reader.GetString("Name");
                                        bool isCompleted = reader.GetBoolean("IsCompleted");
                                        int historyId = reader.GetInt32("HistoryId");

                                        // Create a Border dynamically
                                        Border activityBorder = CreateAllBorder(description, activityId, isCompleted, historyId);

                                        // Add the dynamically created Border to the top of the History
                                        HistoryContainer.Children.Insert(0, activityBorder);
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

        private void RefreshAll_Click(object sender, RoutedEventArgs e)
        {
            // Clear all children from HistoryContainer
            HistoryContainer.Children.Clear();

            // Clear all children from FavoritesContainer
            FavoritesContainer.Children.Clear();

            LoadAll();
            LoadFavorites();
        }

        private async void Activity_Click(object sender, RoutedEventArgs e)
        {
            // Get the activityId from the clicked button's Tag property
            if (sender is Button clickedButton && clickedButton.Tag is int activityId)
            {
                try
                {
                    string connectionString = "datasource=127.0.0.1; port=3306; username = root; password=; database=actifuse;";

                    // Connect to MySQL database
                    await using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        string query = "SELECT ActivityId, Type, Name, Participants, Price, Accessibility, Link FROM activitys WHERE ActivityId = @activityId";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@activityId", activityId);
                            await using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    // Extract activity details from the database
                                    string type = reader.GetString("Type");
                                    string description = reader.GetString("Name");
                                    int participants = reader.GetInt32("Participants");
                                    string price = reader.GetDecimal("Price").ToString();
                                    string accessibility = reader.GetDecimal("Accessibility").ToString();
                                    string link = reader.IsDBNull("Link") ? string.Empty : reader.GetString("Link");
                                    string key = reader.GetInt32("ActivityId").ToString();

                                    // Create and display the ActivityPopup
                                    ActivityPopup popup = new ActivityPopup();
                                    popup.SetActivityDetails(type, description, participants, price, accessibility, key, link);
                                    popup.ShowDialog();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error in History: {ex.Message}");
                }
            }
        }

        private async void IsComplete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                // Assuming the Tag property of the toggleButton is of type int
                if (toggleButton.Tag is int historyId)
                {
                    bool isChecked = toggleButton.IsChecked ?? false;
                    bool isCompleted = isChecked; // Toggle the current state

                    try
                    {
                        string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";

                        // Connect to MySQL database
                        await using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            await connection.OpenAsync();

                            string query = "UPDATE historys SET IsCompleted = @isCompleted WHERE HistoryId = @historyId";
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@isCompleted", isCompleted);
                                command.Parameters.AddWithValue("@historyId", historyId);

                                int rowsAffected = await command.ExecuteNonQueryAsync();

                                if (rowsAffected > 0)
                                {
                                    RefreshAll_Click(null, null);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error in IsComplete_Click: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Failed to retrieve historyId.");
                }
            }
        }

        private async void IsFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                // Assuming the Tag property of the toggleButton is of type int
                if (toggleButton.Tag is int historyId)
                {
                    bool isChecked = toggleButton.IsChecked ?? false;
                    bool isCompleted = isChecked; // Toggle the current state

                    try
                    {
                        string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";

                        // Connect to MySQL database
                        await using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            await connection.OpenAsync();

                            string query = "UPDATE historys SET IsFavorite = @isFavorite WHERE HistoryId = @historyId";
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@isFavorite", isCompleted);
                                command.Parameters.AddWithValue("@historyId", historyId);

                                int rowsAffected = await command.ExecuteNonQueryAsync();

                                if (rowsAffected > 0)
                                {
                                    RefreshAll_Click(null, null);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error in IsFavorite_Click: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Failed to retrieve historyId.");
                }
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteButton)
            {
                // Assuming the Tag property of the deleteButton is of type int
                if (deleteButton.Tag is int historyId)
                {
                    try
                    {
                        string connectionString = "datasource=127.0.0.1; port=3306; username=root; password=; database=actifuse;";

                        // Connect to MySQL database
                        await using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            await connection.OpenAsync();

                            string query = "DELETE FROM historys WHERE HistoryId = @historyId";
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@historyId", historyId);

                                int rowsAffected = await command.ExecuteNonQueryAsync();

                                if (rowsAffected > 0)
                                {
                                    RefreshAll_Click(null, null);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error in Delete_Click: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Failed to retrieve historyId.");
                }
            }
        }

        private Border CreateFavoriteBorder(string activityName, int activityId, bool isCompleted, int historyId)
        {
            // Create the border
            Border border = new Border
            {
                CornerRadius = new CornerRadius(5),
                Background = new SolidColorBrush(Color.FromRgb(109, 93, 110)),
                Width = 190,
                Height = 93,
                Margin = new Thickness(0, 12, 0, 0),
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 5,
                    ShadowDepth = 0,
                    Opacity = 0.3
                }
            };

            // Create stack panel for content
            StackPanel stackPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Create text block for activity name
            TextBlock textBlock = new TextBlock
            {
                Text = activityName,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(5),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.Black,
                Margin = new Thickness(0, 0, 0, 0),
            };

            // Create the main button
            Button mainButton = new Button
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0), // Remove border
                Content = textBlock, // Set text block as content
                Margin = new Thickness(5, 0, 5, 5),
                Tag = activityId
            };
            mainButton.Click += Activity_Click;

            // Create inner stack panel for buttons
            StackPanel innerStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Create toggle button for completion
            ToggleButton completionButton = new ToggleButton
            {
                Height = 20,
                Width = 20,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 0, 5, 0),
                Tag = historyId
            };

            completionButton.Click += IsComplete_Click;
            completionButton.Content = new ImageAwesome { Icon = FontAwesomeIcon.Check, Foreground = Brushes.Black, Height = 10, Width = 10 };

            // Set IsChecked based on the condition
            if (isCompleted)
            {
                completionButton.IsChecked = true;
            }

            // Create toggle button for favorite
            ToggleButton favoriteButton = new ToggleButton
            {
                Height = 20,
                Width = 20,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 0, 5, 0),
                IsChecked = true, // Set to true as default for favorited
                Tag = historyId
            };
            favoriteButton.Click += IsFavorite_Click;
            favoriteButton.Content = new ImageAwesome { Icon = FontAwesomeIcon.Star, Foreground = Brushes.Black, Height = 10, Width = 10 };

            // Create delete button
            Button deleteButton = new Button
            {
                Height = 20,
                Width = 20,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 0, 0, 0),
                Tag = historyId
            };
            deleteButton.Click += Delete_Click;
            deleteButton.Content = new ImageAwesome { Icon = FontAwesomeIcon.Remove, Foreground = Brushes.Black, Height = 10, Width = 10 };

            // Add buttons to inner stack panel
            innerStackPanel.Children.Add(completionButton);
            innerStackPanel.Children.Add(favoriteButton);
            innerStackPanel.Children.Add(deleteButton);

            // Add inner stack panel to main stack panel
            stackPanel.Children.Add(mainButton);
            stackPanel.Children.Add(innerStackPanel);

            // Set stack panel as content of border
            border.Child = stackPanel;

            return border;
        }

        private Border CreateAllBorder(string activityName, int activityId, bool isCompleted, int historyId)
        {
            // Create the border
            Border border = new Border
            {
                CornerRadius = new CornerRadius(5),
                Background = new SolidColorBrush(Color.FromRgb(109, 93, 110)),
                Width = 150,
                Height = 93,
                Margin = new Thickness(8), // Adjusted margin
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 5,
                    ShadowDepth = 0,
                    Opacity = 0.3
                }
            };

            // Create stack panel for content
            StackPanel stackPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Create text block for activity name
            TextBlock textBlock = new TextBlock
            {
                Text = activityName,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(5),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.Black,
                Margin = new Thickness(0, 0, 0, 0),
            };

            // Create the main button
            Button mainButton = new Button
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0), // Remove border
                Content = textBlock, // Set text block as content
                Margin = new Thickness(5, 0, 5, 5),
                Tag = activityId
            };
            mainButton.Click += Activity_Click;

            // Create inner stack panel for buttons
            StackPanel innerStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Create toggle button for completion
            ToggleButton completionButton = new ToggleButton
            {
                Height = 20,
                Width = 20,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 0, 5, 0),
                Tag = historyId
            };
            completionButton.Click += IsComplete_Click;
            completionButton.Content = new ImageAwesome { Icon = FontAwesomeIcon.Check, Foreground = Brushes.Black, Height = 10, Width = 10 };

            // Set IsChecked based on the condition
            if (isCompleted)
            {
                completionButton.IsChecked = true;
            }

            // Create toggle button for favorite
            ToggleButton favoriteButton = new ToggleButton
            {
                Height = 20,
                Width = 20,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 0, 5, 0),
                Tag = historyId
            };
            favoriteButton.Click += IsFavorite_Click;
            favoriteButton.Content = new ImageAwesome { Icon = FontAwesomeIcon.Star, Foreground = Brushes.Black, Height = 10, Width = 10 };

            // Create delete button
            Button deleteButton = new Button
            {
                Height = 20,
                Width = 20,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 0, 0, 0),
                Tag = historyId
            };
            deleteButton.Click += Delete_Click;
            deleteButton.Content = new ImageAwesome { Icon = FontAwesomeIcon.Remove, Foreground = Brushes.Black, Height = 10, Width = 10 };

            // Add buttons to inner stack panel
            innerStackPanel.Children.Add(completionButton);
            innerStackPanel.Children.Add(favoriteButton);
            innerStackPanel.Children.Add(deleteButton);

            // Add main button and inner stack panel to content stack panel
            stackPanel.Children.Add(mainButton);
            stackPanel.Children.Add(innerStackPanel);

            // Set stack panel as content of border
            border.Child = stackPanel;

            return border;
        }
    }
}
