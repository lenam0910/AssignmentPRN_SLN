
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Media.Imaging;

using Service;

namespace WPF.Supplier
{
    /// <summary>
    /// Interaction logic for SupplierDashboard.xaml
    /// </summary>
    public partial class SupplierDashboard : Window
    {
        private SupplierService supplierService;
        private UserSupplierService userSupplierService;
        public SupplierDashboard()
        {
            userSupplierService = new();
            supplierService = new SupplierService();
            InitializeComponent();
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataAccess.Models.User user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            var supplier = userSupplierService.GetSupplierByUserId(user.UserId);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    string imagePath = user.Avatar;

                    if (File.Exists(imagePath))
                    {
                        avaSupplier.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                    }

                }
            }

                txtBlockHead.Text = " " + user.Username;
            MainFrame.Navigate(new StatisticsPage());


        }



        private void Nav_Account(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UpdateProfilePage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties.Remove("UserAccount");
            Login login = new Login();
            login.Show();
            this.Hide();
        }

        private void Nav_Orders(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new InventoryManagement());
        }

        private void Nav_Warehouse(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new WarehouseManagement());
        }


        private void Nav_Products(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductManagement());

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TransactionLogPage());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductIncomePage());
        }
    }
}
