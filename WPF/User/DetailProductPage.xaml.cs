using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for DetailProductPage.xaml
    /// </summary>
    public partial class DetailProductPage : Page
    {
        private OrderService orderService;
        private OrderDetailService orderDetailService;
        private ProductService productService;
        private Inventory inventoryRoot;
        private SupplierService supplierService;
        private InventoryService inventoryService;
        private OrderDetail orderDetail;
        private int selectedQuantity = 0;
        private DataAccess.Models.User user;

        public DetailProductPage(Inventory inventory)
        {
            user = System.Windows.Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
           
            this.inventoryRoot = inventory;
            InitializeComponent();
            selectedQuan.Text = selectedQuantity.ToString();
        }

        private void BuyNow_Click(object sender, RoutedEventArgs e)
        {
            orderDetailService = new OrderDetailService();
            orderService = new OrderService();
            inventoryService = new InventoryService();
            supplierService = new SupplierService();
            productService = new ProductService();
            int quantity = 1;
            int getQuantityInven = inventoryService.getTotalQuantityByProductId(inventoryRoot.ProductId);




            Order order = orderService.GetOrderByUserId(user.UserId);
            if (order == null)
            {
                order = new Order { UserId = user.UserId, Status = "Chờ xử lý" };
                if (orderService.addOrder(order))
                {
                    MessageBox.Show("Tạo giỏ hàng mới thành công!");
                    order = orderService.GetOrderByUserId(user.UserId); 
                }
            }

            OrderDetail orderDetail = orderDetailService.GetOrdersDetailByOrderidAndWarehouseID(inventoryRoot.Warehouse.WarehouseId, order.OrderId);

            if (orderDetail == null)
            {
                if (quantity > getQuantityInven)
                {
                    MessageBox.Show("Số lượng sản phẩm trong kho không đủ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = inventoryRoot.ProductId,
                    WarehouseId = inventoryRoot.WarehouseId,
                    Quantity = quantity,
                    PriceAtOrder = quantity * inventoryRoot.Product.Price
                };

                if (orderDetailService.AddOrderDetail(orderDetail))
                {
                    load();
                    MessageBox.Show("Sản phẩm đã được thêm vào giỏ hàng!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                if (quantity + orderDetail.Quantity > getQuantityInven)
                {
                    MessageBox.Show("Số lượng sản phẩm trong kho không đủ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                orderDetail.Quantity = quantity + orderDetail.Quantity;
                orderDetail.PriceAtOrder = orderDetail.Quantity * inventoryRoot.Product.Price;

                if (orderDetailService.UpdateOrderDetail(orderDetail))
                {
                    load();
                    MessageBox.Show("Sản phẩm đã được thêm vào giỏ hàng!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            orderDetailService = new OrderDetailService();
            orderService = new OrderService();
            inventoryService = new InventoryService();
            supplierService = new SupplierService();
            productService = new ProductService();
            int quantity = int.Parse(selectedQuan.Text);

            Order order = orderService.GetOrderByUserId(user.UserId);
            if (order == null)
            {
                order = new Order { UserId = user.UserId, Status = "Chờ xử lý" };
                if (orderService.addOrder(order))
                {
                    MessageBox.Show("Tạo giỏ hàng mới thành công!");
                    order = orderService.GetOrderByUserId(user.UserId); 
                }
            }

            orderDetail = orderDetailService.GetOrdersDetailByOrderidAndWarehouseID(inventoryRoot.Warehouse.WarehouseId, order.OrderId);
            if (orderDetail == null)
            {
                orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = inventoryRoot.ProductId,
                    WarehouseId = inventoryRoot.WarehouseId,
                    Quantity = quantity,
                    PriceAtOrder = quantity * inventoryRoot.Product.Price
                };

                if (orderDetailService.AddOrderDetail(orderDetail))
                {
                    load();
                    MessageBox.Show("Sản phẩm đã được thêm vào giỏ hàng!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                orderDetail.Quantity = quantity ;
                orderDetail.PriceAtOrder = orderDetail.Quantity * inventoryRoot.Product.Price;

                if (orderDetailService.UpdateOrderDetail(orderDetail))
                {
                    load();
                    MessageBox.Show("Sản phẩm đã được thêm vào giỏ hàng!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void load()
        {
            orderDetailService = new OrderDetailService();
            orderService = new OrderService();
            inventoryService = new InventoryService();
            supplierService = new SupplierService();
            productService = new ProductService();
            
            Order order = orderService.GetOrderByUserId(user.UserId);
            if (order == null)
            {
                order = new Order { UserId = user.UserId, Status = "Chờ xử lý" };
                if (orderService.addOrder(order))
                {
                    MessageBox.Show("Tạo giỏ hàng mới thành công!");
                    order = orderService.GetOrderByUserId(user.UserId);
                }
            }

             orderDetail = orderDetailService.GetOrdersDetailByOrderidAndWarehouseID(inventoryRoot.Warehouse.WarehouseId, order.OrderId);
            if (orderDetail != null)
            {
                selectedQuantity = orderDetail.Quantity;
                selectedQuan.Text = selectedQuantity.ToString();
            }
            var supplier = supplierService.GetAllSuppliers();
            foreach (var item in supplier)
            {
                if (item.SupplierId == inventoryRoot.Product.SupplierId)
                {
                    inventoryRoot.Product.Supplier = item;
                    break;
                }
            }   
            this.DataContext = inventoryRoot;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load();
        }

        private void IncreaseQuantity(object sender, RoutedEventArgs e)
        {
            if (selectedQuantity < inventoryRoot.Quantity)
            {
                selectedQuantity++;
                selectedQuan.Text = selectedQuantity.ToString();
                
            }
        }

        private void DecreaseQuantity(object sender, RoutedEventArgs e)
        {
            if(selectedQuantity > 0)
            {
                selectedQuantity--;
                selectedQuan.Text = selectedQuantity.ToString();
            }
        }

        private void OpenYouTube_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string testExePath = @"D:\FPTU\Kì5\PRN212\AssignmentPRN\AssignmentPRN_SLN\Tool\bin\Debug\Tool.exe";
                string url = $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(inventoryRoot.Product.ProductName)}";
                Process.Start(testExePath, $"\"{url}\""); // Truyền URL làm tham số
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

       
    }
}
