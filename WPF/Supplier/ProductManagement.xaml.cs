using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Service;

namespace WPF.Supplier
{
    public partial class ProductManagement : Page
    {
        private string saveDirectory = @"C:\Users\ADMIN\Desktop\PRN212\AssignmentPRN\AssignmentPRN_SLN\DataAccess\Images\Product\";
        private readonly UserService service;
        private string selectedFilePath;
        private string fileName;
        private string destinationPath = null;
        private CategoryService categoryService;
        private SupplierService supplierService;
        private ProductService productService;
        private DataAccess.Models.Supplier supplier;
        private UserSupplierService UserSupplierService;
        private User user;
        public ProductManagement()
        {
            user = Application.Current.Properties["UserAccount"] as User;

            UserSupplierService = new UserSupplierService();
            categoryService = new();
            supplierService = new();
            productService = new();
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            supplier = UserSupplierService.GetSupplierByUserId(user.UserId);
            // Load danh mục sản phẩm
            cbCategory.ItemsSource = categoryService.getAll();
            cbCategory.DisplayMemberPath = "CategoryName";
            cbCategory.SelectedValuePath = "CategoryId";


            // Load danh sách sản phẩm
            lstProducts.ItemsSource = productService.GetAllProducts();

        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text) ||
                cbCategory.SelectedValue == null
                )
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            User u = Application.Current.Properties["UserAccount"] as User;
            Product newProduct = new Product
            {
                ProductName = txtProductName.Text,
                Price = decimal.Parse(txtPrice.Text),
                QuantityInStock = int.TryParse(txtStock.Text, out int stock) ? stock : 0,
                CategoryId = (int)cbCategory.SelectedValue,
                SupplierId =supplier.SupplierId,
                Description = txtDescription.Text,
                Avatar = destinationPath

            };

            if (productService.AddProduct(newProduct))
            {
                saveAvatar();
                
                MessageBox.Show("Thêm sản phẩm thành công");
                clear();
                LoadData();
            }
            else
            {
                MessageBox.Show("Thêm sản phẩm thất bại");
            }
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                fileName = Path.GetFileName(selectedFilePath);
                destinationPath = Path.Combine(saveDirectory, fileName);

                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(saveDirectory);



                // Hiển thị ảnh lên UI
                imgProduct.Source = new BitmapImage(new Uri(selectedFilePath));
                imgProduct.Visibility = Visibility.Visible;

            }
        }


        private void saveAvatar()
        {
            try
            {
                File.Copy(selectedFilePath, destinationPath, true);
            }
            catch
            {
                MessageBox.Show("Lưu ảnh bị lỗi");
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = (sender as Button).DataContext as Product;
            string src = "";
            if (selectedProduct.Avatar != null)
            {
                src = selectedProduct.Avatar;
            }
            if (selectedProduct != null)
            {
                Title.Text = "Sửa Thông Tin Sản Phẩm";
                txtId.Text = selectedProduct.ProductId.ToString();
                txtProductName.Text = selectedProduct.ProductName;
                txtPrice.Text = selectedProduct.Price.ToString();
                txtStock.Text = selectedProduct.QuantityInStock.ToString();
                cbCategory.SelectedValue = selectedProduct.CategoryId;
                txtDescription.Text = selectedProduct.Description;

                if (!string.IsNullOrEmpty(src))
                {
                    imgProduct.Source = new BitmapImage(new Uri(src));
                }
                else
                {
                    imgProduct.Source = null;
                }
                imgProduct.Visibility = Visibility.Visible;
                addProduct.Visibility = Visibility.Collapsed;
                stpBtn.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("err");
            }
        }


        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            // Lấy sản phẩm đang được chọn từ ListBox
            var selectedProduct = (sender as Button).DataContext as Product;

            if (selectedProduct != null)
            {
                // Xác nhận xóa sản phẩm
                var result = MessageBox.Show($"Bạn có chắc muốn xóa sản phẩm: {selectedProduct.ProductName}?", "Xác nhận", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {

                    productService.DeleteProduct(selectedProduct);
                    MessageBox.Show("Sản phẩm đã bị xóa.");
                    LoadData();

                }
            }
        }
        private void clear()
        {
            txtProductName.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            txtDescription.Clear();
            cbCategory.SelectedItem = null;
            imgProduct.Source = null;
            imgProduct.Visibility = Visibility.Collapsed;
            destinationPath = null;
        }
        private void editProduct_Click_1(object sender, RoutedEventArgs e)
        {
            var product = productService.GetProductById(int.Parse(txtId.Text));
            product.ProductName = txtProductName.Text;
            product.Price = decimal.Parse(txtPrice.Text);
            product.QuantityInStock = int.Parse(txtStock.Text);
            product.CategoryId = (int)cbCategory.SelectedValue;
            product.Description = txtDescription.Text;
            if(destinationPath != null)
            {
                product.Avatar = destinationPath;
            }
           
            if (productService.UpdaterProduct(product))
            {
                Title.Text = "Thêm Sản Phẩm";
                addProduct.Visibility = Visibility.Visible;
                stpBtn.Visibility = Visibility.Collapsed;
                imgProduct.Visibility = Visibility.Collapsed;
                clear();
                LoadData();
                MessageBox.Show("Sửa thông tin sản phẩm thành công");
            }
            else
            {
                MessageBox.Show("Sửa thông tin sản phẩm thất bại");
            }

        }

        private void CancelProduct_Click(object sender, RoutedEventArgs e)
        {
            clear();
            Title.Text = "Thêm Sản Phẩm";
            addProduct.Visibility = Visibility.Visible;
            stpBtn.Visibility = Visibility.Collapsed;
        }
    }

}

