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

namespace WPF.User
{
    /// <summary>
    /// Interaction logic for ShoppingPage.xaml
    /// </summary>
    public partial class ShoppingPage : Page
    {
        private ChatBotAI chatBotAI;
        private ProductService productService;
        private StringBuilder chatHistory;
        private SupplierService supplierService;
        private InventoryService inventoryService;
        public ShoppingPage()
        {
            chatBotAI = new();
            supplierService = new SupplierService();
            inventoryService = new InventoryService();
            chatHistory = new StringBuilder();
            productService = new ProductService();
            InitializeComponent();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async Task helpBot(string userInput)
        {
            // Kiểm tra xem chatHistory có dòng nào chưa
            if (chatHistory.Length > 0)
            {
                chatHistory.AppendLine($"👤 Bạn: {userInput}"); // Chỉ thêm nếu không phải tin nhắn đầu tiên
            }

            string output = await chatBotAI.SendRequestAndGetResponse(userInput);

            // Xử lý xuống dòng
            output = output.Replace("\\n", Environment.NewLine)
                           .Replace("\n", Environment.NewLine)
                           .Replace("**", "");

            chatHistory.AppendLine($"\n🤖 Tư vấn viên: {output}"); // Thêm phản hồi AI vào lịch sử
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

            ChatInput.Clear(); // Xóa input sau khi gửi
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

        private void Page_Loaded_1(object sender, RoutedEventArgs e)
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
            lstProduct.ItemsSource = lstDisplay;
        }

        private async void lstProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product product = lstProduct.SelectedItem as Product;
            if (product != null)
            {
                string input = $"GIới thiệu ngắn gọn về ưu điểm và nhược điểm của sản phẩm" + product.ProductName + " này cho tôi, bạn với tư cách một người tư vấn sản phẩm";
                await helpBot(input);
                await Task.Delay(3000); 
                ChatGptPopup.Visibility = Visibility.Visible;
                OpenChatButton.Visibility = Visibility.Collapsed;
                ChatContent.Text = chatHistory.ToString(); // Cập nhật hiển thị chat



            }

        }
        }

    }


