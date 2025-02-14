using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using TravelShare.Enums;
using TravelShare.Filters;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    [PaginaParaUsuarioLogado]
    public class SeguidorController : Controller
    {
        private readonly ISeguidorRepository _seguidorRepository;
        private readonly INotificacaoRepository _notificacaoRepository;
        private readonly ISessao _sessao;

        public SeguidorController(ISeguidorRepository seguidorRepository, ISessao sessao, INotificacaoRepository notificacaoRepository)
        {
            _seguidorRepository = seguidorRepository;
            _sessao = sessao;
            _notificacaoRepository = notificacaoRepository;
        }

        public class SeguirRequest
        {
            public string SeguindoId { get; set; }
            public string UsuarioId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SeguirOuDeseguir([FromBody] SeguirRequest request)
        {
            if (string.IsNullOrEmpty(request.SeguindoId) || string.IsNullOrEmpty(request.UsuarioId))
            {
                return Json(new { success = false, message = "Informações inválidas." });
            }

            var usuario = _sessao.BuscarSessaoDoUsuario();

            var usuarioSeguindo = await _seguidorRepository.BuscarSeguindoAsync(usuario.Id, request.SeguindoId);

            if (usuarioSeguindo == null)
            {
                await _seguidorRepository.SeguirUsuarioAsync(usuario.Id, request.SeguindoId);

                var notificacao = new NotificacaoModel
                {
                    UsuarioDestino = request.SeguindoId,
                    UsuarioOrigem = usuario.Id,
                    Notificacao = NotificacaoEnum.SeguiuVoce,
                    DataCriacao = DateTime.UtcNow,
                    ComentarioId = null,
                    PostId = null
                };

                await _notificacaoRepository.AddNotificacaoAsync(notificacao);

            }
            else
            {
                await _seguidorRepository.DeseguirUsuarioAsync(usuario.Id, request.SeguindoId);

                await _notificacaoRepository.RemoverNotificacaoAsync(usuario.Id, request.SeguindoId, NotificacaoEnum.SeguiuVoce, null);

            }

            return Json(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> RemoverSeguidor(string seguidorId)
        {
            if (string.IsNullOrEmpty(seguidorId))
            {
                TempData["Message"] = "Não foi possível executar essa ação.";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var usuario = _sessao.BuscarSessaoDoUsuario();

                var seguidor = await _seguidorRepository.BuscarSeguidorAsync(usuario.Id, seguidorId);

                if (seguidor != null)
                {
                    await _seguidorRepository.RemoverSeguidorAsync(usuario.Id, seguidorId);
                }
                else
                {
                    TempData["Message"] = "Não foi possível remover o seguidor.";
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}