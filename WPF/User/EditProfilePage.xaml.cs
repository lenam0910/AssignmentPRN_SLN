using System;
using System.Collections.Generic;
using System.IO;
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
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
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
                imgUserAvatar.Source = new BitmapImage(new Uri(user.Avatar));
                imgUserAvatar.Visibility = Visibility.Visible;
            }
            else
            {
                imgUserAvatar = null;
            }

           
        }

        private void saveAvatar()
        {
            File.Copy(selectedFilePath, destinationPathUser, true);
        }
        private void DeleteOldUserAvatar()
        {
            if (!string.IsNullOrEmpty(user.Avatar) && File.Exists(user.Avatar))
            {
                File.Delete(user.Avatar);
            }
        }
        private void ChangeUserAvatar_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                fileName = System.IO.Path.GetFileName(selectedFilePath);
                destinationPathUser = System.IO.Path.Combine(saveDirectoryUser, fileName);

                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(saveDirectoryUser);



                // Hiển thị ảnh lên UI
                imgUserAvatar.Source = new BitmapImage(new Uri(selectedFilePath));
                imgUserAvatar.Visibility = Visibility.Visible;

            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            user.FullName = txtFullName.Text;
            user.Email = txtUserEmail.Text;
            user.Password = txtPassword.Password;
            user.Phone = txtUserPhone.Text;
            user.Address = txtUserAddress.Text;
            if (imgUserAvatar.Source != null)
            {
                DeleteOldUserAvatar();
                user.Avatar = destinationPathUser;
            }
            if (userService.UpdateUser(user))
            {
                saveAvatar();
                MessageBox.Show("Sửa thông tin người dùng thành công!");

            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
        }
    }
}
