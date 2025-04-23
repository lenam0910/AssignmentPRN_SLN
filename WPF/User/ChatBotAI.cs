using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;

using Service;

namespace WPF.User
{
    public class ChatBotAI
    {
        private readonly StringBuilder chatHistory; // Lưu lịch sử hội thoại
        private readonly string apiKey = "";
        private APIkeyService apiKeyService;

        public ChatBotAI()
        {
            apiKeyService = new APIkeyService();
            chatHistory = new StringBuilder();
                apiKey = apiKeyService?.GetApiNewest()?.ApiKey1;
            
        }
        public async Task<bool> IsApiKeyValid(string apiKey1)
        {
            string jsonBody = new JObject
            {
                ["contents"] = new JArray
        {
            new JObject
            {
                ["role"] = "user",
                ["parts"] = new JArray
                {
                    new JObject
                    {
                        ["text"] = "hi"
                    }
                }
            }
        }
            }.ToString();


            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey1}");
            request.Content = new StringContent(jsonBody, Encoding.UTF8);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return true;  // API key hợp lệ
            }
            else
            {

                return false;  
            }
        }


        public async Task<string> SendRequestAndGetResponse(string userInput)
        {
            chatHistory.AppendLine($"👤 Bạn: {userInput}");

            string jsonBody = new JObject
            {
                ["contents"] = new JArray
        {
            new JObject
            {
                ["role"] = "user",
                ["parts"] = JArray.Parse(FormatChatHistory()) // Gửi cả lịch sử cuộc hội thoại
            }
        }
            }.ToString();

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
                    var outputText = json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "Không nhận được phản hồi từ AI.";

                    // Thêm phản hồi của AI vào lịch sử chat
                    chatHistory.AppendLine($"🤖 Tư vấn viên: {outputText}");

                    return outputText;
                }
                catch (Exception ex)
                {
                    return $"Lỗi xử lý JSON: {ex.Message}";
                }
            }
            else
            {
                return $"Chat Bot chưa được kích hoạt, vui lòng đợi để Admin kích hoạt!";
            }
        }


        private string FormatChatHistory()
        {
            var chatLines = chatHistory.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var formattedParts = new JArray();

            foreach (var line in chatLines)
            {
                formattedParts.Add(new JObject { ["text"] = line });
            }

            return formattedParts.ToString();
        }


        public void ClearChatHistory()
        {
            chatHistory.Clear();
        }
    }
}
