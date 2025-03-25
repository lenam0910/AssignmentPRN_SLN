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
            
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            InitializeComponent();
        }

        private void load()
        {
            inventoryService = new InventoryService();
            productService = new ProductService();
            orderDetailService = new OrderDetailService();
            orderService = new OrderService();
            order = orderService.GetOrderByUserId(user.UserId);

            if (order == null || order.OrderDetails == null)
            {
                MessageBox.Show("Bạn chưa thêm đơn hàng nào!");
                return;
            }

            var lstOrder = orderDetailService.GetAllOrders();
            decimal total = 0;

            for (int i = lstOrder.Count - 1; i >= 0; i--)
            {
                var item = lstOrder[i];
                if (item.OrderId != order.OrderId)
                {
                    lstOrder.RemoveAt(i);
                }
                else
                {
                    total += item.PriceAtOrder;
                }
            }

            var lstProduct = inventoryService.GetInventoryList();

            for (int i = lstOrder.Count - 1; i >= 0; i--)
            {
                var orderItem = lstOrder[i];
                bool productExistsAndNotDeleted = false;

                foreach (var product in lstProduct)
                {
                    if (product.ProductId == orderItem.ProductId && product.IsDeleted != true)
                    {
                        productExistsAndNotDeleted = true;
                        orderItem.Product = product.Product;
                    }
                }

                if (!productExistsAndNotDeleted)
                {
                    total -= orderItem.PriceAtOrder;
                    orderItem.IsDeleted = true;
                    orderDetailService.UpdateOrderDetail(orderItem);
                    lstOrder.RemoveAt(i);
                }
            }

        
            if (lstOrder.Count == 0)
            {
                order.Status = "Đã bị hủy";
                orderService.updateOrder(order);  
            }

            OrdersListView.ItemsSource = lstOrder;
            TotalAmountText.Text = "Tổng tiền: " + total + "đ";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load();
        }

        public int getQuantityProduct(int ProductId, int warehouseId)
        {
            inventoryService = new InventoryService();
            productService = new ProductService();
            orderDetailService = new OrderDetailService();
            orderService = new OrderService();
            var inventory = inventoryService.GetInventoryByWarehousesID(warehouseId);
            int quantity = 0;

            foreach (var item2 in inventory)
            {
                if (item2.ProductId == ProductId && item2.WarehouseId == warehouseId)
                {
                    quantity += item2.Quantity;
                }
            }


            return quantity;
        }
        private void BuyNow_Click(object sender, RoutedEventArgs e)
        {
            inventoryService = new InventoryService();
            productService = new ProductService();
            orderDetailService = new OrderDetailService();
            orderService = new OrderService();

            if (order == null || order.OrderDetails == null)
            {
                MessageBox.Show("Bạn chưa có đơn hàng nào để thanh toán!");
                return;
            }

            if (order.Status != "Chờ xử lý")
            {
                MessageBox.Show("Chỉ có thể thanh toán các đơn hàng có trạng thái 'Chờ xử lý'.");
                return;
            }

            var lstOrder = orderDetailService.GetAllOrders();
            decimal total = 0;

            for (int i = lstOrder.Count - 1; i >= 0; i--)
            {
                var item = lstOrder[i];
                if (item.OrderId != order.OrderId)
                {
                    lstOrder.RemoveAt(i);
                }
                else
                {
                    total += item.PriceAtOrder;
                }
            }

            var lstProduct = inventoryService.GetInventoryList();

            for (int i = lstOrder.Count - 1; i >= 0; i--)
            {
                var orderItem = lstOrder[i];
                bool productExistsAndNotDeleted = false;

                foreach (var inventoryItem in lstProduct)
                {
                    if (orderItem.ProductId == inventoryItem.ProductId && inventoryItem.IsDeleted != true)
                    {
                        productExistsAndNotDeleted = true;
                        inventoryItem.Quantity -= orderItem.Quantity;
                        inventoryItem.Product = null;
                        inventoryItem.Supplier = null;
                        inventoryService.UpdateInventory(inventoryItem);
                    }
                }

                if (!productExistsAndNotDeleted)
                {
                    total -= orderItem.PriceAtOrder;
                    lstOrder.RemoveAt(i);
                }
            }

            order.Status = "Đặt hàng thành công!";

            if (orderService.updateOrder(order))
            {
                NavigationService?.Navigate(new TransactionsPage());
            }
        }


        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            inventoryService = new InventoryService();
            productService = new ProductService();
            orderDetailService = new OrderDetailService();
            orderService = new OrderService();
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
            inventoryService = new InventoryService();
            productService = new ProductService();
            orderDetailService = new OrderDetailService();
            orderService = new OrderService();
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
            inventoryService = new InventoryService();
            productService = new ProductService();
            orderDetailService = new OrderDetailService();
            orderService = new OrderService();
            e.Handled = true;
            if (sender is System.Windows.Controls.Button button && button.DataContext is OrderDetail orderDetail)
            {
                if (orderDetail.Quantity < getQuantityProduct(orderDetail.ProductId, orderDetail.WarehouseId))
                {
                    orderDetail.Quantity++;
                    orderDetail.PriceAtOrder = orderDetail.Quantity * orderDetail.Product.Price;
                    orderDetailService.UpdateOrderDetail(orderDetail);
                    load();
                }
            }
        }

        private void OrdersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
