using Microsoft.AspNetCore.Mvc;
using TravelShare.Filters;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    [PaginaParaUsuarioDeslogado]
    public class LoginController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISessao _sessao;
        private readonly IEmail _email;
        private readonly IAlterarSenhaRepository _alterarSenhaRepository;

        public LoginController(IUsuarioRepository usuarioRepository, ISessao sessao, IEmail email, IAlterarSenhaRepository alterarSenhaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _sessao = sessao;
            _email = email;
            _alterarSenhaRepository = alterarSenhaRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult CriarConta()
        {
            return View();
        }

        public IActionResult RedefinirSenha()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> CriarConta(UsuarioModel usuario)
        {
            if (ModelState.IsValid)
            {
                var emailExistente = await _usuarioRepository.BuscarUsuarioPorEmailOuUsernameAsync(usuario.Email);
                var usernameExistente = await _usuarioRepository.BuscarUsuarioPorEmailOuUsernameAsync(usuario.Username);

                if (emailExistente != null)
                {
                    // Se já existir, exibe uma mensagem de erro
                    TempData["Message"] = "Já existe uma conta com esse email.";
                    return View(usuario);
                }
                if (usernameExistente != null)
                {
                    // Se já existir, exibe uma mensagem de erro
                    TempData["Message"] = "Já existe uma conta com esse username.";
                    return View(usuario);
                }

                await _usuarioRepository.AddUsuarioAsync(usuario);
                TempData["Message"] = "Conta criada com sucesso! Agora você pode efetuar o login.";
                return RedirectToAction("Login");
            }
            TempData["Message"] = "Tente novamente, houve um erro ao criar sua conta.";
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Entrar(LoginModel loginmodel)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _usuarioRepository.BuscarUsuarioPorEmailOuUsernameAsync(loginmodel.EmailOuUsername);

                if (usuario != null)
                {
                    if (usuario.SenhaValida(loginmodel.Senha))
                    {
                        _sessao.CriarSessaoDoUsuario(usuario);
                        return RedirectToAction("Index", "Home");
                    }
                    TempData["Message"] = $"Senha inválida, tente novamente.";
                }
                TempData["Message"] = $"Usuário não encontrado, tente novamente.";
            }

            TempData["Message"] = $"Os dados não são válidos, tente novamente.";
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> EnviarLinkParaRedefinirSenha(string email)
        {
            var usuarioDb = await _usuarioRepository.BuscarUsuarioPorEmailOuUsernameAsync(email);

            if (usuarioDb != null)
            {

                string novaSenha = usuarioDb.GerarNovaSenha();

                string mensagem = $"Olá {usuarioDb.Nome},<br><br>Sua nova senha é: <strong>{novaSenha}</strong><br><br>Altere sua senha assim que possível.";
                var emailEnviado = await _email.EnviarEmailAsync(usuarioDb.Email, "Redefinição de Senha - TravelShare", mensagem);

                if (emailEnviado)
                {
                    
                    var senhaAlterada = await _usuarioRepository.RedefinirSenha(usuarioDb.Id, novaSenha);

                    if (senhaAlterada)
                    {
                        TempData["Message"] = $"Enviamos para seu e-mail cadastrado uma nova senha.";
                        return RedirectToAction("Login", "Login");

                    }

                    TempData["Message"] = $"Não conseguimos enviar e-mail. Por favor, tente novamente.";
                    return RedirectToAction("Login", "Login");

                }

                TempData["Message"] = $"Email inválido, tente novamente.";
                return View("RedefinirSenha");
            }

            TempData["Message"] = $"Os dados não são válidos, tente novamente.";
            return RedirectToAction("Login", "Login");
        }

    }
}