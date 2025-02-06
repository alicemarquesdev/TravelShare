using Microsoft.AspNetCore.Mvc;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    public class LoginController : Controller
    {
        private readonly ISessao _sessao;
        private readonly IUsuarioRepository _usuarioRepositorio;

        public LoginController(IUsuarioRepository usuarioRepositorio, ISessao sessao)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
        }

        public IActionResult Login()
        {
            if (_sessao.BuscarSessaoDoUsuario() != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult CriarConta()
        {
            if (_sessao.BuscarSessaoDoUsuario() != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult RedefinirSenha()
        {
            if (_sessao.BuscarSessaoDoUsuario() != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Sair()
        {
            _sessao.RemoverSessaoUsuario();
            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> CriarConta(UsuarioModel usuario)
        {
            if (ModelState.IsValid)
            {
                await _usuarioRepositorio.AddUsuarioAsync(usuario);
                TempData["Message"] = "Conta criada com sucesso! Agora você pode efetuar o login.";
                return RedirectToAction("Login");
            }
            TempData["Message"] = "Tente novamente, houve um erro ao criar sua conta.";
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Entrar(LoginModel loginmodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var usuario = await _usuarioRepositorio.BuscarUsuarioPorEmailOuUsername(loginmodel.EmailOuUsername);
                    if (usuario != null)
                    {
                        if (usuario.SenhaValida(loginmodel.Senha))
                        {
                            _sessao.CriarSessaoDoUsuario(usuario);
                            return RedirectToAction("Index", "Home");
                        }
                        TempData["Message"] = $"Senha do usuário é inválida, tente novamente.";
                    }
                    TempData["Message"] = $"Usuário não encontrado, tente novamente.";
                }
                TempData["Message"] = $"Os dados não são válidos, tente novamente.";
                return View("Login");
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Ops, não conseguimos realizar seu login: {ex.Message}";
                return RedirectToAction("Login");
            }
        }
    }
}