using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FishStore.Service
{
    internal class OpenAIService
    {
        private const string apiKey = "69ec13328fc348ee9cd56f35e706676e"; // ❗️Thay bằng API key thật của bạn
        private const string apiUrl = "https://api.aimlapi.com/v1/chat/completions";

        public async Task<string> GetInventoryOptimizationReportAsync(string inventoryData)
        {
            var client = new RestClient(apiUrl);
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                model = "google/gemma-3-12b-it",
                messages = new object[]
                {
                new { role = "system", content = "Bạn là chuyên gia tối ưu kho hàng trong lĩnh vực cá cảnh." },
                new { role = "user", content = $"Dưới đây là dữ liệu kho hiện tại:\n{inventoryData}\nHãy phân tích và gợi ý nhập hàng nếu cần." }
                },
                temperature = 0.7,
                max_tokens = 256
            };

            request.AddJsonBody(body);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var json = JObject.Parse(response.Content);
                return json["choices"]?[0]?["message"]?["content"]?.ToString();
            }
            else
            {
                Console.WriteLine("API Response: " + response.Content);
                return "Lỗi khi gọi API: " + response.ErrorMessage;
                
            }
        }
    }
}
