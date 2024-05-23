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
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : Page
    {
        public string Username { get; set; }

        public History(string username)
        {
            InitializeComponent();
            Username = username;
        }

        private void RefreshAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Activity_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
