using InstaFakeAnalyzer.Data;
using InstaFakeAnalyzer.Models;
using InstaFakeAnalyzer.Utils;
using InstaFakeAnalyzerClient.Utils;

namespace InstaFakeAnalyzer.Services
{
    public partial class Service
    {
        public async Task InserirUsuario(Usuario usuario)
        {
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.NomeUsuario))
            {
                throw new ArgumentException("Usuário inválido ou sem nome de usuário.");
            }
            try
            {
                await BeginTransactionAsync();
                Usuario usuarioCadastrado = await ObterUsuarioByNomeUsuario(usuario.NomeUsuario);
                if (usuarioCadastrado != null)
                    return;

                await DAO.InserirUsuario(usuario, Transaction!);
                await CommitAsync();
            }
            catch (Exception ex)
            {
                await RollbackAsync();
                throw new Exception("Erro ao inserir usuário.", ex);
            }
        }

        public async Task<Usuario> ObterUsuarioByNomeUsuario(string nomeUsuario)
        {
            if (string.IsNullOrWhiteSpace(nomeUsuario))
            {
                throw new ArgumentException("Nome de usuário inválido.");
            }
            try
            {
                return await DAO.ObterUsuarioByNomeUsuario(nomeUsuario, Transaction!);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter usuário por nome de usuário: {nomeUsuario}", ex);
            }
        }


        internal async Task<Usuario> FazerLogin(Usuario user)
        {
            var usuarioCadastrado = await ObterUsuarioByNomeUsuario(user.NomeUsuario);
            var senhaMd5 = MD5.Md5Hash(user.Senha);
            if (usuarioCadastrado == null || usuarioCadastrado.Senha != senhaMd5)
            {
                throw new Exception("Nome de usuario ou senha inválidos");
            }
            Sessao.Iniciar(user);
            return usuarioCadastrado;
        }
    }
}
