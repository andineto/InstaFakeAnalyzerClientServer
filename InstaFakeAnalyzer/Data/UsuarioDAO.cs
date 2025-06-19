using InstaFakeAnalyzer.Models;
using MySqlConnector;

namespace InstaFakeAnalyzer.Data
{
    public partial class DAO
    {
        public static async Task InserirUsuario(Usuario usuario, MySqlTransaction tran)
        {
            string sql = @"INSERT INTO usuario 
                           (Id
                            , nome
                            , nomeusuario
                            , senha
                            , tipousuario) 
                           VALUES 
                           (
                            @id
                            , @nome
                            , @nomeusuario
                            , @senha
                            , @tipousuario)";
            using var command = new MySqlCommand(sql, tran.Connection, tran);
            {
                command.Parameters.AddWithValue("@id", usuario.Id);
                command.Parameters.AddWithValue("@nome", usuario.Nome ?? string.Empty);
                command.Parameters.AddWithValue("@nomeusuario", usuario.NomeUsuario ?? string.Empty);
                command.Parameters.AddWithValue("@senha", usuario.Senha ?? string.Empty);
                command.Parameters.AddWithValue("@tipousuario", (int)usuario.tipoUsuario);
                await command.ExecuteNonQueryAsync();
            }
        }

        public static async Task<Usuario> ObterUsuarioByNomeUsuario(string nomeUsuario, MySqlTransaction tran)
        {
            string sql = @"SELECT * 
                        FROM usuario
                        WHERE nomeusuario = @nomeusuario";
            using var command = new MySqlCommand(sql, tran.Connection, tran);
            {
                command.Parameters.AddWithValue("@nomeusuario", nomeUsuario ?? string.Empty);
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Usuario()
                    {
                        Id = reader.GetInt32("Id"),
                        Nome = reader.GetString("nome"),
                        NomeUsuario = reader.GetString("nomeusuario"),
                        Senha = reader.GetString("senha"),
                        tipoUsuario = (TipoUsuario)reader.GetInt32("tipousuario")
                    };
                }
                return null;
            }
        }


    }
}
