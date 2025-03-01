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
        private string saveDirectoryUser = @"C:\Users\ADMIN\Desktop\PRN212\AssignmentPRN\AssignmentPRN_SLN\DataAccess\Images\Avar\";
        private string saveDirectorySupplier = @"C:\Users\ADMIN\Desktop\PRN212\AssignmentPRN\AssignmentPRN_SLN\DataAccess\Images\Supplier\";

        private readonly UserService service;
        private string selectedFilePath;
        private string fileName;
        private string destinationPathUser = null;
        private string destinationPathSupplier = null;

        private UserService _userService;
        private SupplierService _supplierService;
        private User user;
        private DataAccess.Models.Supplier supplier;
        private UserSupplierService userSupplierService;
        public UpdateProfilePage()
        {
            userSupplierService = new();
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


            DataAccess.Models.Supplier sup = userSupplierService.GetSupplierByUserId(user.UserId);
            if (user.Avatar != null)
            {
                imgSupplierAvatar.Source = new BitmapImage(new Uri(sup.Avatar));
            }
            else
            {
                imgSupplierAvatar = null;
            }
        }

        private void ChangeSupplierAvatar_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                fileName = System.IO.Path.GetFileName(selectedFilePath);
                destinationPathSupplier = System.IO.Path.Combine(saveDirectorySupplier, fileName);

                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(saveDirectorySupplier);



                // Hiển thị ảnh lên UI
                imgSupplierAvatar.Source = new BitmapImage(new Uri(selectedFilePath));
                imgSupplierAvatar.Visibility = Visibility.Visible;

            }
        }

        private void ChangeUserAvatar_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                fileName = System.IO.Path.GetFileName(selectedFilePath);
                destinationPathUser = System.IO.Path.Combine(saveDirectoryUser, fileName);

                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(saveDirectoryUser);



                // Hiển thị ảnh lên UI
                imgUserAvatar.Source = new BitmapImage(new Uri(selectedFilePath));
                imgUserAvatar.Visibility = Visibility.Visible;

            }
        }

        private void SaveSupplier_Click(object sender, RoutedEventArgs e)
        {
            supplier.SupplierName = txtSupplierName.Text;
            supplier.Email = txtSupplierEmail.Text;
            supplier.Phone = txtSupplierPhone.Text;
            if (imgSupplierAvatar.Source != null)
            {
                DeleteOldSupplierAvatar();
                supplier.Avatar = destinationPathSupplier;
            }
            if (_supplierService.UpdateSupplier(supplier))
            {
                saveAvatarSupplier();
                MessageBox.Show("supplier Done");
            }
            
        }

        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            user.FullName = txtFullName.Text;
            user.Email = txtUserEmail.Text;
            user.Password = txtPassword.Password;
            user.Phone = txtUserPhone.Text;
            user.Address = txtUserAddress.Text;
            if (imgUserAvatar.Source != null)
            {
                DeleteOldUserAvatar();
                user.Avatar = destinationPathUser;
            }
            if (_userService.UpdateUser(user))
            {
                saveAvatar();
                MessageBox.Show("user Done");

            }
        }
      

        private void saveAvatar()
        {
            File.Copy(selectedFilePath, destinationPathUser, true);
        }
        private void saveAvatarSupplier()
        {
            File.Copy(selectedFilePath, destinationPathSupplier, true);
        }
        private void DeleteOldUserAvatar()
        {
            if (!string.IsNullOrEmpty(user.Avatar) && File.Exists(user.Avatar))
            {
                File.Delete(user.Avatar);
            }
        }

        private void DeleteOldSupplierAvatar()
        {
            if (!string.IsNullOrEmpty(supplier.Avatar) && File.Exists(supplier.Avatar))
            {
                File.Delete(supplier.Avatar);
            }
        }

    }
}
