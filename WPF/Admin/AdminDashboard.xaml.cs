using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using DataAccess.Models;

namespace WPF.Admin
{
    /// <summary>
    /// Interaction logic for AdminDashboard.xaml
    /// </summary>
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }
        private void btnCategories_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManageCategory());


        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManageUser());
        }


        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnWarehouses_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManageWarehouse());

        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties.Remove("UserAccount");
            Login login = new Login();
            login.Show();
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataAccess.Models.User user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            if (user.Avatar != null) { 
            avaAdmin.Source = new BitmapImage(new Uri(user.Avatar));
            }
            txtAdminName.Text = user.Username;
        }
    }
}
