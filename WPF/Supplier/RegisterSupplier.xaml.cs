
using System.IO;

using System.Text.RegularExpressions;

using System.Windows;

using System.Windows.Media.Imaging;
using DataAccess.Models;
using Microsoft.IdentityModel.Tokens;
using Service;

namespace WPF.Supplier
{
    /// <summary>
    /// Interaction logic for RegisterSupplier.xaml
    /// </summary>
    public partial class RegisterSupplier : Window
    {
        private string saveDirectory = @"C:\Users\THIS PC\Desktop\prn211\AssignmentPRN_SLN\DataAccess\Images\Supplier\";
        private string selectedFilePath;
        private string fileName;
        private string destinationPath ;
        private SupplierService supplierService;
        private UserService userService;
        private DataAccess.Models.User user;    
        public RegisterSupplier()
        {
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
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

                DataAccess.Models.Supplier supplier = new()
                {
                    SupplierName = txtSupplierName.Text.Trim(),
                    ContactInfo = txtContactInfo.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Avatar = !string.IsNullOrEmpty(destinationPath) ? destinationPath : null 
                };

                if (supplierService.SaveSupplier(supplier))
                {
                    user.SupplierId = supplier.SupplierId;



                    if (!string.IsNullOrEmpty(destinationPath) && !string.IsNullOrEmpty(selectedFilePath))
                    {
                                saveAvatarSupplier();
                    }
                    if(userService.UpdateUser(user)){
                        MessageBox.Show("Đăng ký thông tin nhà cung cấp thành công! Hãy chờ Admin duyệt!");
                        Login login = new Login();
                        login.Show();
                        this.Hide();
                    }
                   
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

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

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
