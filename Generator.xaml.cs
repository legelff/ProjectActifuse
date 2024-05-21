using System;
using System.Collections.Generic;
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

        private void FilterGen_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
