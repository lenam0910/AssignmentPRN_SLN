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
    /// Interaction logic for ManageProducts.xaml
    /// </summary>
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
            ProductPopup.Visibility = Visibility.Visible;
            ProductGrid.SelectedItem = null;
        }

        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            Product product = null;
            product = ProductGrid.SelectedItem as Product;
            if (product != null)
            {
                product.ProductName = txtProductName.Text;
                product.Price = decimal.Parse(txtProductPrice.Text);
                product.QuantityInStock = int.Parse(txtProductQuantity.Text);
                product.Description = txtProductDescription.Text;
                product.CategoryId = (int)cbCategories.SelectedValue;
                product.SupplierId = (int)cbSuppliers.SelectedValue;
                product.IsApproved = true;

                if (ProductService.UpdaterProduct(product))
                {
                    ProductPopup.Visibility = Visibility.Collapsed;
                    load();
                    clear();
                    MessageBox.Show("Sửa sản phẩm thành công!");
                }


            }
            else
            {
                product = new();
                product.ProductName = txtProductName.Text;
                product.Price = decimal.Parse(txtProductPrice.Text);
                product.QuantityInStock = int.Parse(txtProductQuantity.Text);
                product.Description = txtProductDescription.Text;
                product.CategoryId = (int)cbCategories.SelectedValue;
                product.SupplierId = (int)cbSuppliers.SelectedValue;
                product.IsApproved = true;
                if ( ProductService.AddProduct(product)){
                    ProductPopup.Visibility = Visibility.Collapsed;

                    load();
                    clear();
                    MessageBox.Show("Thêm sản phẩm thành công!");
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
