using Microsoft.AspNetCore.Mvc;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    public class ComentarioController : Controller
    {
        private readonly IComentarioRepository _comentarioRepository;
        private readonly ISessao _sessao;

        public ComentarioController(IComentarioRepository comentarioRepository, ISessao sessao)
        {
            _comentarioRepository = comentarioRepository;
            _sessao = sessao;
        }

        [HttpPost]
        public async Task<IActionResult> AddComentario(string PostId, string UsuarioId, string Comentario)
        {
            // Não usar model state pq não recebe model, mas é importante para a validação. (se html.for não fizer o papel.
            if (ModelState.IsValid)
            {
                var usuario = _sessao.BuscarSessaoDoUsuario();

                if (usuario != null)
                {
                    var comentario = new ComentarioModel
                    {
                        PostId = PostId,
                        UsuarioId = usuario.Id,
                        UsuarioUsername = usuario.Username,
                        Comentario = Comentario,
                    };

                    await _comentarioRepository.AddComentario(comentario);
                    return Redirect(Request.Headers["Referer"].ToString());
                }
            }
            TempData["Message"] = "Não foi possível adicionar o comentário.";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> DeletarComentario(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                await _comentarioRepository.DeletarComentario(id);
                return Redirect(Request.Headers["Referer"].ToString());
            }

            TempData["Message"] = "Não foi possível deletar o comentário.";
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}