
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DataAccess.Models;
using Microsoft.Win32;
using Service;

namespace WPF.Admin
{
   
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
            DataAccess.Models.User user = UserGrid.SelectedItem as DataAccess.Models.User;
            if (user != null)
            {
                EditUserPanel.Visibility = Visibility.Visible;
                txtFullname.Text = user.FullName;
                txtEmail.Text = user.Email;
                txtUsername.Text = user.Username;
                roleComboBox2.SelectedValue = user.RoleId;
            }
            else
            {
                MessageBox.Show("Hãy chọn trong bảng trước khi sửa!");
            }


        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserGrid.SelectedItem is DataAccess.Models.User selectedUser)
            {
                if (UserService.DeleteUser(selectedUser))
                {
                    MessageBox.Show("Xóa thành công!");
                    load();
                }

            }
            else
            {
                MessageBox.Show("Hãy chọn trong bảng trước khi xóa!");
            }
        }
        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        }
        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            DataAccess.Models.User user = new DataAccess.Models.User();
            user.FullName = fullname.Text;
            user.Email = email.Text;
            user.Username = username.Text;
            string hassPass = HashPassword(password.Password);
            user.Password = hassPass;
            user.RoleId = (int)roleComboBox.SelectedValue;
            if (UserService.CreateUser(user))
            {
                MessageBox.Show("Thêm người dùng thành công!");
                load();
                clear();
                AddUserPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
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
            DataAccess.Models.User user = UserGrid.SelectedItem as DataAccess.Models.User;
            user.FullName = txtFullname.Text;
            user.Email = txtEmail.Text;
            user.Username = txtUsername.Text;
            user.RoleId = (int)roleComboBox2.SelectedValue;
            if (UserService.UpdateUserRole(user))
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text Files (*.txt)| *.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                var lstUser = UserService.getAll();
                List<string> lines = new List<string>();
                foreach (var user in lstUser)
                {
                    string ln = $"{user.UserId}-{user.Username}-{user.FullName}-{user.Email}-{user.Phone}-{user.DateOfBirth}-{user.Gender}-{user.Role.RoleName}-{user.Address}\n";
                    lines.Add(ln);
                }
                File.WriteAllLines(saveFileDialog.FileName, lines);
                MessageBox.Show("Lưu file txt thành công !" + saveFileDialog.FileName);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text Files (*.txt)| *.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                string[] lines = File.ReadAllLines(saveFileDialog.FileName);
                List<DataAccess.Models.User> users = new List<DataAccess.Models.User>();
                foreach (var user in lines)
                {
                    if (string.IsNullOrWhiteSpace(user)) continue;
                    string[] parts = user.Split('-');
                    if (parts.Length < 9) continue;

                    DataAccess.Models.User uFile = new DataAccess.Models.User
                    {
                        UserId = int.Parse(parts[0]),
                        Username = parts[1],
                        FullName = parts[2],
                        Email = parts[3],
                        Phone = parts[4],
                        DateOfBirth = DateOnly.TryParse(parts[5], out DateOnly dob) ? dob : null,
                        Gender = parts[6],
                        Role = new Role { RoleName = parts[7] },
                        Address = parts[8],
                    };
                    users.Add(uFile);
                }
                UserGrid.ItemsSource = users;
                Add.Visibility = Visibility.Collapsed;
                edit.Visibility = Visibility.Collapsed;
                dele.Visibility = Visibility.Collapsed;
                export.Visibility = Visibility.Collapsed;
                Reload.Visibility = Visibility.Visible;
            }
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            load();
            Add.Visibility = Visibility.Visible;
            edit.Visibility = Visibility.Visible;
            dele.Visibility = Visibility.Visible;
            export.Visibility = Visibility.Visible;
            Reload.Visibility = Visibility.Collapsed;
        }
    }
}
