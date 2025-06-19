using InstaFakeAnalyzer.Data;
using InstaFakeAnalyzer.Models;
using InstaFakeAnalyzer.Services;
using InstaFakeAnalyzer.Utils;
using InstaFakeAnalyzerClient.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstaFakeAnalyzer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly Service _service;
        public UsuarioController(Service service)
        {
            _service = service;
        }

        [HttpPost($"cadastrar")]
        public async Task<IActionResult> CadastrarUsuario([FromBody] Usuario usuario)
        {
            try
            {
                usuario.Senha = MD5.Md5Hash(usuario.Senha); // Hash da senha
                usuario.DataCadastro = DateTime.UtcNow;
                await _service.InserirUsuario(usuario);
                return Ok(); // Cadastro bem-sucedido
            }catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }

        }

        [HttpPost($"login")]
        public async Task<IActionResult> FazerLogin([FromBody]Usuario user)
        {
            try
            {
                var usuario = await _service.FazerLogin(user);
                return Ok(usuario);
            }catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [HttpPost($"logout")]
        public IActionResult FazerLogout()
        {
            Sessao.Encerrar();
            return Ok();
        }   

        [HttpGet]
        public async Task<IActionResult> ObterUsuarioLogado()
        {
            var usuario = Sessao.ObterUsuarioLogado();

            if (usuario != null)
            {
                return Ok(usuario); 
            }
            return BadRequest(new
            {
                resultado = "Nenhum Usuário logado"
            }); 
        }


    }
}
