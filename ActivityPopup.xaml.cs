using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ProjectActifuse
{
    /// <summary>
    /// Interaction logic for ActivityPopup.xaml
    /// </summary>
    public partial class ActivityPopup : Window
    {
        public ActivityPopup()
        {
            InitializeComponent();
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

        public void SetActivityDetails(string type, string description, int participants, string price, string accessibility, string key, string link)
        {
            type = char.ToUpper(type[0]) + type.Substring(1).ToLower();

            txtType.Text = $"{type}";
            txtDescription.Text = $"Description: {description}";
            txtParticipants.Text = $"Participants: {participants}";
            txtPrice.Text = $"Price: {price}";
            txtAccessibility.Text = $"Accessibility: {accessibility}";

            // Conditionally display the link
            if (!string.IsNullOrEmpty(link))
            {
                txtLink.Text = $"Link: {link}";
                txtLink.Visibility = Visibility.Visible;
            }
            else
            {
                txtLink.Visibility = Visibility.Collapsed;
            }
        }

        // Event handler to copy link to clipboard
        private void TxtLink_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLink.Text))
            {
                // Extract the actual link part from the TextBlock text
                string link = txtLink.Text.Replace("Link: ", string.Empty);
                Clipboard.SetText(link);
                MessageBox.Show("Link copied to clipboard!");
            }
        }
    }
}
