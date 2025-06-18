using Microsoft.AspNetCore.Mvc;
using TravelShare.Filters;
using TravelShare.Helper.Interfaces;
using TravelShare.Repository.Interfaces;
using TravelShare.ViewModels;

namespace TravelShare.Controllers
{
    // Controller respons�vel por gerenciar a p�gina inicial da aplica��o.
    // M�todos:
    // 1. Index: Retorna a p�gina inicial com os posts das pessoas seguidas pelo usu�rio logado e sugest�es de usu�rios para seguir.
    // 2. Explorar: Retorna posts de usu�rios que o usu�rio logado ainda n�o segue.
    // 3. Notificacoes: Retorna todas as notifica��es do usu�rio logado.
    // 4. PesquisarUsuarios: Busca usu�rios pelo nome ou username e retorna um JSON com as informa��es b�sicas.
    // 5. Error: Exibe a p�gina de erro.
    [PaginaParaUsuarioLogado] // - Acesso apenas para usu�rios logados
    public class HomeController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISessao _sessao;
        private readonly INotificacaoRepository _notificacaoRepository;
        private readonly ILogger<HomeController> _logger;

        // Construtor que recebe as depend�ncias necess�rias para o controlador.
        // Caso alguma depend�ncia seja nula, ser� lan�ado um throw para evitar erros durante a execu��o.
        public HomeController(IPostRepository postRepository, IUsuarioRepository usuarioRepository, ISessao sessao, INotificacaoRepository notificacaoRepository, ILogger<HomeController> logger)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao));
            _notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Retorna a p�gina inicial com os posts das pessoas seguidas pelo usu�rio logado e sugest�es de usu�rios para seguir.
        // Se o usu�rio n�o estiver logado, ele ser� redirecionado para a p�gina de login.
        public async Task<IActionResult> Index()
        {
            try
            {
                var usuario = _sessao.BuscarSessaoDoUsuario();
                if (usuario == null)
                {
                    _logger.LogWarning("Usu�rio n�o encontrado na sess�o.");
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
                _logger.LogError(ex, "Erro ao carregar a p�gina inicial.");
                return RedirectToAction("CriarConta", "Login");
            }
        }

        // Exibe a p�gina de erro
        public IActionResult Error()
        {
            return View();
        }

        // Retorna a p�gina "Explorar" com posts de usu�rios que o usu�rio logado ainda n�o segue.
        // Se o ID do usu�rio for nulo ou vazio, uma exce��o ser� lan�ada.
        public async Task<IActionResult> Explorar()
        {
            try
            {
                var usuario = _sessao.BuscarSessaoDoUsuario();

                if(usuario == null)
                {
                    _logger.LogWarning("Usu�rio n�o encontrado na sess�o.");
                    return RedirectToAction("Login", "Login");
                }

                var viewModel = new HomeViewModel
                {
                    UsuarioLogado = usuario,
                    Posts = await _postRepository.BuscarTodosOsPostsN�oSeguindoAsync(usuario.Id)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar a p�gina Explorar.");
                TempData["Message"] = "Erro ao carregar a p�gina Explorar.";
                return RedirectToAction("Index");
            }
        }

        // Retorna a lista de notifica��es do usu�rio logado.
        // Se o ID do usu�rio for nulo ou vazio, uma exce��o ser� lan�ada.
        public async Task<IActionResult> Notificacoes(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id � nulo");

                var notificacoes = await _notificacaoRepository.BuscarTodasAsNotificacoesDoUsuarioAsync(id);
                return View(notificacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar as notifica��es.");
                TempData["Message"] = "Erro ao carregar a p�gina Notificaceos.";
                return RedirectToAction("Index");
            }
        }

        // Busca usu�rios pelo nome ou username e retorna um JSON com as informa��es b�sicas.
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
                _logger.LogError(ex, "Erro ao pesquisar usu�rios.");
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
