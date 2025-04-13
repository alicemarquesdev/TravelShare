using Microsoft.AspNetCore.Mvc;
using TravelShare.Enums;
using TravelShare.Filters;
using TravelShare.Helper.Interfaces;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    // Controller responsável por gerenciar as ações relacionadas ao seguimento de outros usuários.
    // Métodos:
    // 1. SeguirOuDeseguir: Permite que o usuário siga ou deixe de seguir outro usuário. 
    //    Além disso, dispara uma notificação quando um usuário começa a seguir outro.
    // 2. RemoverSeguidor: Permite que o usuário remova um seguidor de sua lista de seguidores, caso este exista.
    [PaginaParaUsuarioLogado] // - Acesso apenas para usuários logados
    public class SeguidorController : Controller
    {
        private readonly ISeguidorRepository _seguidorRepository;
        private readonly INotificacaoRepository _notificacaoRepository;
        private readonly ISessao _sessao;
        private readonly ILogger<SeguidorController> _logger;

        public SeguidorController(ISeguidorRepository seguidorRepository, ISessao sessao, INotificacaoRepository notificacaoRepository, ILogger<SeguidorController> logger)
        {
            _seguidorRepository = seguidorRepository ?? throw new ArgumentNullException(nameof(seguidorRepository));
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao));
            _notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger não pode ser nulo.");
        }

        // Classe interna para a requisição de seguir ou deixar de seguir um usuário
        public class SeguirRequest
        {
            public required string SeguindoId { get; set; }
            public required string UsuarioId { get; set; }
        }

        // Método para seguir ou deixar de seguir um usuário. A ação é baseada no estado atual de seguimento.
        [HttpPost]
        public async Task<IActionResult> SeguirOuDeseguir([FromBody] SeguirRequest request)
        {
            try
            {
                // Validação dos parâmetros da requisição
                if (string.IsNullOrEmpty(request.SeguindoId) || string.IsNullOrEmpty(request.UsuarioId))
                {
                    throw new Exception ("Informações inválidas.");
                }

                var usuario = _sessao.BuscarSessaoDoUsuario();

                // Verifica se o usuário já segue a pessoa
                var usuarioSeguindo = await _seguidorRepository.BuscarSeguindoAsync(usuario.Id, request.SeguindoId);

                // Se o usuário não segue, realiza o seguimento e cria uma notificação
                if (usuarioSeguindo == null)
                {
                    await _seguidorRepository.SeguirUsuarioAsync(usuario.Id, request.SeguindoId);

                    var notificacao = new NotificacaoModel
                    {
                        UsuarioDestinoId = request.SeguindoId,
                        UsuarioOrigemId = usuario.Id,
                        Notificacao = NotificacaoEnum.SeguiuVoce,
                        DataCriacao = DateTime.UtcNow,
                        ComentarioId = null,
                        PostId = null
                    };

                    // Cria a notificação para o usuário seguido
                    await _notificacaoRepository.AddNotificacaoAsync(notificacao);
                }
                else
                {
                    // Se o usuário já segue, realiza o deseguir e remove a notificação
                    await _seguidorRepository.DeseguirUsuarioAsync(usuario.Id, request.SeguindoId);
                    await _notificacaoRepository.RemoverNotificacaoAsync(usuario.Id, request.SeguindoId, NotificacaoEnum.SeguiuVoce, null);
                }

                // Retorna uma resposta JSON indicando que a operação foi bem-sucedida
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao seguir/deseguir.");
                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    return Json(new { success = false, message = ex.Message });  // Exibe a mensagem amigável
                }

                return Json(new { success = false, message = "Ocorreu um erro. Tente novamente." });
            }
        }

        // Método para remover um seguidor
        [HttpPost]
        public async Task<IActionResult> RemoverSeguidor(string seguidorId)
        {
            try
            {
                // Validação para garantir que o ID do seguidor foi fornecido
                if (string.IsNullOrEmpty(seguidorId))
                {
                    throw new Exception ("seguidorId é nulo");
                }

                var usuario = _sessao.BuscarSessaoDoUsuario();

                // Verifica se o seguidor existe
                var seguidor = await _seguidorRepository.BuscarSeguidorAsync(usuario.Id, seguidorId);

                if (seguidor == null)
                {

                    throw new Exception("Seguidor não encontrado.");

                }

                await _seguidorRepository.RemoverSeguidorAsync(usuario.Id, seguidorId);

                return Json(new { success = true });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover seguidor.");
                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    return Json(new { success = false, message = ex.Message });

                }

                return Json(new { success = false, message = "Ocorreu um erro ao remover o seguidor. Tente novamente mais tarde." });
            }
        }
    }
}