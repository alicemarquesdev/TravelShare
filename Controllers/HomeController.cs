using Microsoft.AspNetCore.Mvc;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ISessao _sessao;
        private readonly IUsuarioRepository _usuarioRepository;

        public HomeController(IPostRepository postRepository, ISessao sessao, IUsuarioRepository usuarioRepository)
        {
            _postRepository = postRepository;
            _sessao = sessao;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = _sessao.BuscarSessaoDoUsuario();

            if (usuario == null) return RedirectToAction("Login", "Login");

            var posts = await _postRepository.BuscarTodosOsPostsSeguindoAsync(usuario.Id);
            List<UsuarioModel> sugestoes = await _usuarioRepository.BuscarSugestoesParaSeguir(usuario.Id);

            ViewBag.UsuarioLogado = usuario;
            ViewBag.SugestoesParaSeguir = sugestoes;

            return View(posts);
        }

        public async Task<IActionResult> Explorar(string id)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            if (usuario == null) return RedirectToAction("Login", "Login");

            var posts = await _postRepository.BuscarTodosOsPostsNãoSeguindoAsync(id);

            ViewBag.UsuarioLogado = usuario;

            return View(posts);
        }

        public async Task<IActionResult> Pesquisar(string id)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            if (usuario == null) return RedirectToAction("Login", "Login");

            List<UsuarioModel> sugestoes = await _usuarioRepository.BuscarSugestoesParaSeguir(usuario.Id);

            ViewBag.UsuarioLogado = usuario;

            return View(sugestoes);
        }
    }
}