﻿using System;
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
            if (order == null)
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
            TotalAmountText.Text = "Tổng tiền: " + total + "đ";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load();
        }

        public int getQuantityProduct(int ProductId, int warehouseId)
        {
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
            foreach (var item in lstOrder)
            {
                foreach (var item2 in lstProduct)
                {
                    if (item.ProductId == item2.ProductId)
                    {
                        item2.Quantity -= item.Quantity;
                        inventoryService.UpdateInventory(item2);
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
