using System.Text.RegularExpressions;
using System.Windows;
using DataAccess.Models;
using Service;
using System.IO;
using System.Windows.Media.Imaging;

namespace WPF
{
    public partial class Register : Window
    {
        private string saveDirectory = @"C:\Users\THIS PC\Desktop\prn211\AssignmentPRN_SLN\DataAccess\Images\Avar\";
        private readonly UserService service;
        private string selectedFilePath;
        private string fileName;
        private string destinationPath = null;
        public Register()
        {
            InitializeComponent();
            service = new UserService();
        }

        

bool ValidateInputs()
        {
            DateOnly.TryParse(dobPicker.Text, out DateOnly dob);

            string userName = uname.Text.Trim();
            string pass = pwd.Password.Trim();
            string fullName = fullname.Text.Trim();
            string emailText = email.Text.Trim();
            string phoneText = phone.Text.Trim();
            string Address = txtAddress.Text.Trim();
            string area = txtArea.Text.Trim();
            string role = radioCheck().RoleName;
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(role) ||                           
                string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(emailText) || string.IsNullOrEmpty(phoneText) || string.IsNullOrEmpty(Address)
                || string.IsNullOrEmpty(area))
            {
                System.Windows.MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (service.checkDuplicateUserName(userName))
            {
                System.Windows.MessageBox.Show("Tên đăng nhập đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (dob.Year >= 2007)
            {
                System.Windows.MessageBox.Show("Yêu cầu 18 tuổi !", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(emailText, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                System.Windows.MessageBox.Show("Email không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(phoneText, @"^\d{10}$"))
            {
                System.Windows.MessageBox.Show("Số điện thoại phải là số và có từ 9 đến 15 chữ số!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private Role radioCheck()
        {
            if (rdASupplier.IsChecked == true)
            {
                return service.GetRole("Supplier");
            }
            else
            {
                return service.GetRole("User");
            }
        }

        private DataAccess.Models.User setUser()
        {
            if (!ValidateInputs()) return null;
            DateOnly.TryParse(dobPicker.Text, out DateOnly dob);
            DataAccess.Models.User u = new()
            {
                Username = uname.Text.Trim(),
                Password = HashPassword(pwd.Password.Trim()), 
                FullName = fullname.Text.Trim(),
                Email = email.Text.Trim(),
                Phone = phone.Text.Trim(),
                Gender = genderComboBox.Text,
                DateOfBirth = dob,
                Address = txtAddress.Text + " | " + txtArea.Text,
                RoleId = radioCheck().RoleId,
                Avatar = !string.IsNullOrEmpty(destinationPath) ? destinationPath : null
            };
            if (!string.IsNullOrEmpty(destinationPath) && !string.IsNullOrEmpty(selectedFilePath))
            {
                saveAvatar();
            }
            return u;
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataAccess.Models.User user = setUser();
                if (user == null) return; 

                if (service.CreateUser(user))
                {
                    System.Windows.MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Hide();
                    Login login = new();
                    login.Show();
                }
                else
                {
                    System.Windows.MessageBox.Show("Đăng ký thất bại. Vui lòng thử lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi lưu vào database: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi không xác định: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void clear()
        {
            uname.Text = "";
            pwd.Password = "";
            fullname.Text = "";
            email.Text = "";
            phone.Text = "";
            genderComboBox.Text = "";
            dobPicker.Text = "";
            rdASupplier.IsChecked = false;
            txtAddress.Text = "";
            txtArea.Text = "";
            rdUser.IsChecked = true;
            avatarImage.Source = null;
        }
        private void clearForm_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }

        private void UploadAvatar_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                fileName = Path.GetFileName(selectedFilePath);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileNameWithTimestamp = Path.GetFileNameWithoutExtension(fileName) + "_" + timestamp + Path.GetExtension(fileName);

                destinationPath = Path.Combine(saveDirectory, fileNameWithTimestamp);

                Directory.CreateDirectory(saveDirectory);

                avatarImage.Source = new BitmapImage(new Uri(selectedFilePath));
                avatarImage.Visibility = Visibility.Visible;
            }
        }

        private void saveAvatar()
        {
            File.Copy(selectedFilePath, destinationPath, true);
        }

    }
}
