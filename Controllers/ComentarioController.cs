using Microsoft.AspNetCore.Mvc;
using System.Text;
using TravelShare.Enums;
using TravelShare.Filters;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    [PaginaParaUsuarioLogado]
    public class ComentarioController : Controller
    {
        private readonly IComentarioRepository _comentarioRepository;
        private readonly ISessao _sessao;
        private readonly IPostRepository _postRepository;
        private readonly INotificacaoRepository _notificacaoRepository;

        public ComentarioController(IComentarioRepository comentarioRepository, ISessao sessao, IPostRepository postRepository, INotificacaoRepository notificacaoRepository)
        {
            _comentarioRepository = comentarioRepository;
            _postRepository = postRepository;
            _notificacaoRepository = notificacaoRepository;
            _sessao = sessao;
        }

        public class ComentarioRequest
        {
            public string Id { get; set; } 
            public string PostId { get; set; }
            public string Comentario { get; set; }
        }

        private string RenderizarComentariosHTML(List<ComentarioModel> comentarios)
        {
            var sb = new StringBuilder();
            foreach (var comentario in comentarios)
            {
                sb.AppendLine($"<div class='comentario'><strong>{comentario.UsuarioUsername}</strong> {comentario.Comentario}");
            }
            return sb.ToString();
        }


        [HttpPost]
        public async Task<IActionResult> AddComentario([FromBody] ComentarioRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PostId))
            {
                return Json(new
                {
                    success = false,
                    message = "Erro ao adicionar comentário."
                });
            }


            if (request.Comentario.Length > 100)
            {
                return Json(new
                {
                    success = false,
                    message = "O comentário deve ter no máximo 100 caracteres."
                });
                    
            }

            var usuario = _sessao.BuscarSessaoDoUsuario();

            if (usuario != null)
            {
                var comentarioModel = new ComentarioModel
                {
                    Comentario = request.Comentario,
                    PostId = request.PostId,
                    UsuarioId = usuario.Id,
                    UsuarioUsername = usuario.Username
                };

                await _comentarioRepository.AddComentarioAsync(comentarioModel);

                var post = await _postRepository.BuscarPostPorIdAsync(request.PostId);

                if(post != null && post.UsuarioId != usuario.Id)
                {
                    var notificacao = new NotificacaoModel
                    {
                        UsuarioDestino = post.UsuarioId,
                        UsuarioOrigem = usuario.Id,
                        Notificacao = NotificacaoEnum.ComentarioPost,
                        DataCriacao = DateTime.UtcNow,
                        ComentarioId = comentarioModel.Id,
                        PostId = post.Id

                    };

                    await _notificacaoRepository.AddNotificacaoAsync(notificacao);
                }

                // Retorne um JSON com sucesso e os dados atualizados
                var comentarios = await _comentarioRepository.BuscarTodosOsComentariosDoPostAsync(request.PostId);
                var comentariosHTML = RenderizarComentariosHTML(comentarios);  // Método para gerar o HTML dos comentários
                return Json(new { success = true, comentariosHTML = comentariosHTML });

            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Usuário não encontrado."
                });

            }
                   }

        [HttpPost]
        public async Task<IActionResult> DeletarComentario([FromBody] ComentarioRequest request)
        {
          
            if (!String.IsNullOrEmpty(request.Id))
            {
                bool sucesso = await _comentarioRepository.DeletarComentarioAsync(request.Id);

                if (sucesso)
                {
                    await _notificacaoRepository.RemoverNotificacaoPorComentarioAsync(request.Id);
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, message = "Não foi possível deletar o comentário." });
        }
    }
}