using Microsoft.AspNetCore.Mvc;
using TravelShare.Enums;
using TravelShare.Filters;
using TravelShare.Helper.Interfaces;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    // Controller responsável por gerenciar as curtidas dos usuários em posts.
    // Métodos:
    // 1. CurtirOuDescurtir: Adiciona ou remove uma curtida de um post. 
    //    Se o usuário já curtiu o post, ele remove a curtida e a notificação correspondente. Caso contrário, ele adiciona a curtida e cria uma notificação para o dono do post.
    [PaginaParaUsuarioLogado] // - Acesso apenas para usuários logados
    public class CurtidaController : Controller
    {
        private readonly ICurtidaRepository _curtidaRepository;
        private readonly IPostRepository _postRepository;
        private readonly INotificacaoRepository _notificacaoRepository;
        private readonly ISessao _sessao;
        private readonly ILogger<CurtidaController> _logger; // Injeção de dependência para o ILogger

        // Construtor que recebe as dependências necessárias para o controlador.
        // Caso alguma dependência seja nula, será lançado um throw para evitar erros durante a execução.
        public CurtidaController(ICurtidaRepository curtidaRepository, ISessao sessao, IPostRepository postRepository, INotificacaoRepository notificacaoRepository, ILogger<CurtidaController> logger)
        {
            _curtidaRepository = curtidaRepository ?? throw new ArgumentNullException(nameof(curtidaRepository), "O repositório de curtidas não pode ser nulo.");
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao), "A sessão não pode ser nula.");
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository), "O repositório de posts não pode ser nulo.");
            _notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository), "O repositório de notificações não pode ser nulo.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "O logger não pode ser nulo.");
        }

        // Classe para mapear os dados da requisição de curtida
        public class CurtidaRequest
        {
            public required string PostId { get; set; }  // ID do post para curtir ou descurtir
        }

        // Método responsável por curtir ou descurtir um post. 
        // Se o usuário já curtiu, ele desfaz a curtida e a notificação. Caso contrário, ele adiciona a curtida e cria uma notificação.
        [HttpPost]
        public async Task<IActionResult> CurtirOuDescurtir([FromBody] CurtidaRequest request)
        {
            try
            {
                var usuario = _sessao.BuscarSessaoDoUsuario();

                if (string.IsNullOrEmpty(request.PostId) || usuario == null)
                {
                    _logger.LogWarning("Tentativa de curtir/descurtir sem um usuário ou post válido.");
                    return Json(new { success = false });
                }

                var post = await _postRepository.BuscarPostPorIdAsync(request.PostId);

                if (post == null)
                {
                    _logger.LogWarning($"Post não encontrado para o ID: {request.PostId}");
                    return Json(new { success = false });
                }

                // Verifica se a curtida já existe
                var curtidaExistente = await _curtidaRepository.BuscarCurtidaExistenteAsync(request.PostId, usuario.Id);

                if (curtidaExistente)
                {
                    // Se já existe, remove a curtida
                    await _curtidaRepository.RemoveCurtidaAsync(request.PostId, usuario.Id);

                    // Remove a notificação de curtida
                    _logger.LogInformation($"Removendo a curtida do usuário {usuario.Username} no post {request.PostId}");
                    await _notificacaoRepository.RemoverNotificacaoAsync(post.UsuarioId, usuario.Id, NotificacaoEnum.CurtidaPost, post.Id);
                }
                else
                {
                    // Se não existe, adiciona a curtida
                    await _curtidaRepository.AddCurtidaAsync(request.PostId, usuario.Id);

                    // Cria a notificação para o proprietário do post
                    if (post.UsuarioId != usuario.Id)
                    {
                        var notificacao = new NotificacaoModel
                        {
                            UsuarioDestinoId = post.UsuarioId,
                            UsuarioOrigemId = usuario.Id,
                            Notificacao = NotificacaoEnum.CurtidaPost,
                            DataCriacao = DateTime.UtcNow,
                            PostId = post.Id,
                            ComentarioId = null
                        };

                        _logger.LogInformation($"Adicionando notificação de curtida para o post {request.PostId} do usuário {usuario.Username}.");
                        await _notificacaoRepository.AddNotificacaoAsync(notificacao);
                    }
                }

                // Recupera o estado da curtida após a ação
                var novaCurtida = await _curtidaRepository.BuscarCurtidaExistenteAsync(request.PostId, usuario.Id);

                // Retorna o estado da curtida (se o usuário está curtindo ou não)
                return Json(new
                {
                    success = true,
                    curtidaAtiva = novaCurtida  // Retorna true ou false dependendo se o usuário curtiu ou não
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar curtir ou descurtir o post.");
                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["Message"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["Message"] = "Ocorreu um erro ao tentar curtir ou descurtir o post. Tente novamente mais tarde.";
                }

                return Json(new { success = false, message = "Ocorreu um erro ao tentar curtir ou descurtir o post. Tente novamente mais tarde." });
            }
         
        }
    }
}
