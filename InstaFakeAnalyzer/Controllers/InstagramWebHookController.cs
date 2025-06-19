using InstaFakeAnalyzer.Data;
using InstaFakeAnalyzer.DTOs;
using InstaFakeAnalyzer.Models;
using InstaFakeAnalyzer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/webhook/instagram")]
public class InstagramWebhookController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _client;
    private const string VERIFY_TOKEN = "FakeNewsAnalyzer"; 

    public InstagramWebhookController(IConfiguration configuration, HttpClient client)
    {
        _client = client;
        _configuration = configuration;
        string apiUrl = _configuration["ApiUrl"];
#if DEBUG
        apiUrl = _configuration["Urls:ApiUrl"];
#endif
        _client.BaseAddress = new Uri(apiUrl);

    }

    // Etapa de verificação do Meta
    [HttpGet]
    public IActionResult Get([FromQuery(Name = "hub.mode")] string mode,
                             [FromQuery(Name = "hub.verify_token")] string token,
                             [FromQuery(Name = "hub.challenge")] string challenge)
    {
        if (mode == "subscribe" && token == VERIFY_TOKEN)
        {
            return Ok(challenge); 
        }
        return Unauthorized();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] InstagramWebhookDTOs body)
    {
        Console.WriteLine("Recebido webhook: " + JsonSerializer.Serialize(body));

        try
        {
            foreach (var entry in body.Entry)
            {
                foreach (var messaging in entry.Messaging)
                {
                    if (entry.Id == messaging.Sender.Id)
                    {
                        Console.WriteLine($"Reprocessamento de mensagem auto-recebida...Ignorar");
                        continue;
                    }

                    var senderId = messaging.Sender.Id;
                    var messageText = messaging.Message?.Text;

                    if (!string.IsNullOrEmpty(messageText))
                    {
                        Console.WriteLine($"Mensagem recebida de {senderId}: {messageText}");

                        var noticia = new Noticia()
                        {
                            Conteudo = messageText,
                        };

                        await EnviarRespostaInstagramAsync(senderId, "Sua notícia foi recebida e está sendo analisada...");

                        string url = "noticia";

                        var response = await _client.PostAsJsonAsync(url, noticia);

                        var responseBody = await response.Content.ReadAsStringAsync();
                        
                        noticia = JsonSerializer.Deserialize<Noticia>(
                            responseBody,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );

                        await EnviarRespostaInstagramAsync(senderId, noticia.Justificativa);

                        Console.WriteLine(noticia.Justificativa);
                        return Ok();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar a mensagem: {ex.Message}");
            return BadRequest(new { erro = ex.Message });
        }

        return Ok();
    }

public async Task EnviarRespostaInstagramAsync(string senderId, string resposta)
    {
        string pageAccessToken = _configuration["ACCESS_TOKEN_FB"];
        using var client = new HttpClient();

        var jsonBody = new
        {
            recipient = new { id = senderId },
            message = new { text = resposta }
        };

        var content = new StringContent(JsonSerializer.Serialize(jsonBody), Encoding.UTF8, "application/json");

        var url = $"https://graph.facebook.com/v18.0/me/messages?access_token={pageAccessToken}";
        var response = await client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Erro ao enviar mensagem: " + erro);
        }
    }
}
