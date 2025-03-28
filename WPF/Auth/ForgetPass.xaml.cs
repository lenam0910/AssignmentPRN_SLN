﻿
using System.Text.RegularExpressions;
using System.Windows;
using Service;

namespace WPF
{
    public partial class ForgetPass : Window
    {
        private string generatedOTP = "";
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

            generatedOTP = emailSenderService.GenerateOTP().Trim();
            string to = txtEmail.Text.Trim();
            if (service.checkExistedGmail(to))
            {
                if (emailSenderService.SendEmail(to, generatedOTP))
                {

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

        private int otpAttempts = 0;
        private const int maxOtpAttempts = 3;

        private void VerifyOTP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string inputOtp = txtOTP.Text.Trim();

                // Kiểm tra OTP không được để trống
                if (string.IsNullOrEmpty(inputOtp))
                {
                    MessageBox.Show("Vui lòng nhập mã OTP!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra số lần thử OTP
                if (otpAttempts >= maxOtpAttempts)
                {
                    MessageBox.Show("Bạn đã nhập sai OTP quá nhiều lần. Vui lòng thử lại sau!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    btnResendOTP.Visibility = Visibility.Visible;
                    return;
                }

                // Kiểm tra OTP hợp lệ
                if (inputOtp.Equals(generatedOTP, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Xác thực OTP thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    otpPanel.Visibility = Visibility.Collapsed;
                    passwordPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    otpAttempts++; // Tăng số lần nhập sai
                    if (otpAttempts >= maxOtpAttempts)
                    {
                        MessageBox.Show("Bạn đã nhập sai OTP quá nhiều lần. Vui lòng thử gửi lại mã!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        btnResendOTP.Visibility = Visibility.Visible;
                    }
                    else
                        MessageBox.Show($"Mã OTP không chính xác! Bạn còn {maxOtpAttempts - otpAttempts} lần thử.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string newPassword = txtNewPassword.Password.Trim();
                string confirmPassword = txtConfirmPassword.Password.Trim();

                if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu mới!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (newPassword.Length < 6)
                {
                    MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("Mật khẩu nhập lại không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (User == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin người dùng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                User.Password = HashPassword(newPassword);
                if (service.UpdateUser(User))
                {
                    MessageBox.Show("Mật khẩu đã được đặt lại thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    Login login = new();
                    login.Show();
                    this.Close(); 
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi cập nhật mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void ResendOTP_Click(object sender, RoutedEventArgs e)
        {
            otpAttempts = 0;
            generatedOTP = emailSenderService.GenerateOTP().Trim();
            string to = txtEmail.Text.Trim();
            if (service.checkExistedGmail(to))
            {
                if (emailSenderService.SendEmail(to, generatedOTP))
                {
                    // Ẩn emailPanel, hiện otpPanel
                    MessageBox.Show($"Gửi lại mã xác nhận đã gửi thành công!", "Email Gửi Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
}
