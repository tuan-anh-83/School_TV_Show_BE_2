using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Services.CloudFlareService
{
    public class CloudService : ICloudService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accountId;
        private readonly string _apiToken;

        public CloudService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _accountId = configuration["Cloudflare:AccountId"];
            _apiToken = configuration["Cloudflare:ApiToken"];
        }

        public async Task<string> UploadVideoAsync(IFormFile file)
        {
            // Kiểm tra file có hợp lệ không
            if (file == null || file.Length == 0)
                throw new Exception("File is empty.");

            // Thiết lập headers
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // API URL để upload trực tiếp video
            string apiUrl = $"https://api.cloudflare.com/client/v4/accounts/{_accountId}/stream";

            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            // Thêm file vào form-data
            content.Add(fileContent, "file", file.FileName);

            // Gửi request upload trực tiếp lên Cloudflare
            var response = await _httpClient.PostAsync(apiUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Upload error: {responseBody}");
                throw new Exception($"Upload error: {responseBody}");
            }

            // Lấy video ID từ JSON phản hồi
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var videoId = jsonDoc.RootElement.GetProperty("result").GetProperty("uid").GetString();

            // Trả về URL xem video
            return $"https://watch.cloudflarestream.com/{videoId}";
        }
    }
}
