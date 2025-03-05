using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WPF.User
{
    public class ChatBotAI
    {
        public ChatBotAI() { }
        private string apiKey = "AIzaSyAJbeqohHAZ9U7eOcf00T6k4GmDEr7j5wU";
        public async Task<string> SendRequestAndGetResponse(string userInput)
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
    }
}
