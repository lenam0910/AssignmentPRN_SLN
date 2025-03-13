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
    /// Interaction logic for DetailProductPage.xaml
    /// </summary>
    public partial class DetailProductPage : Page
    {
        private OrderService orderService;
        private OrderDetailService orderDetailService;
        private ProductService productService;
        private Product productRoot;
        private SupplierService supplierService;
        private InventoryService inventoryService;
        private OrderDetail orderDetail;
        private int selectedQuantity = 0;
        private DataAccess.Models.User user;

        public DetailProductPage(Product product)
        {
            user = System.Windows.Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            orderDetailService = new OrderDetailService();
            orderService = new OrderService();
            inventoryService = new InventoryService();
            supplierService = new SupplierService();
            productService = new ProductService();
            this.productRoot = product;
            InitializeComponent();
            selectedQuan.Text = selectedQuantity.ToString();
        }

        private void BuyNow_Click(object sender, RoutedEventArgs e)
        {
            int quantity = 1;
            int getQuantityInven = inventoryService.getTotalQuantityByProductId(productRoot.ProductId);




            Order order = orderService.GetOrderByUserId(user.UserId);
            if (order == null)
            {
                order = new Order { UserId = user.UserId, Status = "Chờ xử lý" };
                if (orderService.addOrder(order))
                {
                    MessageBox.Show("Tạo giỏ hàng mới thành công!");
                    order = orderService.GetOrderByUserId(user.UserId); // Lấy lại Order sau khi thêm
                }
            }

            OrderDetail orderDetail = orderDetailService.GetOrdersDetailByProductIdAndOrderID(productRoot.ProductId, order.OrderId);
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
                    ProductId = productRoot.ProductId,
                    WarehouseId = productRoot.CategoryId,
                    Quantity = quantity,
                    PriceAtOrder = quantity * productRoot.Price
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
                orderDetail.PriceAtOrder = orderDetail.Quantity * productRoot.Price;

                if (orderDetailService.UpdateOrderDetail(orderDetail))
                {
                    load();
                    MessageBox.Show("Sản phẩm đã được thêm vào giỏ hàng!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
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

             orderDetail = orderDetailService.GetOrdersDetailByProductIdAndOrderID(productRoot.ProductId, order.OrderId);
            if (orderDetail == null)
            {
                orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = productRoot.ProductId,
                    WarehouseId = productRoot.CategoryId,
                    Quantity = quantity,
                    PriceAtOrder = quantity * productRoot.Price
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
                orderDetail.PriceAtOrder = orderDetail.Quantity * productRoot.Price;

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
            var lstSup = supplierService.GetAllSuppliers();
            var lstInven = inventoryService.GetInventoryList();
            foreach (DataAccess.Models.Supplier item in lstSup)
            {
                if (item.SupplierId == productRoot.SupplierId)
                {
                    productRoot.Supplier = item;
                }
            }
            foreach (DataAccess.Models.Inventory item in lstInven)
            {
                if (item.ProductId == productRoot.ProductId)
                {
                    productRoot.QuantityInStock = item.Quantity;
                }
            }
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

             orderDetail = orderDetailService.GetOrdersDetailByProductIdAndOrderID(productRoot.ProductId, order.OrderId);
            if (orderDetail != null)
            {
                selectedQuantity = orderDetail.Quantity;
                selectedQuan.Text = selectedQuantity.ToString();
            }
            this.DataContext = productRoot;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load();
        }

        private void IncreaseQuantity(object sender, RoutedEventArgs e)
        {
            if (selectedQuantity < productRoot.QuantityInStock)
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
    }
}
