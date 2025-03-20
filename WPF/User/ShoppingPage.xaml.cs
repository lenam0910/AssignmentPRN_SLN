using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
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
using Microsoft.EntityFrameworkCore.Metadata;
using Service;
using static MaterialDesignThemes.Wpf.Theme;
using Newtonsoft.Json.Linq;
using DataAccess.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.ObjectModel;

namespace WPF.User
{
    /// <summary>
    /// Interaction logic for ShoppingPage.xaml
    /// </summary>
    public partial class ShoppingPage : System.Windows.Controls.Page
    {
        private ChatBotAI chatBotAI;
        private ProductService productService;
        private StringBuilder chatHistory;
        private SupplierService supplierService;
        private InventoryService inventoryService;
        private CategoryService categoryService;
        private DataAccess.Models.User user;
        private OrderService orderService;
        private OrderDetailService orderDetailService;
        private List<Product> productLst;
        private WarehousesService WarehousesService;
        public ShoppingPage()
        {
            WarehousesService = new WarehousesService();
            orderDetailService = new OrderDetailService();
            orderService = new OrderService();
            user = System.Windows.Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            categoryService = new CategoryService();
            chatBotAI = new();
            supplierService = new SupplierService();
            inventoryService = new InventoryService();
            chatHistory = new StringBuilder();
            productService = new ProductService();
            InitializeComponent();
        }



        private async Task helpBot(string userInput)
        {
            if (chatHistory.Length > 0)
            {
                chatHistory.AppendLine($"👤 Bạn: {userInput}"); // Chỉ thêm nếu không phải tin nhắn đầu tiên
            }

            string output = await chatBotAI.SendRequestAndGetResponse(userInput);

            // Xử lý xuống dòng
            output = output.Replace("\\n", Environment.NewLine)
                           .Replace("\n", Environment.NewLine)
                           .Replace("**", "");

            chatHistory.AppendLine($"\n🤖 Tư vấn viên: {output}");
        }


        private async Task sendBot(string userInput)
        {
            chatHistory.AppendLine($"👤 Bạn: {userInput}"); // Thêm tin nhắn của người dùng vào lịch sử

            string output = await chatBotAI.SendRequestAndGetResponse(userInput);

            // Xử lý xuống dòng
            output = output.Replace("\\n", Environment.NewLine)
                           .Replace("\n", Environment.NewLine)
                           .Replace("**", "");

            chatHistory.AppendLine($"\n🤖 Tư vấn viên: {output}"); // Thêm phản hồi AI vào lịch sử

        }
        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            string userInput = ChatInput.Text;
            if (string.IsNullOrWhiteSpace(userInput)) return;

            ChatInput.Clear();
            await sendBot(userInput);
            ChatContent.Text = chatHistory.ToString(); // Cập nhật hiển thị chat
        }

        private void OpenChatButton_Click(object sender, RoutedEventArgs e)
        {
            ChatGptPopup.Visibility = Visibility.Visible;
            OpenChatButton.Visibility = Visibility.Collapsed;
        }

        private void CloseChatPopup_Click(object sender, RoutedEventArgs e)
        {
            ChatGptPopup.Visibility = Visibility.Collapsed;
            OpenChatButton.Visibility = Visibility.Visible;

        }
        private void load()
        {
            var inventory = inventoryService.GetInventoryList();
            var products = productService.GetAllProducts();
            var lstDisplay = new List<Product>();
            if (products == null || !products.Any())
            {
                MessageBox.Show("Danh sách sản phẩm trống!");
            }
            foreach (Product product in products)
            {
                foreach (Inventory item in inventory)
                {

                    if (item.ProductId == product.ProductId)
                    {
                        lstDisplay.Add(product);
                    }
                }
            }
            CategoryFilter.ItemsSource = categoryService.getAll();
            CategoryFilter.DisplayMemberPath = "CategoryName";
            CategoryFilter.SelectedValuePath = "CategoryId";
            lstProduct.ItemsSource = lstDisplay;
         
            CategoryFilter.SelectedItem = null;
        }

        private void Page_Loaded_1(object sender, RoutedEventArgs e)
        {
            load();
        }

