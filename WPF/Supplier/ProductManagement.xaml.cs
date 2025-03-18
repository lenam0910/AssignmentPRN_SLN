
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DataAccess.Models;
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
        private DataAccess.Models.User user;
        public ProductManagement()
        {
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;

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
            lstProducts.ItemsSource = productService.GetAllProductsBySupplierId(supplier.SupplierId);

        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra input đầu vào
                if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                    string.IsNullOrWhiteSpace(txtPrice.Text) ||
                    cbCategory.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra UserAccount
                if (!Application.Current.Properties.Contains("UserAccount") ||
                    Application.Current.Properties["UserAccount"] is not DataAccess.Models.User u)
                {
                    MessageBox.Show("Không tìm thấy tài khoản người dùng. Vui lòng đăng nhập lại.");
                    return;
                }

                // Kiểm tra giá sản phẩm hợp lệ
                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Giá sản phẩm phải là một số hợp lệ và lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra số lượng sản phẩm hợp lệ
                if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
                {
                    MessageBox.Show("Số lượng sản phẩm phải là một số nguyên không âm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Tạo sản phẩm mới
                Product newProduct = new Product
                {
                    ProductName = txtProductName.Text.Trim(),
                    Price = price,
                    QuantityInStock = stock,
                    CategoryId = (int)cbCategory.SelectedValue,
                    SupplierId = supplier?.SupplierId ?? 0, // Kiểm tra supplier có null không
                    Description = txtDescription.Text?.Trim(),
                    Avatar = !string.IsNullOrEmpty(destinationPath) ? destinationPath : null // Chỉ lưu avatar nếu có đường dẫn
                };

                // Thêm sản phẩm vào database
                if (productService.AddProduct(newProduct))
                {
                    // Lưu avatar nếu có ảnh
                    if (!string.IsNullOrEmpty(destinationPath))
                    {
                        saveAvatar();
                    }

                    MessageBox.Show("Thêm sản phẩm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    clear();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Thêm sản phẩm thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void saveAvatar()
        {
            File.Copy(selectedFilePath, destinationPath, true);
        }
        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                fileName = System.IO.Path.GetFileName(selectedFilePath);

                // Get the timestamp and append it to the filename
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileNameWithTimestamp = System.IO.Path.GetFileNameWithoutExtension(fileName) + "_" + timestamp + System.IO.Path.GetExtension(fileName);

                destinationPath = System.IO.Path.Combine(saveDirectory, fileNameWithTimestamp);

                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(saveDirectory);



                // Hiển thị ảnh lên UI
                imgProduct.Source = new BitmapImage(new Uri(selectedFilePath));
                imgProduct.Visibility = Visibility.Visible;

            }
        }
     


      
        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = (sender as Button).DataContext as Product;
           
            if (selectedProduct != null)
            {
                Title.Text = "Sửa Thông Tin Sản Phẩm";
                txtId.Text = selectedProduct.ProductId.ToString();
                txtProductName.Text = selectedProduct.ProductName;
                txtPrice.Text = selectedProduct.Price.ToString();
                txtStock.Text = selectedProduct.QuantityInStock.ToString();
                cbCategory.SelectedValue = selectedProduct.CategoryId;
                txtDescription.Text = selectedProduct.Description;

                if (!string.IsNullOrEmpty(selectedProduct.Avatar))
                {
                    imgProduct.Source = new BitmapImage(new Uri(selectedProduct.Avatar));
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
            try
            {
                // Kiểm tra ID sản phẩm hợp lệ
                if (!int.TryParse(txtId.Text, out int productId))
                {
                    MessageBox.Show("ID sản phẩm không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Lấy thông tin sản phẩm từ database
                var product = productService.GetProductById(productId);
                if (product == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra input đầu vào
                if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                    string.IsNullOrWhiteSpace(txtPrice.Text) ||
                    string.IsNullOrWhiteSpace(txtStock.Text) ||
                    cbCategory.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra giá sản phẩm hợp lệ
                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Giá sản phẩm phải là một số hợp lệ và lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra số lượng sản phẩm hợp lệ
                if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
                {
                    MessageBox.Show("Số lượng sản phẩm phải là một số nguyên không âm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cập nhật thông tin sản phẩm
                product.ProductName = txtProductName.Text.Trim();
                product.Price = price;
                product.QuantityInStock = stock;
                product.CategoryId = (int)cbCategory.SelectedValue;
                product.Description = txtDescription.Text?.Trim();
                product.IsApproved = false;

                // Chỉ cập nhật Avatar nếu có thay đổi
                if (!string.IsNullOrEmpty(destinationPath))
                {
                    product.Avatar = destinationPath;
                }

                // Cập nhật sản phẩm
                if (productService.UpdaterProduct(product))
                {
                    saveAvatar();
                    Title.Text = "Thêm Sản Phẩm";
                    addProduct.Visibility = Visibility.Visible;
                    stpBtn.Visibility = Visibility.Collapsed;
                    imgProduct.Visibility = Visibility.Collapsed;
                    clear();
                    LoadData();
                    MessageBox.Show("Sửa thông tin sản phẩm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Sửa thông tin sản phẩm thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

