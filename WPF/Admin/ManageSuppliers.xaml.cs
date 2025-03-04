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
using Microsoft.Xaml.Behaviors;
using Service;

namespace WPF.Admin
{
    /// <summary>
    /// Interaction logic for ManageSuppliers.xaml
    /// </summary>
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
                SupplierPopup.Visibility = Visibility.Visible;
                txtSupplierEmail.Text = supp.Email;
                txtSupplierName.Text = supp.SupplierName;
                txtSupplierPhone.Text = supp.Phone;
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
            }
            else
            {
                MessageBox.Show("Hãy chọn nhà cung cấp trước khi xóa!");
            }
        }

       

        private void OpenPendingProductsPopup(object sender, RoutedEventArgs e)
        {
            PendingSuppliersList.Visibility = Visibility.Visible;
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
        

        private void DeleteOldSupplierAvatar(DataAccess.Models.Supplier supplier)
        {
            if (!string.IsNullOrEmpty(supplier.Avatar) && File.Exists(supplier.Avatar))
            {
                File.Delete(supplier.Avatar);
            }
        }
        private void SaveSupplier_Click(object sender, RoutedEventArgs e)
        {
            DataAccess.Models.Supplier supp =  null;
            supp = SupplierGrid.SelectedItem as DataAccess.Models.Supplier;

            if (supp != null)
            {
                supp.SupplierName = txtSupplierName.Text;
                supp.Email  = txtSupplierEmail.Text;   
                supp.Phone = txtSupplierPhone.Text;
                
                supp.IsApproved = true;

                if (SupplierService.UpdateSupplier(supp))
                {
                    SupplierPopup.Visibility = Visibility.Collapsed;
                    load();
                    clear();
                    MessageBox.Show("Sửa sản nhà cung cấp thành công!");
                }


            }
            else
            {
                supp = new();
                supp.SupplierName = txtSupplierName.Text;
                supp.Email = txtSupplierEmail.Text;
                supp.Phone = txtSupplierPhone.Text;

                supp.IsApproved = true;
                if (SupplierService.SaveSupplier(supp))
                {
                    SupplierPopup.Visibility = Visibility.Collapsed;

                    load();
                    clear();
                    MessageBox.Show("Thêm nhà cung cấp thành công!");
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
            }
            else
            {
                MessageBox.Show("Hãy chọn nhà cung cấp trước khi duyệt!");
            }
        }

      
    }
}
