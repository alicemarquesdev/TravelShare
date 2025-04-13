using Microsoft.AspNetCore.Mvc;
using System.Text;
using TravelShare.Enums;
using TravelShare.Filters;
using TravelShare.Helper.Interfaces;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    // Controller responsável pelo gerenciamento de comentários em posts.
    // Métodos:
    // 1. AddComentario: Adiciona um novo comentário a um post. Notifica o proprietário do post se o comentário não for do próprio usuário.
    // 2. DeletarComentario: Remove um comentário existente de um post e também remove as notificações associadas.
    [PaginaParaUsuarioLogado] // - Acesso apenas para usuários logados
    public class ComentarioController : Controller
    {
        private readonly IComentarioRepository _comentarioRepository;
        private readonly ISessao _sessao;
        private readonly IPostRepository _postRepository;
        private readonly INotificacaoRepository _notificacaoRepository;
        private readonly ILogger<ComentarioController> _logger;

        // Construtor que recebe as dependências necessárias para o controlador.
        // Caso alguma dependência seja nula, será lançado um throw para evitar erros durante a execução.
        public ComentarioController(IComentarioRepository comentarioRepository,
                                    ISessao sessao,
                                    IPostRepository postRepository,
                                    INotificacaoRepository notificacaoRepository,
                                    ILogger<ComentarioController> logger)
        {
            _comentarioRepository = comentarioRepository ?? throw new ArgumentNullException(nameof(comentarioRepository), "O repositório de comentários não pode ser nulo.");
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao), "A sessão não pode ser nula.");
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository), "O repositório de posts não pode ser nulo.");
            _notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository), "O repositório de notificações não pode ser nulo.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "O logger não pode ser nulo.");
        }

        public class ComentarioRequest
        {
            public string Id { get; set; }
            public string PostId { get; set; }
            public string Comentario { get; set; }
        }

        // Método para renderizar os comentários em formato HTML para o frontend
        private string RenderizarComentariosHTML(List<ComentarioModel> comentarios)
        {
            var sb = new StringBuilder();
            foreach (var comentario in comentarios)
            {
                sb.AppendLine($"<div class='comentario'><strong>{comentario.UsuarioUsername}</strong> {comentario.Comentario}</div>");
            }
            return sb.ToString();
        }


        // Método responsável por adicionar um comentário a um post.
        // Verifica se o post existe e se o comentário é válido.
        [HttpPost]
        public async Task<IActionResult> AddComentario([FromBody] ComentarioRequest request)
        {
            try
            {
                // Verifica se o ID do post está presente
                if (string.IsNullOrWhiteSpace(request.PostId))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Erro ao adicionar comentário."
                    });
                }

                // Verifica o tamanho do comentário
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

                    // Adiciona o comentário ao repositório
                    await _comentarioRepository.AddComentarioAsync(comentarioModel);

                    var post = await _postRepository.BuscarPostPorIdAsync(request.PostId);

                    // Notifica o dono do post, caso o comentário não seja do próprio dono
                    if (post != null && post.UsuarioId != usuario.Id)
                    {
                        var notificacao = new NotificacaoModel
                        {
                            UsuarioDestinoId = post.UsuarioId,
                            UsuarioOrigemId = usuario.Id,
                            Notificacao = NotificacaoEnum.ComentarioPost,
                            DataCriacao = DateTime.UtcNow,
                            ComentarioId = comentarioModel.Id,
                            PostId = post.Id
                        };

                        // Adiciona a notificação ao repositório
                        await _notificacaoRepository.AddNotificacaoAsync(notificacao);
                    }

                    // Retorna os comentários renderizados em HTML
                    var comentarios = await _comentarioRepository.BuscarTodosOsComentariosDoPostAsync(request.PostId);
                    var comentariosHTML = RenderizarComentariosHTML(comentarios); // Método para gerar o HTML dos comentários
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
            catch (Exception ex)
            {
                // Loga o erro e retorna uma mensagem amigável
                _logger.LogError(ex, "Erro ao adicionar comentário.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Método responsável por deletar um comentário de um post.
        [HttpPost]
        public async Task<IActionResult> DeletarComentario([FromBody] ComentarioRequest request)
        {
            try
            {
                // Verifica se o ID do comentário está presente
                if (!String.IsNullOrEmpty(request.Id))
                {
                    var comentarioId = request.Id;
                    // Deleta o comentário no repositório
                    bool sucesso = await _comentarioRepository.DeletarComentarioAsync(request.Id);

                    // Caso o comentário seja deletado com sucesso, também remove a notificação
                    if (sucesso)
                    {
                        // Primeiro, tenta encontrar a notificação associada ao comentário
                        var notificacao = await _notificacaoRepository.BuscarNotificacaoPorComentarioId(comentarioId);

                        // Se a notificação existir, remove-a
                        if (notificacao != null)
                        {
                            await _notificacaoRepository.RemoverNotificacaoPorComentarioAsync(comentarioId);
                        }

                        return Json(new { success = true });
                    }
                }
                return Json(new { success = false, message = "Não foi possível deletar o comentário." });
            }
            catch (Exception ex)
            {
                // Loga o erro e retorna uma mensagem amigável
                _logger.LogError(ex, "Erro ao deletar comentário.");
                return Json(new { success = false, message = "Ocorreu um erro ao deletar o comentário. Tente novamente mais tarde." });
            }
        }
    }
}