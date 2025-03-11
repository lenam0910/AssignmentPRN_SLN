using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            try
            {
                // Kiểm tra xem tài khoản người dùng có tồn tại hay không
                if (!Application.Current.Properties.Contains("UserAccount") ||
                    Application.Current.Properties["UserAccount"] is not DataAccess.Models.User user)
                {
                    MessageBox.Show("Không tìm thấy tài khoản người dùng. Vui lòng đăng nhập lại.");
                    return;
                }

                // Kiểm tra input đầu vào
                if (string.IsNullOrWhiteSpace(txtSupplierName.Text) ||
                    string.IsNullOrWhiteSpace(txtContactInfo.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin nhà cung cấp.");
                    return;
                }

                // Kiểm tra email hợp lệ
                if (!IsValidEmail(txtEmail.Text))
                {
                    MessageBox.Show("Email không hợp lệ!");
                    return;
                }

                // Kiểm tra số điện thoại hợp lệ
                if (!IsValidPhoneNumber(txtPhone.Text))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ!");
                    return;
                }

                // Tạo đối tượng Supplier
                DataAccess.Models.Supplier supplier = new()
                {
                    SupplierName = txtSupplierName.Text.Trim(),
                    ContactInfo = txtContactInfo.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Avatar = !string.IsNullOrEmpty(destinationPath) ? destinationPath : null // Chỉ lưu Avatar nếu có
                };

                // Lưu thông tin nhà cung cấp
                if (supplierService.SaveSupplier(supplier))
                {
                    // Liên kết User với Supplier
                    UserSupplier userSupplier = new()
                    {
                        SupplierId = supplier.SupplierId,
                        UserId = user.UserId
                    };
                    userSupplierService.Add(userSupplier);

                    // Lưu Avatar nếu có ảnh
                    if (!string.IsNullOrEmpty(destinationPath))
                    {
                        saveAvatarSupplier();
                    }

                    // Hiển thị thông báo và chuyển sang màn hình Login
                    MessageBox.Show("Đăng ký thông tin nhà cung cấp thành công! Hãy chờ Admin duyệt!");
                    Login login = new Login();
                    login.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Đăng ký thông tin nhà cung cấp thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        // Kiểm tra email hợp lệ
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // Kiểm tra số điện thoại hợp lệ (Việt Nam: 10 chữ số)
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^0\d{9}$");
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

                // Get the timestamp and append it to the filename
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileNameWithTimestamp = System.IO.Path.GetFileNameWithoutExtension(fileName) + "_" + timestamp + System.IO.Path.GetExtension(fileName);

                destinationPath = System.IO.Path.Combine(saveDirectory, fileNameWithTimestamp);

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
