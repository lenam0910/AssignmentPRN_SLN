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

namespace WPF.User
{
    /// <summary>
    /// Interaction logic for ShoppingPage.xaml
    /// </summary>
    public partial class ShoppingPage : Page
    {
        private string apiKey = "AIzaSyAJbeqohHAZ9U7eOcf00T6k4GmDEr7j5wU";
        private ProductService productService;
        private StringBuilder chatHistory;
        public ShoppingPage()
        {
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lstProduct.ItemsSource = productService.GetAllProducts();
        }


        private async Task<string> SendRequestAndGetResponse(string userInput)
        {
            string jsonBody = $@"{{
                ""contents"": [
                    {{
                        ""role"": ""user"",
                        ""parts"": [
                            {{
                                ""text"": ""{userInput}""
                            }}
                        ]
                    }}
                ]
            }}";

            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}");
            request.Content = new StringContent(jsonBody, Encoding.UTF8);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request).ConfigureAwait(false);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var json = JObject.Parse(responseBody);
                    var outputText = json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                    return outputText ?? "Không nhận được phản hồi từ AI.";
                }
                catch (Exception ex)
                {
                    return $"Lỗi xử lý JSON: {ex.Message}";
                }
            }
            else
            {
                return $"Lỗi API: {response.StatusCode} - {response.ReasonPhrase}\nChi tiết: {responseBody}";
            }
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            string userInput = ChatInput.Text;
            if (string.IsNullOrWhiteSpace(userInput)) return;

            ChatInput.Clear(); // Xóa input sau khi gửi
            chatHistory.AppendLine($"👤 Bạn: {userInput}"); // Thêm tin nhắn của người dùng vào lịch sử

            string output = await SendRequestAndGetResponse(userInput);

            // Xử lý xuống dòng
            output = output.Replace("\\n", Environment.NewLine)
                           .Replace("\n", Environment.NewLine)
                           .Replace("**", "");

            chatHistory.AppendLine($"🤖 GPT: {output}"); // Thêm phản hồi AI vào lịch sử
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
    }
}

