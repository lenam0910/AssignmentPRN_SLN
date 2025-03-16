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
using Service;

namespace WPF.Admin
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
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
