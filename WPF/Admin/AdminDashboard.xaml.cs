using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Service;
using WPF.User;

namespace WPF.Admin
{
    /// <summary>
    /// Interaction logic for AdminDashboard.xaml
    /// </summary>
    public partial class AdminDashboard : Window
    {
        private APIkeyService apiKeyService;
        private DataAccess.Models.User user;
        public AdminDashboard()
        {
            apiKeyService = new APIkeyService();
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
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
            MainFrame.Navigate (new ManageProducts());
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
            MainFrame.Navigate(new MainPage());

        }

        private void btnSuppliers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManageSuppliers());

        }

        private void GearButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton btnSettings = sender as ToggleButton;
            if (btnSettings != null)
            {
                SettingsMenu.IsOpen = !SettingsMenu.IsOpen;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsOverlay.Visibility = Visibility.Visible;
        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            SettingsOverlay.Visibility = Visibility.Collapsed;
        }

        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           

            ApiKey api = new ApiKey() { ApiKey1 = ApiKeyPasswordBox.Text, UserId = user.UserId };

            

            if (apiKeyService.Add(api))
            {
                var lstApi = apiKeyService.GetAll();
                {
                    foreach (var item in lstApi)
                    {
                        if (item.ApiKeyId != api.ApiKeyId)
                        {
                            apiKeyService.Delete(item);
                        }
                    }
                }
                MessageBox.Show("Thêm API KEY mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Không thể thêm API KEY vào dịch vụ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
    }
}
