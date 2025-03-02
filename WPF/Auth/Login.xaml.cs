using System;
using System.Collections.Generic;
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
using Service;
using WPF;
using WPF.Admin;
using WPF.Supplier;

namespace WPF
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private UserService userService;
        private UserSupplierService userSupplierService;    
        public Login()
        {
            userSupplierService = new UserSupplierService();    
            InitializeComponent();
            userService = new();

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

            string uName = name.Text;
            string unHashPass = password.Password;
            //string hashedPass = HashPassword(unHashPass);
            var account = userService.Login(uName, unHashPass);

            if (account != null )
            {
                DataAccess.Models.Supplier supplier = userSupplierService.GetSupplierByUserId(account.UserId);
                if (rememberMeCheckBox.IsChecked == true)
                {
                    if (Application.Current.Properties.Contains("userNameRemem"))
                    {
                        Application.Current.Properties["userNameRemem"] = uName;
                    }
                    else
                    {
                        Application.Current.Properties.Add("userNameRemem", uName);
                    }

                    if (Application.Current.Properties.Contains("pwdRemem"))
                    {
                        Application.Current.Properties["pwdRemem"] = unHashPass;
                    }
                    else
                    {
                        Application.Current.Properties.Add("pwdRemem", unHashPass);
                    }
                }
                else
                {
                    Application.Current.Properties.Remove("userNameRemem");
                    Application.Current.Properties.Remove("pwdRemem");
                }
                Application.Current.Properties["UserAccount"] = account;
                if (account.RoleId == 1)
                {
                    AdminDashboard adminDashboard = new AdminDashboard();   
                    this.Hide();
                    adminDashboard.Show();
                }
                else if (account.RoleId == 2)
                {
                   
                }

                else
                {
                    if(supplier == null)
                    {
                        RegisterSupplier registerSupplier = new();
                        registerSupplier.Show();
                        this.Hide();
                    }
                    else
                    {
                        SupplierDashboard supplierDashboard = new();
                        supplierDashboard.Show();
                        this.Hide();
                    }
                        
                }
                
               
            }
            else
            {
                MessageBox.Show("LOGIN ERROR !!!");
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
