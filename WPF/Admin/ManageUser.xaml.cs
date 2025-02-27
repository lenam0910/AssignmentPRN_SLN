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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataAccess.Models;
using Service;

namespace WPF.Admin
{
    /// <summary>
    /// Interaction logic for ManageUser.xaml
    /// </summary>
    public partial class ManageUser : Page
    {
        private UserService UserService;
        private RoleService RoleService;
        public ManageUser()
        {
            UserService = new UserService();
            RoleService = new RoleService();
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load();
        }
        private void load()
        {
            var list = UserService.getAll();
            var listRole = RoleService.GetAll();
            foreach (var item in list)
            {
                foreach (var role in listRole)
                {
                    if (item.RoleId == role.RoleId)
                    {
                        item.Role = role;
                    }

                }
            }
            UserGrid.ItemsSource = list;


            roleComboBox.ItemsSource = listRole;
            roleComboBox.DisplayMemberPath = "RoleName";
            roleComboBox.SelectedValuePath = "RoleId";

            roleComboBox2.ItemsSource = listRole;
            roleComboBox2.DisplayMemberPath = "RoleName";
            roleComboBox2.SelectedValuePath = "RoleId";
        }
        public void clear()
        {
            fullname.Clear();
            email.Clear();
            username.Clear();
            password.Clear();
            roleComboBox.SelectedValue = null;
        }
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUserPanel.Visibility = Visibility.Visible;
            EditUserPanel.Visibility = Visibility.Collapsed;

        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {

            AddUserPanel.Visibility = Visibility.Collapsed;
            User user = UserGrid.SelectedItem as User;
            if (user != null)
            {
                EditUserPanel.Visibility = Visibility.Visible;
                txtFullname.Text = user.FullName;
                txtEmail.Text = user.Email;
                txtUsername.Text = user.Username;
                roleComboBox2.SelectedValue = user.Role.RoleId;
            }
            else
            {
                MessageBox.Show("Hãy chọn trong bảng trước khi sửa!");
            }


        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserGrid.SelectedItem is User selectedUser)
            {
                UserService.UpdateUser(selectedUser);
                MessageBox.Show("Xóa thành công!");

            }
            else
            {
                MessageBox.Show("Hãy chọn trong bảng trước khi xóa!");
            }
        }

        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            User user = new User();
            user.FullName = fullname.Text;
            user.Email = email.Text;
            user.Username = username.Text;
            user.Password = password.Password;
            user.RoleId =(int) roleComboBox.SelectedValue;
            if (UserService.CreateUser(user))
            {
                MessageBox.Show("Thêm người dùng thành công!");
                load(); 
                clear();
                AddUserPanel.Visibility = Visibility.Collapsed;
            }
            else {
                MessageBox.Show("Thêm người dùng thất bại!");
            }
        }

        private void CancelAddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUserPanel.Visibility = Visibility.Collapsed;
            clear();
        }

        private void SaveEditUser_Click(object sender, RoutedEventArgs e)
        {
            User user = UserGrid.SelectedItem as User;
            user.FullName = txtFullname.Text;
            user.Email = txtEmail.Text;
            user.Username = txtUsername.Text;
            user.RoleId = (int)roleComboBox2.SelectedValue;
            if (UserService.UpdateUser(user))
            {
                MessageBox.Show("Sửa người dùng thành công!");
                load();
                UserGrid.SelectedItem = null;
                EditUserPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Sửa người dùng thất bại!");
            }
        }

        private void CancelEditUser_Click(object sender, RoutedEventArgs e)
        {
            EditUserPanel.Visibility = Visibility.Collapsed;
            UserGrid.SelectedItem = null;
        }


    }
}
