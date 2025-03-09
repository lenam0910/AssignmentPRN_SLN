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
    /// Interaction logic for OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private OrderService orderService;
        private OrderDetailService orderDetailService;
        private DataAccess.Models.User user;
        private Order order;
        private ProductService productService;
        private InventoryService inventoryService;
        public OrdersPage()
        {
            inventoryService = new InventoryService();
            productService = new ProductService();
            orderDetailService = new OrderDetailService();
            orderService = new OrderService();
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            InitializeComponent();
        }

        private void load()
        {
            order = orderService.GetOrderByUserId(user.UserId);
            if(order == null)
            {
                MessageBox.Show("Bạn chưa thêm đơn hàng nào!");
                return;
            }
            var lstOrder = orderDetailService.GetAllOrders();
            decimal total = 0;
            foreach (var item in lstOrder)
            {
                if (item.OrderId != order.OrderId )
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
            
            OrdersListView.ItemsSource = lstOrder;
            
            TotalAmountText.Text = "Tổng tiền: "+ total + "đ";
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load();
        }

        public int getQuantityProduct(int ProductId,int warehouseId)
        {
            var inventory = inventoryService.GetInventoryByWarehousesID(warehouseId);
            var products = productService.GetAllProducts();
            int quantity = 0;
            if (products == null || !products.Any())
            {
                MessageBox.Show("Danh sách sản phẩm trống!");
            }
            foreach (Product product in products)
            {
                    if (inventory.ProductId == product.ProductId && product.ProductId == ProductId)
                    {
                        quantity = inventory.Quantity;
                        return quantity;
                    }
            }
            return quantity;
        }
        private void BuyNow_Click(object sender, RoutedEventArgs e)
        {
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
            var lstProduct = inventoryService.GetInventoryList();
            foreach (var item in lstOrder)
            {
                foreach (var item2 in lstProduct)
                {
                    if (item.ProductId == item2.ProductId )
                    {
                        item2.Quantity -= item.Quantity;
                    }
                }
            }

            order.Status = "Đã Thanh Toán";
            if (orderService.updateOrder(order))
            {
                NavigationService?.Navigate(new TransactionsPage());
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (sender is System.Windows.Controls.Button button && button.DataContext is OrderDetail orderDetail)
            {
                orderDetail.IsDeleted = true;
                orderDetailService.UpdateOrderDetail(orderDetail);
                load();
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (sender is System.Windows.Controls.Button button && button.DataContext is OrderDetail orderDetail)
            {
                if (orderDetail.Quantity > 1)
                {
                    orderDetail.Quantity--;
                    orderDetail.PriceAtOrder = orderDetail.Quantity * orderDetail.Product.Price;
                    orderDetailService.UpdateOrderDetail(orderDetail);
                    load();
                }
            }
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (sender is System.Windows.Controls.Button button && button.DataContext is OrderDetail orderDetail)
            {
                if(orderDetail.Quantity < getQuantityProduct(orderDetail.ProductId,orderDetail.WarehouseId))
                {
                    orderDetail.Quantity++;
                    orderDetail.PriceAtOrder = orderDetail.Quantity * orderDetail.Product.Price;
                    orderDetailService.UpdateOrderDetail(orderDetail);
                    load();
                }
            }
        }
    }
}
