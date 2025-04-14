 using System;
using System.Collections.Generic;
using System.Drawing;
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
using QRCoder;
using Service;
using static System.Net.Mime.MediaTypeNames;

namespace WPF.User
{
    /// <summary>
    /// Interaction logic for EditProfilePage.xaml
    /// </summary>
    public partial class EditProfilePage : Page
    {
        private string saveDirectoryUser = @"C:\Users\ADMIN\Desktop\PRN212\AssignmentPRN\AssignmentPRN_SLN\DataAccess\Images\Avar\";
        private  UserService userService;
        private string selectedFilePath;
        private string fileName;
        private DataAccess.Models.User user;
        private string destinationPathUser = null;
        public EditProfilePage()
        {
            userService = new UserService();
            user = System.Windows.Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
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

            if (user.Avatar != null)
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

           
        }

        private void DeleteOldUserAvatar()
        {
            if (!string.IsNullOrEmpty(user.Avatar) && File.Exists(user.Avatar))
            {
                File.Delete(user.Avatar);
            }
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                    string.IsNullOrWhiteSpace(txtUserEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtUserPhone.Text) ||
                    string.IsNullOrWhiteSpace(txtUserAddress.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
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

                if (!string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    string hassPass = HashPassword(txtPassword.Password);
                    user.Password = hassPass;
                }

                if (imgUserAvatar.Source != null && !string.IsNullOrEmpty(destinationPathUser))
                {
                    DeleteOldUserAvatar();
                    user.Avatar = destinationPathUser;
                }

                bool isUpdated = userService.UpdateUser(user);
                if (isUpdated)
                {
                    if (imgUserAvatar.Source != null && !string.IsNullOrEmpty(destinationPathUser))
                    {
                        Console.WriteLine(destinationPathUser);
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

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        }
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }


        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^0\d{9}$");
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void CloseOverlay_Click(object sender, RoutedEventArgs e)
        {
            imgQRCode.Source = null;
            qrOverlay.Visibility = Visibility.Collapsed;
        }

        private void btnGenerateQR_Click(object sender, RoutedEventArgs e)
        {
            string input = user.UserId.ToString();

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(input, QRCodeGenerator.ECCLevel.H))
            using (QRCode qrCode = new QRCode(qrCodeData))
            using (Bitmap qrBitmap = qrCode.GetGraphic(20))
            using (MemoryStream ms = new MemoryStream())
            {
                qrBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                BitmapImage qrImage = new BitmapImage();
                qrImage.BeginInit();
                qrImage.CacheOption = BitmapCacheOption.OnLoad;
                qrImage.StreamSource = ms;
                qrImage.EndInit();

                imgQRCode.Source = qrImage;
                qrOverlay.Visibility = Visibility.Visible;
            }
        }
    }
}
