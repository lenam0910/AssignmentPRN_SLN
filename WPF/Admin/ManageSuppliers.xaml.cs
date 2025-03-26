
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media.Imaging;
using DataAccess.Models;
using Service;

namespace WPF.Admin
{

    public partial class ManageSuppliers : Page
    {
        private SupplierService SupplierService;
        private UserService userService;
        public ManageSuppliers()
        {
            userService = new UserService();
            SupplierService = new SupplierService();
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load();
        }
        private void load()
        {
            SupplierGrid.ItemsSource = SupplierService.GetAllSuppliers();
            PendingSuppliersList.ItemsSource = SupplierService.GetAllSuppliersNotApprove();

            var lstUserSupplier = userService.getAll().Where(x => x.RoleId == 3 && (x.SupplierId == null ||x.Supplier.IsDeleted == true)).ToList();
            cbUsers.ItemsSource = lstUserSupplier;
            cbUsers.DisplayMemberPath = "Username";
            cbUsers.SelectedValuePath = "UserId";   

        }
        private void OpenAddSupplierPopup(object sender, RoutedEventArgs e)
        {
            SupplierPopup.Visibility = Visibility.Visible;
            SupplierGrid.SelectedItem = null;
        }

        private void OpenEditSupplierPopup(object sender, RoutedEventArgs e)
        {
            DataAccess.Models.Supplier supp = SupplierGrid.SelectedItem as DataAccess.Models.Supplier;

            if (supp != null)
            {
                combobox.Visibility = Visibility.Collapsed;
                SupplierPopup.Visibility = Visibility.Visible;
                txtSupplierEmail.Text = supp.Email;
                txtSupplierName.Text = supp.SupplierName;
                txtSupplierPhone.Text = supp.Phone;
                cbUsers.SelectedValue = supp.Users.FirstOrDefault().UserId;
                if (supp.Avatar != null)
                {
                    imgSupplierAvatar.Source = new BitmapImage(new Uri(supp.Avatar));
                    imgSupplierAvatar.Visibility = Visibility.Visible;
                }
                else
                {
                    imgSupplierAvatar = null;
                }
            }
            else
            {
                MessageBox.Show("Hãy chọn nhà cung cấp trước khi sửa!");
            }
        }

        private void DeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            DataAccess.Models.Supplier supp = SupplierGrid.SelectedItem as DataAccess.Models.Supplier;

            if (supp != null)
            {
                supp.IsDeleted = true;
                SupplierService.UpdateSupplier(supp);
                MessageBox.Show("Xoá nhà cung cấp thành công!");
 
            }
            else
            {
                MessageBox.Show("Hãy chọn nhà cung cấp trước khi xóa!");
            }
        }



        private void OpenPendingProductsPopup(object sender, RoutedEventArgs e)
        {
            PendingSuppliersPopup.Visibility = Visibility.Visible;
            SupplierGrid.SelectedItem = null;
        }



        private void clear()
        {
            txtSupplierEmail.Clear();
            txtSupplierName.Clear();
            txtSupplierPhone.Clear();
            cbUsers.SelectedItem = null;
            SupplierGrid.SelectedItem = null;
            SupplierPopup.Visibility = Visibility.Collapsed;
            PendingSuppliersPopup.Visibility = Visibility.Collapsed;
        }
        private void ClosePopup(object sender, RoutedEventArgs e)
        {
            clear();
        }



        private void SaveSupplier_Click(object sender, RoutedEventArgs e)
        {
            string supplierName = txtSupplierName.Text.Trim();
            string email = txtSupplierEmail.Text.Trim();
            string phone = txtSupplierPhone.Text.Trim();

            if (string.IsNullOrEmpty(supplierName))
            {
                MessageBox.Show("Tên nhà cung cấp không được để trống!");
                return;
            }
            if (supplierName.Length > 100)
            {
                MessageBox.Show("Tên nhà cung cấp không được dài quá 100 ký tự!");
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Email không được để trống!");
                return;
            }
            if (email.Length > 150)
            {
                MessageBox.Show("Email không được dài quá 150 ký tự!");
                return;
            }
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không đúng định dạng!");
                return;
            }

            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Số điện thoại không được để trống!");
                return;
            }
            if (!Regex.IsMatch(phone, @"^\d{10,11}$"))
            {
                MessageBox.Show("Số điện thoại phải chứa 10-11 chữ số và chỉ được chứa số!");
                return;
            }

            DataAccess.Models.Supplier supp = SupplierGrid.SelectedItem as DataAccess.Models.Supplier;
            if (supp != null)
            {
                supp.SupplierName = supplierName;
                supp.Email = email;
                supp.Phone = phone;
                supp.IsApproved = true;
                if (SupplierService.UpdateSupplier(supp))
                {
                    SupplierPopup.Visibility = Visibility.Collapsed;
                    load();
                    clear();
                    MessageBox.Show("Sửa nhà cung cấp thành công!");
                }
                else
                {
                    MessageBox.Show("Sửa nhà cung cấp thất bại!");
                }
            }
            else
            {
               DataAccess.Models.User u = userService.GetUserByID((int)cbUsers.SelectedValue);
                supp = new DataAccess.Models.Supplier
                {
                    SupplierName = supplierName,
                    Email = email,
                    Phone = phone,
                    IsApproved = true,

                };

                if (SupplierService.SaveSupplier(supp))
                {
                    u.SupplierId = supp.SupplierId;
                    if(userService.UpdateUser(u)){
                        SupplierPopup.Visibility = Visibility.Collapsed;
                        load();
                        clear();
                        MessageBox.Show("Thêm nhà cung cấp thành công!");
                    }
                    
                }
                else
                {
                    MessageBox.Show("Thêm nhà cung cấp thất bại!");
                }
            }
        }

        private void ApproveSupplier_Click(object sender, RoutedEventArgs e)
        {
            DataAccess.Models.Supplier supp = PendingSuppliersList.SelectedItem as DataAccess.Models.Supplier;
            if (supp != null)
            {
                supp.IsApproved = true;
                SupplierService.UpdateSupplier(supp);
                load();
            }
            else
            {
                MessageBox.Show("Hãy chọn nhà cung cấp trước khi duyệt!");
            }
        }


    }
}
