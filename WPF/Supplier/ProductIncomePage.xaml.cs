
using System.Windows;
using System.Windows.Controls;

using DataAccess.Models;
using Service;

namespace WPF.Supplier
{
    /// <summary>
    /// Interaction logic for ProductIncomePage.xaml
    /// </summary>
    public partial class ProductIncomePage : Page
    {
        private WarehousesService warehousesService;
        private OrderService orderService;
        private ProductService productService;
        private SupplierService SupplierService;
        private OrderDetailService orderDetailService;
        private DataAccess.Models.User user;

        public ProductIncomePage()
        {
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            warehousesService = new WarehousesService();
            orderService = new OrderService();
            productService = new ProductService();
            SupplierService = new SupplierService();
            orderDetailService = new OrderDetailService();
            InitializeComponent();
        }

        public ProductIncomePage(int warehouseId)
        {
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            warehousesService = new WarehousesService();
            orderService = new OrderService();
            productService = new ProductService();
            orderDetailService = new OrderDetailService();
            InitializeComponent();
            if(warehouseId != 0)
            {
                WarehouseComboBox.SelectedValue = warehouseId;
            }

        }

        public void load()
        {
            WarehouseComboBox.ItemsSource = warehousesService.GetAllWarehousesByIdSupplier(SupplierService.GetSupplierByUserId(user.UserId).SupplierId);
            WarehouseComboBox.DisplayMemberPath = "WarehouseName";
            WarehouseComboBox.SelectedValuePath = "WarehouseId";
        }

        private void WarehouseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WarehouseComboBox.SelectedValue == null)
            {
                MessageBox.Show("Không có giá trị được chọn trong WarehouseComboBox.");
                return;
            }

            var lst = orderService.GetAllOrdersPaid();
            int total = 0;
            var lstDisplay = new List<OrderDetail>();
            foreach (var item in lst)
            {
                var orderDetail = orderDetailService.GetAllOrdersByOrderId(item.OrderId);
                foreach (var item1 in orderDetail)
                {
                    if (item1.WarehouseId == (int)WarehouseComboBox.SelectedValue)
                    {
                        lstDisplay.Add(item1);
                        total += (int)item1.PriceAtOrder;
                    }
                }
            }

            if (lstDisplay.Count == 0)
            {
                MessageBox.Show("Không có đơn hàng nào khớp với kho đã chọn.");
            }

            IncomeDataGrid.ItemsSource = lstDisplay;
            TotalRevenueText.Text = $"Tổng doanh thu: {total} VND";
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           load();
        }
    }
}
