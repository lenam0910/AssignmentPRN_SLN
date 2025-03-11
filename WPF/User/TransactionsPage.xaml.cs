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

namespace WPF.User
{
    /// <summary>
    /// Interaction logic for TransactionsPage.xaml
    /// </summary>
    public partial class TransactionsPage : Page
    {
        private OrderService orderService;
        private OrderDetailService orderDetailService;
        private DataAccess.Models.User user;
        private ProductService productService;
        private Order order;
        public TransactionsPage()
        {
            productService = new ProductService();
            user = System.Windows.Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            orderService = new OrderService();
            orderDetailService = new OrderDetailService();
            InitializeComponent();
        }

        private void load()
        {
            var lstOrder = orderService.GetAllOrdersByUserId(user.UserId);
            //decimal total = 0;
            //foreach (var item in lstOrder)
            //{
            //    if (item.OrderId != order.OrderId)
            //    {
            //        lstOrder.Remove(item);
            //    }
            //    else
            //    {
            //        total += item.PriceAtOrder;
            //    }
            //}
            //var lstProduct = productService.GetAllProducts();
            //foreach (var item in lstOrder)
            //{
            //    foreach (var item2 in lstProduct)
            //    {
            //        if (item.ProductId == item2.ProductId)
            //        {
            //            item.Product = item2;
            //        }
            //    }
            //}
            OrdersListView.ItemsSource = lstOrder;
        }
        private void OrdersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            order = OrdersListView.SelectedItem as Order;
            var lstOrder = orderDetailService.GetAllOrders();
            decimal total = 0;
            foreach (var item in lstOrder)
            {
                if (item.OrderId != order.OrderId)
                {
                    lstOrder.Remove(item);
                }
                else
                {
                    total += item.PriceAtOrder;
                }
            }
            var lstProduct = productService.GetAllProducts();
            foreach (var item in lstOrder)
            {
                foreach (var item2 in lstProduct)
                {
                    if (item.ProductId == item2.ProductId)
                    {
                        item.Product = item2;
                    }
                }
            }
            OrderIdText.Text = "Mã đơn hàng: " + order.OrderId;
            OrderDateText.Text = "Ngày đặt hàng: " + order.OrderDate;
            OrderStatusText.Text = "Trạng thái: " + order.Status;
            OrderDetailsDataGrid.ItemsSource = lstOrder;
            ProductListView.Visibility = Visibility.Visible;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load();
        }
    }
}
