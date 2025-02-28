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
using DataAccess.Models;
using Service;

namespace WPF.Supplier
{
    /// <summary>
    /// Interaction logic for UpdateProfilePage.xaml
    /// </summary>
    public partial class UpdateProfilePage : Page
    {
        private UserService _userService;
        private SupplierService _supplierService;
        private User user;
        private DataAccess.Models.Supplier supplier;
        public UpdateProfilePage()
        {
            user = Application.Current.Properties["UserAccount"] as User;
            _userService = new UserService();
            _supplierService = new SupplierService();
            InitializeComponent();
            Load();
        }

        private void Load()
        {

            supplier = _supplierService.GetSuppliersByUserId(user.UserId);
            txtFullName.Text = user.Username;
            txtUserEmail.Text = user.Email;
            txtPassword.Password = user.Password;
            txtUserPhone.Text = user.Phone;
            txtUserAddress.Text = user.Address;
            if (user.Avatar != null) {
                imgUserAvatar.Source = new BitmapImage(new Uri(user.Avatar));
            }
            else
            {
                imgUserAvatar = null;
            }

            txtSupplierName.Text = supplier.SupplierName;
            txtSupplierEmail.Text = supplier.Email;
            txtSupplierPhone.Text = supplier.Phone;
        }

        private void ChangeSupplierAvatar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeUserAvatar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveSupplier_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
