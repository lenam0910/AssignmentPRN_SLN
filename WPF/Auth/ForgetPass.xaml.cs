using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using WPF;
using DataAccess.Models;
using Microsoft.Win32;
using Service;

namespace WPF
{
    public partial class ForgetPass : Window
    {
        private string generatedOTP = ""; // Lưu OTP tạm thời
        private DataAccess.Models.User User;
        private UserService service;
        private EmailSenderService emailSenderService;
        public ForgetPass()
        {
            InitializeComponent();
            service = new UserService();
            emailSenderService = new EmailSenderService();
        }



        private void txtEmail_PreviewLostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            User = service.GetUserByEmail(email);
            if (!IsValidGmail(email))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ Gmail hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
            }
        }

        private bool IsValidGmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
            return Regex.IsMatch(email, pattern);
        }

        private void SendEmail_Click(object sender, RoutedEventArgs e)
        {

            string otp = emailSenderService.GenerateOTP();
            string to = txtEmail.Text.Trim();
            if (service.checkExistedGmail(to))
            {
                if (emailSenderService.SendEmail(to, otp))
                {
                    // Ẩn emailPanel, hiện otpPanel
                    MessageBox.Show($"Mã xác nhận đã gửi thành công!", "Email Gửi Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                    emailPanel.Visibility = Visibility.Collapsed;
                    otpPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Lỗi khi gửi email! ", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show($"Không tồn tại Gmail!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }



        }

        private void VerifyOTP_Click(object sender, RoutedEventArgs e)
        {
            if (txtOTP.Text.Trim() == generatedOTP)
            {
                MessageBox.Show("Xác thực OTP thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Ẩn otpPanel, hiện passwordPanel
                otpPanel.Visibility = Visibility.Collapsed;
                passwordPanel.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Mã OTP không chính xác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (txtNewPassword.Password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string newPass = HashPassword(txtNewPassword.Password);
            User.Password = newPass;
            service.UpdateUser(User);
            MessageBox.Show("Mật khẩu đã được đặt lại thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            Login login = new();
            login.Show();
            this.Close(); // Đóng form sau khi đổi mật khẩu thành công

        }
        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            Register register = new();
            this.Hide();
            register.Show();
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Login login = new();
            login.Show();
        }
    }
}
