using InstaFakeAnalyzer.Models;
using MySqlConnector;

namespace InstaFakeAnalyzer.Data
{
    public partial class DAO
    {
        public static async Task InserirNoticia(Noticia noticia, MySqlTransaction tran)
        {
            string sql =
            @"
                INSERT INTO mensagem 
                (
                    fake
                    , conteudo
                    , verificado
                    , md5
                    , justificativa
                    , tipo
                )
                VALUES
                (
                    @snFalsa
                    , @conteudo
                    , @snAnalizada
                    , @md5
                    , @justificativa
                    , @tipo
                )";

            using var command = new MySqlCommand(sql, tran.Connection, tran);
            command.Parameters.AddWithValue("@snFalsa", noticia.snFalsa ? 1 : 0);
            command.Parameters.AddWithValue("@conteudo", noticia.Conteudo ?? string.Empty);
            command.Parameters.AddWithValue("@snAnalizada", noticia.snAnalizada ? 1 : 0);
            command.Parameters.AddWithValue("@md5", noticia.Md5 ?? string.Empty);
            command.Parameters.AddWithValue("@justificativa", noticia.Justificativa ?? string.Empty);
            command.Parameters.AddWithValue("@tipo", noticia.Tipo);
            await command.ExecuteNonQueryAsync();
        }

        public static async Task<List<Noticia>> ObterNoticiasTodas(MySqlTransaction tran)
        {
            List<Noticia> noticias = new List<Noticia>();
            string sql =
            @"
            SELECT
                fake
                , conteudo
                , verificado
                , justificativa
            FROM mensagem
            WHERE tipo = 1
            ";
            using var command = new MySqlCommand(sql, tran.Connection, tran);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var noticia = new Noticia()
                {
                    snFalsa = reader.GetInt32(0) == 1 ? true : false,
                    Conteudo = reader.GetString(1),
                    snAnalizada = reader.GetInt32(2) == 1 ? true : false,
                    Justificativa = reader.GetString(3)
                };
                noticias.Add(noticia);
            }
            return noticias;
        }
        public static async Task<Noticia> ObterNoticiaPorMd5(string md5, MySqlTransaction tran)
        {
            Noticia noticia = null;
            string sql =
            @"
            SELECT
                fake
                , conteudo
                , verificado
                , justificativa
            FROM mensagem
            WHERE tipo = 1
            AND md5 = @md5
        ";
            using var command = new MySqlCommand(sql, tran.Connection, tran);
            command.Parameters.AddWithValue("@md5", md5);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {

                noticia = new Noticia()
                {
                    snFalsa = reader.GetInt32(0) == 1 ? true : false,
                    Conteudo = reader.GetString(1),
                    snAnalizada = reader.GetInt32(2) == 1 ? true : false,
                    Justificativa = reader.GetString(3)
                };


            }
            return noticia;


        }
        public static async Task<List<Noticia>> ObterNoticiasNaoVerificadas(MySqlTransaction tran)
        {
            List<Noticia> noticias = new List<Noticia>();
            string sql =
            @"
            SELECT
                fake
                , conteudo
                , verificado
                , justificativa
            FROM mensagem
            WHERE tipo = 1
            AND verificado = 0
            ";
            using var command = new MySqlCommand(sql, tran.Connection, tran);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var noticia = new Noticia()
                {
                    snFalsa = reader.GetInt32(0) == 1 ? true : false,
                    Conteudo = reader.GetString(1),
                    snAnalizada = reader.GetInt32(2) == 1 ? true : false,
                    Justificativa = reader.GetString(3)
                };
                noticias.Add(noticia);
            }
            return noticias;
        }

        public static async Task AlterarNoticia(Noticia noticia, MySqlTransaction tran)
        {
            var sql = @"
                        UPDATE mensagem SET
                        fake = @fake
                       ,conteudo = @conteudo
                       ,verificado = @verificado
                       ,md5 = @md5
                       ,justificativa = @justificativa
                       ,tipo = @tipo
                        WHERE md5 = @md5
                        ";

            using var command = new MySqlCommand(sql, tran.Connection, tran);
            command.Parameters.AddWithValue("@fake", noticia.snFalsa ? 1 : 0);
            command.Parameters.AddWithValue("@conteudo", noticia.Conteudo ?? string.Empty);
            command.Parameters.AddWithValue("@verificado", noticia.snAnalizada ? 1 : 0);
            command.Parameters.AddWithValue("@md5", noticia.Md5 ?? string.Empty);
            command.Parameters.AddWithValue("@justificativa", noticia.Justificativa ?? string.Empty);
            command.Parameters.AddWithValue("@tipo", noticia.Tipo);
            await command.ExecuteNonQueryAsync();
        }
        
        public static async Task DeletarNoticia(string md5, MySqlTransaction tran)
        {
            if (string.IsNullOrEmpty(md5))
                throw new Exception("MD5 Vazio");

            var sql =
                @"
                    DELETE 
                    FROM mensagem
                    WHERE md5 = @md5
                ";

            using var command = new MySqlCommand(sql, tran.Connection, tran);
            command.Parameters.AddWithValue("@md5", md5);
            await command.ExecuteNonQueryAsync();
        }


    }

}   
