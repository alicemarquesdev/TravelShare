using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using TravelShare.Filters;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    [PaginaParaUsuarioLogado]
    public class HomeController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISessao _sessao;
        private readonly INotificacaoRepository _notificacaoRepository;

        public HomeController(IPostRepository postRepository, IUsuarioRepository usuarioRepository, ISessao sessao, INotificacaoRepository notificacaoRepository)
        {
            _postRepository = postRepository;
            _usuarioRepository = usuarioRepository;
            _sessao = sessao;
            _notificacaoRepository = notificacaoRepository;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = _sessao.BuscarSessaoDoUsuario();

            var posts = await _postRepository.BuscarTodosOsPostsSeguindoAsync(usuario.Id);

            List<UsuarioModel> sugestoes = await _usuarioRepository.BuscarSugestoesParaSeguirAsync(usuario.Id);

            ViewBag.SugestoesParaSeguir = sugestoes;
            ViewBag.UsuarioLogado = usuario;

            return View(posts);
        }

        public async Task<IActionResult> Explorar(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");

            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            var posts = await _postRepository.BuscarTodosOsPostsNãoSeguindoAsync(id);

            ViewBag.UsuarioLogado = usuario;

            return View(posts);
        }

        public async Task<IActionResult> Notificacoes(string id)
        {
           var notificacoes = await _notificacaoRepository.BuscarTodasAsNotificacoesDoUsuarioAsync(id);

            return View(notificacoes);
        }


        public IActionResult Error()
        {
            var erro = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            };

            return View(erro);
        }

      

        public IActionResult Sair()
        {
            _sessao.RemoverSessaoUsuario();
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public async Task<IActionResult> PesquisarUsuarios(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
            {
                return Json(new List<object>());
            }

            var usuarios = await _usuarioRepository.PesquisarUsuariosAsync(termo);

            var usuariosRetorno = usuarios.Select(u => new {
                Nome = string.IsNullOrEmpty(u.Nome) ? "Nome Desconhecido" : u.Nome,
                Username = string.IsNullOrEmpty(u.Username) ? "@SemUsername" : "@" + u.Username,
                CidadeDeNascimento = string.IsNullOrEmpty(u.CidadeDeNascimento) ? "Cidade Desconhecida" : u.CidadeDeNascimento,
                ImagemPerfil = string.IsNullOrEmpty(u.ImagemPerfil) ? Url.Content("/imagens/default-user.png") : Url.Content(u.ImagemPerfil),
                Id = u.Id
            }).ToList();

            return Json(usuariosRetorno);
        }
    }
}