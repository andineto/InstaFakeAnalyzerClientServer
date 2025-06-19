using InstaFakeAnalyzer.Data;
using InstaFakeAnalyzer.Models;
using System.Security.Cryptography;


namespace InstaFakeAnalyzer.Services
{
    public partial class Service
    {
        public async Task<Noticia> ProcessaNoticia(Noticia noticia)
        {
            Noticia noticiaIa = await VerificarNoticia(noticia);

            return noticiaIa;
        }

        public async Task<Noticia> InserirNoticia(Noticia noticia)
        {
            if (noticia == null || string.IsNullOrWhiteSpace(noticia.Conteudo))
            {
                return null;
            }
            try
            {
                await BeginTransactionAsync();
                Noticia noticiaVerificada = await ObterNoticiaPorMd5(noticia.Md5);
                if (noticiaVerificada != null)
                {
                    // Se já existe, não insere novamente
                    return noticiaVerificada;
                }

                noticiaVerificada = await ProcessaNoticia(noticia);
                if (string.IsNullOrWhiteSpace(noticiaVerificada.Justificativa))
                {
                    noticiaVerificada.Justificativa = "Não obtivemos nenhuma resposta, tente novamente.";
                    return noticiaVerificada;
                }

                await DAO.InserirNoticia(noticiaVerificada, Transaction);
                await CommitAsync();
                return noticiaVerificada;
            }
            catch (Exception ex)
            {
                await RollbackAsync();
                throw new Exception("Erro ao inserir notícia.", ex);
            }
        }
        public async Task SalvarNoticiaDiretamente(Noticia noticia)
        {
            if (noticia == null || string.IsNullOrWhiteSpace(noticia.Conteudo))
            {
                return;
            }
            try
            {
                await BeginTransactionAsync();
                Noticia noticiaVerificada = await ObterNoticiaPorMd5(noticia.Md5);
                if (noticiaVerificada != null)
                {
                    // Se já existe, não insere novamente
                    return;
                }
                await DAO.InserirNoticia(noticia, Transaction);
                await CommitAsync();
            }
            catch (Exception ex)
            {
                await RollbackAsync();
                throw new Exception("Erro ao inserir notícia.", ex);
            }
        }
        


        public async Task<List<Noticia>> ObterNoticiasNaoVerificadas()
        {
            try
            {
                await BeginTransactionAsync();
                return await DAO.ObterNoticiasNaoVerificadas(Transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter notícias não verificadas.", ex);
            }
        }

        public async Task<List<Noticia>> ObterNoticiasTodas()
        {
            try
            {
                await BeginTransactionAsync();
                return await DAO.ObterNoticiasTodas(Transaction);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter notícias.", ex);
            }
        }

        public async Task<Noticia> ObterNoticiaPorMd5(string md5)
        {
            try
            {
                await BeginTransactionAsync();
                return await DAO.ObterNoticiaPorMd5(md5, Transaction);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter notícia com o md5 {md5}.", ex);
            }
        }

       public async Task AlterarNoticia(Noticia noticia)
        {
            if (noticia == null || string.IsNullOrWhiteSpace(noticia.Conteudo))
            {
                throw new ArgumentException("Notícia inválida ou sem conteúdo.");
            }
            try
            {
                await BeginTransactionAsync();
                if (ObterNoticiaPorMd5(noticia.Md5) == null)
                {
                    throw new Exception("Notícia não encontrada para alteração.");
                }
                await DAO.AlterarNoticia(noticia, Transaction);
                await CommitAsync();
            }
            catch (Exception ex)
            {
                await RollbackAsync();
                throw new Exception("Erro ao alterar notícia.", ex);
            }
        }

        public async Task ExcluirNoticia(string md5)
        {
            try
            {
                await BeginTransactionAsync();
                if (ObterNoticiaPorMd5(md5) == null)
                {
                    throw new Exception("Notícia não encontrada para excluir.");
                }
                await DAO.DeletarNoticia(md5, Transaction);
                await CommitAsync();
            }
            catch (Exception ex)
            {
                await RollbackAsync();
                throw new Exception("Erro ao excluir notícia.", ex);
            }
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

            var resultado = await _geminiService.AnalisarNoticiaAsync(prompt);
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
