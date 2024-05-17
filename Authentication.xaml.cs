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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectActifuse
{
    /// <summary>
    /// Interaction logic for Authentication.xaml
    /// </summary>
    public partial class Authentication : Window
    {
        public Authentication()
        {
            InitializeComponent();
            LoginPage loginPage = new LoginPage(this); // Pass reference to current Authentication window
            AuthenticationFrame.Content = loginPage;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationFrame.Content = new Register();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginPage loginPage = new LoginPage(this); // Pass reference to current Authentication window
            AuthenticationFrame.Content = loginPage;
        }

        public void CloseWindow()
        {
            this.Close();
        }
    }
}
