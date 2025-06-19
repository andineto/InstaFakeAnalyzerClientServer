using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InstaFakeAnalyzer.DTOs;
using Microsoft.Extensions.Configuration;

namespace InstaFakeAnalyzer.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<GeminiResponse> AnalisarNoticiaAsync(string prompt)
        {
            var apiKey = _configuration["GEMINI_APIKEY"];
            var baseUrl = _configuration["GEMINI_BASE_URL"];

#if DEBUG
            apiKey = _configuration["Gemini:ApiKey"];
            baseUrl = _configuration["Gemini:BaseUrl"];
#endif

            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("API Key do Gemini não configurada.");

            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var payload = new GeminiRequest
            {
                contents = new[]
                {
                    new GeminiContent
                    {
                        parts = new[]
                        {
                            new GeminiPart { text = prompt }
                        }
                    }
                }
            };

            var jsonRequest = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();

                var geminiResponseCandidate = JsonSerializer.Deserialize<GeminiResponseCandidate>(jsonResponse, options);

                var texto = geminiResponseCandidate?.candidates?[0]?.content?.parts?[0]?.text;

                if (string.IsNullOrWhiteSpace(texto))
                    throw new Exception("Resposta da IA vazia.");

                // Espera-se que o Gemini retorne um JSON no conteúdo
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(texto!, options);

                if (geminiResponse == null)
                    throw new Exception("Falha ao converter a resposta da IA.");

                return geminiResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar resposta da Gemini API: {ex.Message}");
                return new GeminiResponse { text = null, fake = true };
            }
        }
    }
}
