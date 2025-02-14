using Microsoft.AspNetCore.Mvc;
using TravelShare.Enums;
using TravelShare.Filters;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    [PaginaParaUsuarioLogado]
    public class CurtidaController : Controller
    {
        private readonly ICurtidaRepository _curtidaRepository;
        private readonly IPostRepository _postRepository;
        private readonly INotificacaoRepository _notificacaoRepository;
        private readonly ISessao _sessao;

        public CurtidaController(ICurtidaRepository curtidaRepository, ISessao sessao, IPostRepository postRepository, INotificacaoRepository notificacaoRepository)
        {
            _curtidaRepository = curtidaRepository;
            _postRepository = postRepository;
            _notificacaoRepository = notificacaoRepository;
            _sessao = sessao;
        }

        public class CurtidaRequest
        {
            public string PostId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> CurtirOuDescurtir([FromBody] CurtidaRequest request)
        {
            var usuario = _sessao.BuscarSessaoDoUsuario();

            var post = await _postRepository.BuscarPostPorIdAsync(request.PostId);

            if (string.IsNullOrEmpty(request.PostId) || usuario == null)
            {
                return Json(new { success = false });
            }

            // Verifica se a curtida já existe
            var curtidaExistente = await _curtidaRepository.BuscarCurtidaExistenteAsync(request.PostId, usuario.Id);

            if (curtidaExistente)
            {
                // Se já existe, remove a curtida
                await _curtidaRepository.RemoveCurtidaAsync(request.PostId, usuario.Id);

                await _notificacaoRepository.RemoverNotificacaoAsync(post.UsuarioId, usuario.Id, NotificacaoEnum.CurtidaPost, post.Id);
            }
            else
            {
                // Se não existe, adiciona a curtida
                await _curtidaRepository.AddCurtidaAsync(request.PostId, usuario.Id);


                if(post != null && post.UsuarioId != usuario.Id)
                {
                    var notificacao = new NotificacaoModel
                    {
                        UsuarioDestino = post.UsuarioId,
                        UsuarioOrigem = usuario.Id,
                        Notificacao = NotificacaoEnum.CurtidaPost,
                        DataCriacao = DateTime.UtcNow,
                        PostId = post.Id,
                        ComentarioId = null
                    };


                    await _notificacaoRepository.AddNotificacaoAsync(notificacao);
                }

            }

            // Recupera o estado da curtida após a ação
            var novaCurtida = await _curtidaRepository.BuscarCurtidaExistenteAsync(request.PostId, usuario.Id);

            return Json(new
            {
                success = true,
                curtidaAtiva = novaCurtida  // Retorna true ou false dependendo se o usuário curtiu
            });
        }




    }
}