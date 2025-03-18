
using System.Windows;
using System.Windows.Controls;
using Service;

namespace WPF.Admin
{
    
    public partial class MainPage : Page
    {
        private ProductService ProductService;
        private SupplierService SupplierService;
        private UserService UserService;
        public MainPage()
        {
            ProductService = new ProductService();
            SupplierService = new SupplierService();
            UserService = new UserService();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ManageUser()); 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ManageSuppliers());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ManageProducts());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int numberProduct = ProductService.GetAllProducts().Count;
            int numberSupplier = SupplierService.GetAllSuppliers().Count;
            int numberUser = UserService.getAll().Count;
            product.Text = numberProduct.ToString();
            supplier.Text = numberSupplier.ToString();
            user.Text = numberUser.ToString();
        }
    }
}
