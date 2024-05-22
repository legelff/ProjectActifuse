﻿using MySqlConnector;
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
        public Generator()
        {
            InitializeComponent();
            Loaded += Generator_Loaded;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
