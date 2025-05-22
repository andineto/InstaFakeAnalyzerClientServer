using InstaFakeAnalyzer.Models;


namespace InstaFakeAnalyzer.Services
{
    public class NoticiasService
    {
        private readonly DeepSeekService _deepSeekService;

        public NoticiasService(DeepSeekService deepSeekService)
        {
            Console.WriteLine("Constructor chamado NoticiasService");
            _deepSeekService = deepSeekService;
        }

        public async Task<Noticia> ProcessaNoticia(Noticia noticia)
        {
            Noticia noticiaIa = await VerificarNoticia(noticia);

            return noticiaIa;
        }

        public async Task<Noticia> VerificarNoticia(Noticia noticia)
        {
            string prompt =
                "Você é um sistema que analisa se uma mensagem é uma fake news. Caso a mensagem contenha um link, analise o conteúdo do link.\n\n" +
                "Sua tarefa é responder **apenas** com um objeto JSON válido contendo **duas chaves**:\n" +
                "1. \"Text\" (string): Uma justificativa clara e objetiva em português.\n" +
                "2. \"Fake\" (boolean): true se for uma fake news, false caso contrário.\n\n" +
                "⚠️ **Instruções obrigatórias:**\n" +
                "- NÃO use \\boxed, markdown, ```json ou qualquer formatação especial.\n" +
                "- NÃO inclua comentários, explicações extras ou texto fora do JSON.\n" +
                "- Retorne apenas o JSON puro, como neste exemplo:\n" +
                "{ \"text\": \"Justificativa da análise em português.\", \"fake\": true }\n\n" +
                $"Noticia a ser analisada: {noticia.Conteudo}";

            var resultado = await _deepSeekService.AnalisarNoticiaAsync(prompt);
            return new Noticia
            {
                Conteudo = noticia.Conteudo,
                Justificativa = resultado.text,
                snFalsa = resultado.fake,
                snAnalizada = false,
                DataCadastro = DateTime.UtcNow
            };
        }
    }
}
