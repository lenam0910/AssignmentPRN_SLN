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

        private string selectedFilePath;
        private string fileName;
        private string destinationPathUser = null;
        private string destinationPathSupplier = null;

        private UserService _userService;
        private SupplierService _supplierService;
        private DataAccess.Models.User user;
        private DataAccess.Models.Supplier supplier;
        private UserSupplierService userSupplierService;
        public UpdateProfilePage()
        {
            userSupplierService = new();
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
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
            if (user.Avatar != null)
            {
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

                // Get the timestamp and append it to the filename
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileNameWithTimestamp = System.IO.Path.GetFileNameWithoutExtension(fileName) + "_" + timestamp + System.IO.Path.GetExtension(fileName);

                destinationPathUser = System.IO.Path.Combine(saveDirectorySupplier, fileNameWithTimestamp);
                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(saveDirectorySupplier);



                // Hiển thị ảnh lên UI
                imgSupplierAvatar.Source = new BitmapImage(new Uri(selectedFilePath));
                imgSupplierAvatar.Visibility = Visibility.Visible;

            }
        }
        private void saveAvatarSupplier()
        {
            File.Copy(selectedFilePath, destinationPathSupplier, true);
        }

        private void saveAvatar()
        {
            File.Copy(selectedFilePath, destinationPathUser, true);
        }
        private void ChangeUserAvatar_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                fileName = System.IO.Path.GetFileName(selectedFilePath);

                // Get the timestamp and append it to the filename
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileNameWithTimestamp = System.IO.Path.GetFileNameWithoutExtension(fileName) + "_" + timestamp + System.IO.Path.GetExtension(fileName);

                destinationPathUser = System.IO.Path.Combine(saveDirectoryUser, fileNameWithTimestamp);

                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(saveDirectoryUser);



                // Hiển thị ảnh lên UI
                imgUserAvatar.Source = new BitmapImage(new Uri(selectedFilePath));
                imgUserAvatar.Visibility = Visibility.Visible;

            }
        }
        private void SaveSupplier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtSupplierName.Text) ||
                    string.IsNullOrWhiteSpace(txtSupplierEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtSupplierPhone.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin nhà cung cấp.");
                    return;
                }

                // Kiểm tra email hợp lệ
                if (!IsValidEmail(txtSupplierEmail.Text))
                {
                    MessageBox.Show("Email không hợp lệ!");
                    return;
                }

                // Kiểm tra số điện thoại hợp lệ
                if (!IsValidPhoneNumber(txtSupplierPhone.Text))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ!");
                    return;
                }

                // Cập nhật thông tin nhà cung cấp
                supplier.SupplierName = txtSupplierName.Text.Trim();
                supplier.Email = txtSupplierEmail.Text.Trim();
                supplier.Phone = txtSupplierPhone.Text.Trim();

                // Cập nhật Avatar nếu có thay đổi
                if (imgSupplierAvatar.Source != null && !string.IsNullOrEmpty(destinationPathSupplier))
                {
                    DeleteOldSupplierAvatar();
                    supplier.Avatar = destinationPathSupplier;
                }

                // Gọi service để cập nhật
                if (_supplierService.UpdateSupplier(supplier))
                {
                    if (imgSupplierAvatar.Source != null)
                    {
                        saveAvatarSupplier();
                    }
                    MessageBox.Show("Sửa thông tin Công ty cung cấp thành công!");
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi cập nhật thông tin nhà cung cấp.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                    string.IsNullOrWhiteSpace(txtUserEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtUserPhone.Text) ||
                    string.IsNullOrWhiteSpace(txtUserAddress.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin người dùng.");
                    return;
                }

                // Kiểm tra email hợp lệ
                if (!IsValidEmail(txtUserEmail.Text))
                {
                    MessageBox.Show("Email không hợp lệ!");
                    return;
                }

                // Kiểm tra số điện thoại hợp lệ
                if (!IsValidPhoneNumber(txtUserPhone.Text))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ!");
                    return;
                }

                // Cập nhật thông tin người dùng
                user.FullName = txtFullName.Text.Trim();
                user.Email = txtUserEmail.Text.Trim();
                user.Phone = txtUserPhone.Text.Trim();
                user.Address = txtUserAddress.Text.Trim();

                // Chỉ cập nhật mật khẩu nếu người dùng nhập mật khẩu mới
                if (!string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    user.Password = txtPassword.Password;
                }

                // Cập nhật Avatar nếu có thay đổi
                if (imgUserAvatar.Source != null && !string.IsNullOrEmpty(destinationPathUser))
                {
                    DeleteOldUserAvatar();
                    user.Avatar = destinationPathUser;
                }

                // Gọi service để cập nhật
                if (_userService.UpdateUser(user))
                {
                    if (imgUserAvatar.Source != null)
                    {
                        saveAvatar();
                    }
                    MessageBox.Show("Sửa thông tin người dùng thành công!");
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi cập nhật thông tin người dùng.");
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
