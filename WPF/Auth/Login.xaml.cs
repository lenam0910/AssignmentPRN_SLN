
using System.Windows;

using Service;
using WPF.Admin;
using WPF.Supplier;
using WPF.User;

namespace WPF
{
    
    public partial class Login : Window
    {
        private UserService userService;
        private SupplierService supplierService;
        public Login()
        {
            InitializeComponent();
            userService = new();
            supplierService = new();

        }


        private void RememberMe()
        {
            if (Application.Current.Properties.Contains("userNameRemem") &&
                Application.Current.Properties.Contains("pwdRemem"))
            {
                string savedUsername = Application.Current.Properties["userNameRemem"].ToString();
                string savedPassword = Application.Current.Properties["pwdRemem"].ToString();

                name.Text = savedUsername;
                password.Password = savedPassword;

                rememberMeCheckBox.IsChecked = true;
            }


        }


        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string uName = name.Text?.Trim();
                string unHashPass = password.Password?.Trim();
                string hassPass = HashPassword(unHashPass);

                if (string.IsNullOrEmpty(uName) || string.IsNullOrEmpty(hassPass))
                {
                    MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var account = userService.Login(uName, hassPass);

                if (account == null)
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var supplier = supplierService.GetSupplierByUserId(account.UserId);

                if (rememberMeCheckBox.IsChecked == true)
                {
                    Application.Current.Properties["userNameRemem"] = uName;
                    Application.Current.Properties["pwdRemem"] = unHashPass; 
                }
                else
                {
                    Application.Current.Properties.Remove("userNameRemem");
                    Application.Current.Properties.Remove("pwdRemem");
                }

                Application.Current.Properties["UserAccount"] = account;

                switch (account.RoleId)
                {
                    case 1:
                        new AdminDashboard().Show();
                        break;

                    case 2:
                        new UserDashboard().Show();
                        break;

                    default:
                        if (supplier == null)
                        {
                            new RegisterSupplier().Show();
                        }
                        else if (supplier.IsApproved == true)
                        {
                            new SupplierDashboard().Show();
                        }
                        else
                        {
                            MessageBox.Show("Tài khoản nhà cung cấp của bạn chưa được kiểm duyệt!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        break;
                }

                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        }
        private void regist_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Register register = new();
            register.Show();
        }

        private void forget_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ForgetPass forget = new();
            forget.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RememberMe();
        }
    }
}
