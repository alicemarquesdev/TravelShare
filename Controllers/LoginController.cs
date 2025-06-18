using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TravelShare.Filters;
using TravelShare.Helper;
using TravelShare.Helper.Interfaces;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    // Controller responsável por gerenciar o processo de autenticação e criação de conta dos usuários.  
    // Métodos:  
    // 1. Index: Retorna a página inicial do login.  
    // 2. Login: Retorna a view de login.  
    // 3. CriarConta: Retorna a view para criação de conta.  
    // 4. RedefinirSenha: Retorna a view para redefinição de senha.  
    // 5. Sair: Encerra a sessão do usuário e redireciona para a página de login.  
    // 6. CriarConta (POST): Processa a criação de um novo usuário, verificando duplicatas e salvando no banco de dados.  
    // 7. Entrar (POST): Processa a autenticação do usuário, validando email/username e senha, e criando uma sessão.  
    // 8. EnviarLinkParaRedefinirSenha (POST): Gera uma nova senha temporária e envia por e-mail ao usuário, atualizando no banco de dados.  
    [PaginaParaUsuarioDeslogado] // - Acesso apenas para usuários deslogados  
    public class LoginController : Controller
    {
        public readonly string _googleAPIKey;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISessao _sessao;
        private readonly IEmail _email;
        private readonly IAlteracaoSenhaRepository _alterarSenhaRepository;
        private readonly ILogger<LoginController> _logger;

        // Construtor que injeta as dependências e verifica se nenhuma delas é nula
        public LoginController(IOptions<GoogleAPISettings> googleAPISettings,
            IUsuarioRepository usuarioRepository,
            ISessao sessao,
            IEmail email,
            IAlteracaoSenhaRepository alterarSenhaRepository,
            ILogger<LoginController> logger)
        {
            _googleAPIKey = googleAPISettings.Value.ApiKey ?? throw new ArgumentNullException("A chave da API do Google não foi configurada corretamente.");
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao));
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _alterarSenhaRepository = alterarSenhaRepository ?? throw new ArgumentNullException(nameof(alterarSenhaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Página inicial de login
        public IActionResult Index() => View();

        // Exibe a página de login
        public IActionResult Login() => View();

        // Exibe a página de criação de conta
        public IActionResult CriarConta()
        {
            ViewBag.GoogleApiKey = _googleAPIKey; // Passa a chave da API do Google para a view
            return View();
        }

        // Exibe a página de redefinição de senha
        public IActionResult RedefinirSenha() => View();

        // Método para encerrar a sessão do usuário e redirecionar para a página de login
        public IActionResult Sair()
        {
            _sessao.RemoverSessaoDoUsuario(); // Remove os dados da sessão
            return RedirectToAction("Login", "Login");
        }

        // Metodo usado para criar conta do usuário.
        [HttpPost]
        [ValidateAntiForgeryToken] // Validar Token
        public async Task<IActionResult> CriarConta(UsuarioModel usuario)
        {
            // Verifica se os dados fornecidos são válidos
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Preencha todos os campos corretamente.";
                return View(usuario);
            }

            try
            {
                // Verifica se já existe um usuário com o mesmo email ou username
                var usuarioExistente = await _usuarioRepository.BuscarUsuarioPorEmailOuUsernameAsync(usuario.Email);

                if (usuarioExistente != null)
                {
                    if (usuarioExistente.Email.ToLower() == usuario.Email.ToLower())
                    {
                        TempData["Message"] = "Já existe uma conta com esse email.";
                        return View(usuario);
                    }
                    else if (usuarioExistente.Username.ToLower() == usuario.Username.ToLower())
                    {
                        TempData["Message"] = "Já existe uma conta com esse username.";
                        return View(usuario);
                    }
                }

                // Adiciona o usuário ao banco de dados
                await _usuarioRepository.AddUsuarioAsync(usuario);
                TempData["Message"] = "Conta criada com sucesso! Agora você pode efetuar o login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar conta.");
                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["Message"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["Message"] = "Erro ao criar conta. Tente novamente mais tarde.";
                }

                return View(usuario);
            }
        }

        // Método necessário para efetuar Login.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Entrar(LoginModel loginmodel)
        {
            // Verifica se os dados enviados são válidos
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Preencha todos os campos corretamente.";
                return View("Login", loginmodel);
            }

            try
            {
                var usuario = await _usuarioRepository.BuscarUsuarioPorEmailOuUsernameAsync(loginmodel.EmailOuUsername);

                if (usuario != null)
                {
                    if (usuario.SenhaValida(loginmodel.Senha))
                    {
                        _sessao.CriarSessaoDoUsuario(usuario);
                        _logger.LogInformation($"Usuário {usuario.Username} logado com sucesso.");
                        return RedirectToAction("Index", "Home");
                    }

                    TempData["Message"] = "Senha inválida, tente novamente.";
                    return View("Login", loginmodel);
                }

                TempData["Message"] = "Email ou username inválidos.";
                return View("Login", loginmodel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar realizar login.");
                TempData["Message"] = "Erro ao realizar login. Tente novamente mais tarde.";
                return View("Login", loginmodel);
            }
        }


        // Metodo usado para redefinir a senha, email com nova senha é enviado ao email do usuario.
        [HttpPost]
        [ValidateAntiForgeryToken] // Validar Token
        public async Task<IActionResult> EnviarLinkParaRedefinirSenha(RedefinirSenhaModel redefinirSenhaModel)
        {
            // Verifica se os dados enviados são válidos
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Preencha o campo corretamente.";
                return View(redefinirSenhaModel);
            }

            try
            {
                // Busca o usuário no banco de dados pelo email fornecido
                var usuarioDb = await _usuarioRepository.BuscarUsuarioPorEmailOuUsernameAsync(redefinirSenhaModel.Email);

                if (usuarioDb == null)
                {
                    TempData["Message"] = "Email inválido, tente novamente.";
                    return View(redefinirSenhaModel);
                }

                // Gera uma nova senha temporária para o usuário
                string novaSenha = usuarioDb.GerarNovaSenha();
                string mensagem = $"Olá {usuarioDb.Nome},<br><br>Sua nova senha é: <strong>{novaSenha}</strong><br><br>Altere sua senha assim que possível.";

                // Envia um email para o usuário com a nova senha gerada
                var emailEnviado = await _email.EnviarEmailAsync(usuarioDb.Email, "Redefinição de Senha - TravelShare", mensagem);

                if (emailEnviado)
                {
                    // Atualiza a senha no banco de dados
                    var senhaAlterada = await _alterarSenhaRepository.RedefinirSenha(usuarioDb.Id, novaSenha);

                    if (senhaAlterada)
                    {
                        TempData["Message"] = "Enviamos para seu e-mail cadastrado uma nova senha.";
                        return RedirectToAction("Login", "Login");
                    }

                    TempData["Message"] = "Não conseguimos redefinir sua senha. Por favor, tente novamente.";
                    return View(redefinirSenhaModel);
                }
                TempData["Message"] = "Não conseguimos enviar o e-mail. Tente novamente mais tarde.";
                return View(redefinirSenhaModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar redefinir senha.");
                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["Message"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["Message"] = "Erro ao redefinir senha. Tente novamente mais tarde.";
                }

                return View("RedefinirSenha");
            }
        }
    }
}
