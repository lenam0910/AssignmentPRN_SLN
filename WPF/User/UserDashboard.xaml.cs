using System;
using System.Collections.Generic;
using System.IO;
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
using Service;

namespace WPF.User
{
    /// <summary>
    /// Interaction logic for UserDashboard.xaml
    /// </summary>
    public partial class UserDashboard : Window
    {
        private DataAccess.Models.User user;

        public UserDashboard()
        {
            InitializeComponent();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties.Remove("UserAccount");
            Login login = new Login();
            login.Show();
            this.Hide();
        }
        public void RefreshUserProfile()
        {
            var user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    string imagePath = user.Avatar;
                    if (File.Exists(imagePath))
                    {
                        avarImg.Source = null;
                        avarImg.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            DataAccess.Models.User user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;

            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    string imagePath = user.Avatar;

                    if (File.Exists(imagePath))
                    {
                        avarImg.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                    }

                }
                this.DataContext = user;
                MainFrame.Navigate(new ShoppingPage());

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ShoppingPage());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrdersPage());

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TransactionsPage());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EditProfilePage());
        }
    }
}
