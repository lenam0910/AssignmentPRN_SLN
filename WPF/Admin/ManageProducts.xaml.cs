
using System.Windows;
using System.Windows.Controls;
using DataAccess.Models;
using Service;

namespace WPF.Admin
{
   
    public partial class ManageProducts : Page
    {
        private ProductService ProductService;
        private CategoryService CategoryService;
        private SupplierService SupplierService;
        public ManageProducts()
        {
            InitializeComponent();
            ProductService = new ProductService();
            CategoryService = new CategoryService();
            SupplierService = new SupplierService();
        }

        private void OpenPendingProductsPopup(object sender, RoutedEventArgs e)
        {
            PendingProductsPopup.Visibility = Visibility.Visible;
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            Product product = ProductGrid.SelectedItem as Product;
            if (product != null)
            {

                product.IsDeleted = true;
                if(ProductService.DeleteProduct(product)){
                    load();
                    clear();
                    MessageBox.Show("Xóa sản phẩm thành công!");
                }
            }
            else
            {
                MessageBox.Show("Hãy chọn sản phẩm trước khi chỉnh xóa!");
            }
        }

        private void OpenEditProductPopup(object sender, RoutedEventArgs e)
        {
            Product product = ProductGrid.SelectedItem as Product;
            if (product != null) {
                ProductPopup.Visibility = Visibility.Visible;
            txtProductName.Text = product.ProductName;
                txtProductPrice.Text =  product.Price.ToString();
                txtProductQuantity.Text = product.QuantityInStock.ToString();
                txtProductDescription.Text = product.Description;
                cate.Visibility = Visibility.Visible;
                sup.Visibility = Visibility.Visible;

                cbCategories.ItemsSource = CategoryService.getAll();
                cbSuppliers.ItemsSource = SupplierService.GetAllSuppliers();

                cbCategories.SelectedValue = product.CategoryId;
                cbSuppliers.SelectedValue = product.SupplierId;


            }
            else
            {
                MessageBox.Show("Hãy chọn sản phẩm trước khi chỉnh sửa!");
            }
        }

        private void OpenAddProductPopup(object sender, RoutedEventArgs e)
        {
            cate.Visibility = Visibility.Visible;
            sup.Visibility = Visibility.Visible;
            ProductPopup.Visibility = Visibility.Visible;
            ProductGrid.SelectedItem = null;
        }

        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            string productName = txtProductName.Text.Trim();
            string priceText = txtProductPrice.Text.Trim();
            string quantityText = txtProductQuantity.Text.Trim();
            string description = txtProductDescription.Text.Trim();

            if (string.IsNullOrEmpty(productName))
            {
                MessageBox.Show("Tên sản phẩm không được để trống!");
                return;
            }
            if (productName.Length > 100)
            {
                MessageBox.Show("Tên sản phẩm không được dài quá 100 ký tự!");
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price) || price < 0)
            {
                MessageBox.Show("Giá sản phẩm phải là số hợp lệ và không âm!");
                return;
            }

            if (!int.TryParse(quantityText, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Số lượng tồn kho phải là số nguyên hợp lệ và không âm!");
                return;
            }

            if (description.Length > 500)
            {
                MessageBox.Show("Mô tả không được dài quá 500 ký tự!");
                return;
            }

            int categoryId = (int)cbCategories.SelectedValue;
            int supplierId = (int)cbSuppliers.SelectedValue;
            if (categoryId <= 0 || supplierId <= 0)
            {
                MessageBox.Show("Vui lòng chọn danh mục và nhà cung cấp hợp lệ!");
                return;
            }

            Product product = ProductGrid.SelectedItem as Product;
            if (product != null)
            {
                product.ProductName = productName;
                product.Price = price;
                product.QuantityInStock = quantity;
                product.Description = description;
                product.CategoryId = categoryId;
                product.SupplierId = supplierId;
                product.IsApproved = true;

                if (ProductService.UpdaterProduct(product))
                {
                    ProductPopup.Visibility = Visibility.Collapsed;
                    load();
                    clear();
                    MessageBox.Show("Sửa sản phẩm thành công!");
                }
                else
                {
                    MessageBox.Show("Sửa sản phẩm thất bại!");
                }
            }
            else
            {
                // Thêm sản phẩm mới
                product = new Product
                {
                    ProductName = productName,
                    Price = price,
                    QuantityInStock = quantity,
                    Description = description,
                    CategoryId = categoryId,
                    SupplierId = supplierId,
                    IsApproved = true
                };

                if (ProductService.AddProduct(product))
                {
                    ProductPopup.Visibility = Visibility.Collapsed;
                    load();
                    clear();
                    MessageBox.Show("Thêm sản phẩm thành công!");
                }
                else
                {
                    MessageBox.Show("Thêm sản phẩm thất bại!");
                }
            }
        }
        private void clear()
        {
            txtProductName.Clear();
            txtProductPrice.Clear();
            txtProductQuantity.Clear();
            txtProductDescription.Clear();
            cbCategories.SelectedItem = null;   
            cbSuppliers.SelectedItem = null;
        }
        private void ClosePopup(object sender, RoutedEventArgs e)
        {
            PendingProductsPopup.Visibility = Visibility.Collapsed;
            ProductPopup.Visibility = Visibility.Collapsed;
            clear();
        }

        private void ApproveProduct_Click(object sender, RoutedEventArgs e)
        {
            Product p = PendingProductsList.SelectedItem as Product;
            if (p != null) { 
            p.IsApproved = true;
                ProductService.UpdaterProduct(p);
                load();
            }
            else
            {
                MessageBox.Show("Hãy chọn sản phẩm trước khi duyệt!");
            }
        }
        private void load()
        {
            PendingProductsList.ItemsSource = ProductService.GetAllProductsNotApprove();
            ProductGrid.ItemsSource = ProductService.GetAllProducts();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load();

            cbCategories.ItemsSource = CategoryService.getAll();
            cbCategories.DisplayMemberPath = "CategoryName";
            cbCategories.SelectedValuePath = "CategoryId";


            cbSuppliers.ItemsSource = SupplierService.GetAllSuppliers();
            cbSuppliers.DisplayMemberPath = "SupplierName";
            cbSuppliers.SelectedValuePath = "SupplierId";
        }
    }
}
