using Microsoft.AspNetCore.Mvc;
using TravelShare.Filters;
using TravelShare.Helper.Interfaces;
using TravelShare.Repository.Interfaces;
using TravelShare.ViewModels;

namespace TravelShare.Controllers
{
    // Controller responsável por gerenciar a página inicial da aplicação.
    // Métodos:
    // 1. Index: Retorna a página inicial com os posts das pessoas seguidas pelo usuário logado e sugestões de usuários para seguir.
    // 2. Explorar: Retorna posts de usuários que o usuário logado ainda não segue.
    // 3. Notificacoes: Retorna todas as notificações do usuário logado.
    // 4. PesquisarUsuarios: Busca usuários pelo nome ou username e retorna um JSON com as informações básicas.
    // 5. Error: Exibe a página de erro.
    [PaginaParaUsuarioLogado] // - Acesso apenas para usuários logados
    public class HomeController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISessao _sessao;
        private readonly INotificacaoRepository _notificacaoRepository;
        private readonly ILogger<HomeController> _logger;

        // Construtor que recebe as dependências necessárias para o controlador.
        // Caso alguma dependência seja nula, será lançado um throw para evitar erros durante a execução.
        public HomeController(IPostRepository postRepository, IUsuarioRepository usuarioRepository, ISessao sessao, INotificacaoRepository notificacaoRepository, ILogger<HomeController> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao));
            _notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Retorna a página inicial com os posts das pessoas seguidas pelo usuário logado e sugestões de usuários para seguir.
        // Se o usuário não estiver logado, ele será redirecionado para a página de login.
        public async Task<IActionResult> Index()
        {
            try
            {
                var usuario = _sessao.BuscarSessaoDoUsuario();
                if (usuario == null)
                {
                    _logger.LogWarning("Usuário não encontrado na sessão.");
                    return RedirectToAction("Login", "Login");
                }

                var viewModel = new HomeViewModel
                {
                    UsuarioLogado = usuario,
                    Posts = await _postRepository.BuscarTodosOsPostsSeguindoAsync(usuario.Id),
                    UsuariosSugestoes = await _usuarioRepository.BuscarSugestoesParaSeguirAsync(usuario.Id)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar a página inicial.");
                return RedirectToAction("CriarConta", "Login");
            }
        }

        // Exibe a página de erro
        public IActionResult Error()
        {
            return View();
        }

        // Retorna a página "Explorar" com posts de usuários que o usuário logado ainda não segue.
        // Se o ID do usuário for nulo ou vazio, uma exceção será lançada.
        public async Task<IActionResult> Explorar()
        {
            try
            {
                var usuario = _sessao.BuscarSessaoDoUsuario();

                if(usuario == null)
                {
                    _logger.LogWarning("Usuário não encontrado na sessão.");
                    return RedirectToAction("Login", "Login");
                }

                var viewModel = new HomeViewModel
                {
                    UsuarioLogado = usuario,
                    Posts = await _postRepository.BuscarTodosOsPostsNãoSeguindoAsync(usuario.Id)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar a página Explorar.");
                TempData["Message"] = "Erro ao carregar a página Explorar.";
                return RedirectToAction("Index");
            }
        }

        // Retorna a lista de notificações do usuário logado.
        // Se o ID do usuário for nulo ou vazio, uma exceção será lançada.
        public async Task<IActionResult> Notificacoes(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id é nulo");

                var notificacoes = await _notificacaoRepository.BuscarTodasAsNotificacoesDoUsuarioAsync(id);
                return View(notificacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar as notificações.");
                TempData["Message"] = "Erro ao carregar a página Notificaceos.";
                return RedirectToAction("Index");
            }
        }

        // Busca usuários pelo nome ou username e retorna um JSON com as informações básicas.
        // Se o termo de pesquisa estiver vazio, retorna uma lista vazia.
        [HttpGet]
        public async Task<IActionResult> PesquisarUsuarios(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                {
                    return Json(new List<object>());
                }

                var usuarios = await _usuarioRepository.PesquisarUsuariosAsync(termo);

                var usuariosRetorno = usuarios.Select(u => new
                {
                    Nome = string.IsNullOrEmpty(u.Nome) ? "Nome Desconhecido" : u.Nome,
                    Username = string.IsNullOrEmpty(u.Username) ? "@SemUsername" : "@" + u.Username,
                    CidadeDeNascimento = string.IsNullOrEmpty(u.CidadeDeNascimento) ? "Cidade Desconhecida" : u.CidadeDeNascimento,
                    ImagemPerfil = string.IsNullOrEmpty(u.ImagemPerfil) ? Url.Content("/imagens/default-user.png") : Url.Content(u.ImagemPerfil),
                    Id = u.Id
                }).ToList();

                return Json(usuariosRetorno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar usuários.");
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
