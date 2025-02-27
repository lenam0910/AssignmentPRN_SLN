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
using DataAccess.Models;
using Repository;
using Service;

namespace WPF.Supplier
{
    /// <summary>
    /// Interaction logic for SupplierDashboard.xaml
    /// </summary>
    public partial class SupplierDashboard : Window
    {
        private SupplierService supplierService; 
        public SupplierDashboard()
        {
            supplierService = new SupplierService();
            InitializeComponent();
        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            User user = Application.Current.Properties["UserAccount"] as User;
            var supplier = supplierService.GetSupplierById(user.SupplierId.Value);
            if (user is User && !string.IsNullOrEmpty(user.Avatar))
            {
                avaSupplier.Source = new BitmapImage(new Uri(user.Avatar));
            }
           
            
                txtBlockHead.Text = " " + user.Username;
                txtNameSupplier.Text = "Dashboard " + supplier.SupplierName;

        }

       

        private void Nav_Account(object sender, RoutedEventArgs e)
        {

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
    }
}
