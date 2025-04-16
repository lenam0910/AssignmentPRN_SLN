﻿
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media.Imaging;
using QRCoder;
using Service;
using WPF.User;

namespace WPF.Supplier
{

    public partial class UpdateProfilePage : Page
    {
        private string saveDirectoryUser = @"D:\FPTU\Kì5\PRN212\AssignmentPRN\AssignmentPRN_SLN\DataAccess\Images\Avar\";
        private string saveDirectorySupplier = @"D:\FPTU\Kì5\PRN212\AssignmentPRN\AssignmentPRN_SLN\WPF\Supplier\";

        private string selectedFilePath;
        private string fileName;
        private string destinationPathUser = null;
        private string destinationPathSupplier = null;
        private EmailSenderService emailSenderService;
        private UserService _userService;
        private SupplierService _supplierService;
        private DataAccess.Models.User user;
        private DataAccess.Models.Supplier supplier;
        public UpdateProfilePage()
        {
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            _userService = new UserService();
            _supplierService = new SupplierService();
            emailSenderService = new EmailSenderService();  
            supplier = _supplierService.GetSupplierByUserId(user.UserId);
            InitializeComponent();
            Load();
        }

        private void Load()
        {

            txtFullName.Text = user.Username;
            txtUserEmail.Text = user.Email;
            txtPassword.Password = user.Password;
            txtUserPhone.Text = user.Phone;
            txtUserAddress.Text = user.Address;
            if (!string.IsNullOrEmpty(user.Avatar) )
            {
                string imagePath = user.Avatar;

                if (File.Exists(imagePath))
                {
                    imgUserAvatar.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                }
            }
            else
            {
                imgUserAvatar = null;
            }

            txtSupplierName.Text = supplier.SupplierName;
            txtSupplierEmail.Text = supplier.Email;
            txtSupplierPhone.Text = supplier.Phone;


            DataAccess.Models.Supplier sup = _supplierService.GetSupplierByUserId(user.UserId);
            if (!string.IsNullOrEmpty(sup.Avatar))
            {
                string imagePathSupplier = supplier.Avatar;

                if (File.Exists(imagePathSupplier))
                {
                    imgSupplierAvatar.Source = new BitmapImage(new Uri(imagePathSupplier, UriKind.Absolute));
                }
            }
            else
            {
                imgSupplierAvatar.Source = null; 
            }

        }

        private void ChangeSupplierAvatar_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {

                selectedFilePath = openFileDialog.FileName;
                fileName = System.IO.Path.GetFileName(selectedFilePath);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileNameWithTimestamp = System.IO.Path.GetFileNameWithoutExtension(fileName) + "_" + timestamp + System.IO.Path.GetExtension(fileName);

                destinationPathSupplier = System.IO.Path.Combine(saveDirectorySupplier, fileNameWithTimestamp);
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

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileNameWithTimestamp = System.IO.Path.GetFileNameWithoutExtension(fileName) + "_" + timestamp + System.IO.Path.GetExtension(fileName);

                destinationPathUser = System.IO.Path.Combine(saveDirectoryUser, fileNameWithTimestamp);

                Directory.CreateDirectory(saveDirectoryUser);



                imgUserAvatar.Source = new BitmapImage(new Uri(selectedFilePath));
                imgUserAvatar.Visibility = Visibility.Visible;

            }
        }
        private void SaveSupplier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSupplierName.Text) ||
                    string.IsNullOrWhiteSpace(txtSupplierEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtSupplierPhone.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin nhà cung cấp.");
                    return;
                }

                if (!IsValidEmail(txtSupplierEmail.Text))
                {
                    MessageBox.Show("Email không hợp lệ!");
                    return;
                }

                if (!IsValidPhoneNumber(txtSupplierPhone.Text))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ!");
                    return;
                }

                supplier.SupplierName = txtSupplierName.Text.Trim();
                supplier.Email = txtSupplierEmail.Text.Trim();
                supplier.Phone = txtSupplierPhone.Text.Trim();

                if (imgSupplierAvatar.Source != null && !string.IsNullOrEmpty(destinationPathSupplier))
                {
                    DeleteOldSupplierAvatar();
                    supplier.Avatar = destinationPathSupplier;
                }

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
        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        }
        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                    string.IsNullOrWhiteSpace(txtUserEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtUserPhone.Text) ||
                    string.IsNullOrWhiteSpace(txtUserAddress.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin người dùng.");
                    return;
                }

                if (!IsValidEmail(txtUserEmail.Text))
                {
                    MessageBox.Show("Email không hợp lệ!");
                    return;
                }

                if (!IsValidPhoneNumber(txtUserPhone.Text))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ!");
                    return;
                }

                user.FullName = txtFullName.Text.Trim();
                user.Email = txtUserEmail.Text.Trim();
                user.Phone = txtUserPhone.Text.Trim();
                user.Address = txtUserAddress.Text.Trim();

                if (!string.IsNullOrWhiteSpace(txtPassword.Password) && !txtPassword.Password.ToString().Equals(user.Password.ToString()))
                {
                    string hassPass = HashPassword(txtPassword.Password);
                    user.Password = hassPass;
                }

                if (imgUserAvatar.Source != null && !string.IsNullOrEmpty(destinationPathUser))
                {
                    user.Avatar = destinationPathUser;
                }

                if (_userService.UpdateUser(user))
                {
                    if (imgUserAvatar.Source != null)
                    {

                        saveAvatar();
                        foreach (Window window in System.Windows.Application.Current.Windows)
                        {
                            if (window is SupplierDashboard supplierDashboard)
                            {
                                supplierDashboard.RefreshUserProfile();
                                break;
                            }
                        }
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

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^0\d{9}$");
        }





       

        private void DeleteOldSupplierAvatar()
        {
            if (!string.IsNullOrEmpty(supplier.Avatar) && File.Exists(supplier.Avatar))
            {
                File.Delete(supplier.Avatar);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string input = user.UserId.ToString();
            string userEmail = user.Email; 

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(input, QRCodeGenerator.ECCLevel.H))
            using (QRCode qrCode = new QRCode(qrCodeData))
            using (Bitmap qrBitmap = qrCode.GetGraphic(20))
            using (MemoryStream ms = new MemoryStream())
            {
                qrBitmap.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                BitmapImage qrImage = new BitmapImage();
                qrImage.BeginInit();
                qrImage.CacheOption = BitmapCacheOption.OnLoad;
                qrImage.StreamSource = ms;
                qrImage.EndInit();

                imgQRCode.Source = qrImage;
                qrOverlay.Visibility = Visibility.Visible;

                ms.Position = 0;
                bool emailSent = emailSenderService.SendQRCodeEmail(userEmail, ms);
                if (emailSent)
                {
                    MessageBox.Show("Mã QR đã được gửi đến email của bạn!");
                }
            }
        }

        private void CloseOverlay_Click(object sender, RoutedEventArgs e)
        {
            imgQRCode.Source = null;
            qrOverlay.Visibility = Visibility.Collapsed;
        }
    }
}
