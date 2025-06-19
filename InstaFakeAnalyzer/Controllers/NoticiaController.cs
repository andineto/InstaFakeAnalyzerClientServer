using InstaFakeAnalyzer.Data;
using InstaFakeAnalyzer.Models;
using InstaFakeAnalyzer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace InstaFakeAnalyzer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoticiaController : ControllerBase
    {
        private readonly Service _service;


        public NoticiaController(Service noticiasService)
        {
            Console.WriteLine("Constructor chamado NoticiasController");
            _service = noticiasService;
        }

        

        [HttpPost]
        public async Task<IActionResult> PostNoticia([FromBody] Noticia noticia)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            Noticia noticiaVerificada = await _service.InserirNoticia(noticia);
            return Ok(noticiaVerificada);
        }

        [HttpGet]
        public async Task<IActionResult> GetNoticias()
        {
            var noticias = _service.ObterNoticiasTodas();
            return Ok(noticias);
        }

        [HttpGet ("{id}")]
        public async Task<IActionResult> GetNoticiaByMd5(string md5)
        {
            var noticia = await _service.ObterNoticiaPorMd5(md5);
            if (noticia == null)
            {
                return NotFound();
            }
            return Ok(noticia);
        }
        [HttpGet ("verificar")]
        public async Task<ActionResult> GetNoticiasNaoVerificadas()
        {
            var noticias = _service.ObterNoticiasNaoVerificadas();
            return Ok(noticias);
        }

        [HttpPost("direto")]
        public async Task<ActionResult> SalvarNoticiaDiretamente([FromBody] Noticia noticia)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _service.SalvarNoticiaDiretamente(noticia);
                return Ok();
            }
            catch(DbException ex)
            {
                return BadRequest(new { ex.Message });
            }


        }

        [HttpPut]
        public async Task<IActionResult> UpdateNoticia([FromBody] Noticia noticia)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _service.AlterarNoticia(noticia);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [HttpDelete ("{md5}")]
        public async Task<IActionResult> DeleteNoticia(string md5)
        {
            try
            {
                await _service.ExcluirNoticia(md5);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
    }
}