        private async void lstProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product product = lstProduct.SelectedItem as Product;
            if (product != null)
            {
                string input = $"GIới thiệu ngắn gọn về ưu điểm và nhược điểm của sản phẩm" + product.ProductName + " này cho tôi, bạn với tư cách một người tư vấn sản phẩm";
                await helpBot(input);
                ChatGptPopup.Visibility = Visibility.Visible;
                OpenChatButton.Visibility = Visibility.Collapsed;
                ChatContent.Text = chatHistory.ToString();

            }


        }



        public int getWarehouseByProductId(int ProductId)
        {
            var inventory = inventoryService.GetInventoryList();
            var products = productService.GetAllProducts();
            if (products == null || !products.Any())
            {
                MessageBox.Show("Danh sách sản phẩm trống!");
            }
            foreach (Product product in products)
            {
                foreach (Inventory item in inventory)
                {
                    if (item.ProductId == product.ProductId && product.ProductId == ProductId)
                    {
                        return item.WarehouseId;
                    }
                }
            }
            return 0;
        }

        private void BuyNowButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (sender is System.Windows.Controls.Button button && button.DataContext is Product selectedProduct)
            {
                int quantity = 1;
                int getQuantityInven = inventoryService.getTotalQuantityByProductId(selectedProduct.ProductId);


               

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

                OrderDetail orderDetail = orderDetailService.GetOrdersDetailByProductIdAndOrderID(selectedProduct.ProductId, order.OrderId);
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
                        ProductId = selectedProduct.ProductId,
                        WarehouseId = getWarehouseByProductId(selectedProduct.ProductId),
                        Quantity = quantity,
                        PriceAtOrder = quantity * selectedProduct.Price
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
                    orderDetail.PriceAtOrder = orderDetail.Quantity * selectedProduct.Price;

                    if (orderDetailService.UpdateOrderDetail(orderDetail))
                    {
                        load();
                        MessageBox.Show("Sản phẩm đã được thêm vào giỏ hàng!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }



        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
            {
                load();
                return;
            }
            var lstTemp = productService.GetAllProducts();
            var lst = inventoryService.GetInventoryList();
            productLst = new List<Product>();
            if (CategoryFilter.SelectedValue != null)
            {
                foreach (var product in lstTemp)
                {
                    foreach (var item in lst)
                    {
                        if (product.ProductId == item.ProductId && product.CategoryId == (int)CategoryFilter.SelectedValue && product.ProductName.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            productLst.Add(product);
                        }
                    }
                }
            }
            else
            {
                foreach (var product in lstTemp)
                {
                    foreach (var item in lst)
                    {
                        if (product.ProductId == item.ProductId && product.ProductName.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            productLst.Add(product);
                        }
                    }
                }
            }
        
            lstProduct.ItemsSource = null;
            lstProduct.ItemsSource = productLst;
            
        }

        private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryFilter.SelectedItem == null)
            {
                load();
                return;
            }
            productLst = new List<Product>();
            var lstTemp = productService.GetAllProducts();
            var lst = inventoryService.GetInventoryList();
            if (string.IsNullOrEmpty(SearchBox.Text))
            {
                foreach (var product in lstTemp)
                {
                    foreach (var item in lst)
                    {
                        if (product.ProductId == item.ProductId && product.CategoryId == (int)CategoryFilter.SelectedValue)
                        {
                            productLst.Add(product);
                        }
                    }
                }
            }
            else
            {
                foreach (var product in lstTemp)
                {
                    foreach (var item in lst)
                    {
                        if (product.ProductId == item.ProductId && product.CategoryId == (int)CategoryFilter.SelectedValue && product.ProductName.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            productLst.Add(product);
                        }
                    }
                }
               
            }
         
            lstProduct.ItemsSource = null;
            lstProduct.ItemsSource = productLst;
        }

        private void ToggleOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (sender is System.Windows.Controls.Button button && button.DataContext is Product selectedProduct)
            {
                NavigationService?.Navigate(new DetailProductPage(selectedProduct));

            }
        }
    }
}


