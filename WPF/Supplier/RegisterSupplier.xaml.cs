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
using DataAccess.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Service;

namespace WPF.Supplier
{
    /// <summary>
    /// Interaction logic for RegisterSupplier.xaml
    /// </summary>
    public partial class RegisterSupplier : Window
    {
        private string saveDirectory = @"C:\Users\ADMIN\Desktop\PRN212\AssignmentPRN\AssignmentPRN_SLN\DataAccess\Images\Supplier\";
        private string selectedFilePath;
        private string fileName;
        private string destinationPath = null;
        private SupplierService supplierService;
        private UserService userService;
        private UserSupplierService userSupplierService;
        public RegisterSupplier()
        {
            userSupplierService = new UserSupplierService();
            InitializeComponent();
            supplierService = new();
            userService = new();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            DataAccess.Models.User user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;

            DataAccess.Models.Supplier supplier = new()
            {
                SupplierName = txtSupplierName.Text,
                ContactInfo = txtContactInfo.Text,
                Email = txtEmail.Text,
                Phone = txtPhone.Text,
                Avatar = destinationPath
            };

            if (supplierService.SaveSupplier(supplier))
            {
                UserSupplier userSupplier = new UserSupplier();
                userSupplier.SupplierId = supplier.SupplierId;
                userSupplier.UserId = user.UserId;
                userSupplierService.Add(userSupplier);
                saveAvatarSupplier();

                SupplierDashboard supplierDashboard = new SupplierDashboard();
                MessageBox.Show("Đăng ký thông tin nhà cung cấp thành công!");
                supplierDashboard.Show();
                this.Hide();
            }

            else
            {
                MessageBox.Show("Đăng ký thông tin nhà cung cấp thất bại!");
            }


        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            MessageBox.Show("Nếu bạn không đăng ký, bạn không thể nhập xuất sản phẩm !");
            login.Show();
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void UploadAvatar_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                fileName = System.IO.Path.GetFileName(selectedFilePath);
                destinationPath = System.IO.Path.Combine(saveDirectory, fileName);

                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(saveDirectory);



                // Hiển thị ảnh lên UI
                avatarImage.Source = new BitmapImage(new Uri(selectedFilePath));
                avatarImage.Visibility = Visibility.Visible;

            }
        }

        private void saveAvatarSupplier()
        {
            File.Copy(selectedFilePath, destinationPath, true);
        }
    }
}
