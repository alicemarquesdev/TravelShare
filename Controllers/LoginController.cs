using Microsoft.AspNetCore.Mvc;
using TravelShare.Filters;
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
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISessao _sessao;
        private readonly IEmail _email;
        private readonly IAlteracaoSenhaRepository _alterarSenhaRepository;
        private readonly ILogger<LoginController> _logger;

        // Construtor que injeta as dependências e verifica se nenhuma delas é nula
        public LoginController(
            IUsuarioRepository usuarioRepository,
            ISessao sessao,
            IEmail email,
            IAlteracaoSenhaRepository alterarSenhaRepository,
            ILogger<LoginController> logger)
        {
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
        public IActionResult CriarConta() => View();

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
        public async Task<IActionResult> CriarConta(UsuarioModel usuario)
        {
            // Verifica se os dados fornecidos são válidos
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Tente novamente, houve um erro ao criar sua conta.";
                return View(usuario);
            }

            try
            {
                // Verifica se já existe um usuário com o mesmo email ou username
                var emailExistente = await _usuarioRepository.BuscarUsuarioPorEmailOuUsernameAsync(usuario.Email);
                var usernameExistente = await _usuarioRepository.BuscarUsuarioPorEmailOuUsernameAsync(usuario.Username);

                if (emailExistente != null)
                {
                    TempData["Message"] = "Já existe uma conta com esse email.";
                    return View(usuario);
                }
                if (usernameExistente != null)
                {
                    TempData["Message"] = "Já existe uma conta com esse username.";
                    return View(usuario);
                }

                // Adiciona o usuário ao banco de dados
                await _usuarioRepository.AddUsuarioAsync(usuario);
                TempData["Message"] = "Conta criada com sucesso! Agora você pode efetuar o login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar conta.");
                TempData["Message"] = "Erro interno ao criar conta. Tente novamente mais tarde.";
                return View(usuario);
            }
        }

        // Método necessário para efetuar Login.
        [HttpPost]
        public async Task<IActionResult> Entrar(LoginModel loginmodel)
        {
            // Verifica se os dados enviados são válidos
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Os dados não são válidos, tente novamente.";
                return View("Login");
            }

            try
            {
                // Busca o usuário no banco pelo email ou username fornecido
                var usuario = await _usuarioRepository.BuscarUsuarioPorEmailOuUsernameAsync(loginmodel.EmailOuUsername);

                if (usuario != null)
                {
                    // Valida a senha do usuário
                    if (usuario.SenhaValida(loginmodel.Senha))
                    {
                        _sessao.CriarSessaoDoUsuario(usuario); // Cria a sessão do usuário logado
                        return RedirectToAction("Index", "Home");
                    }

                    TempData["Message"] = "Senha inválida, tente novamente.";
                }
                else
                {
                    TempData["Message"] = "Usuário não encontrado, tente novamente.";
                }

                return View("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar realizar login.");
                TempData["Message"] = "Erro interno ao realizar login. Tente novamente mais tarde.";
                return View("Login");
            }
        }

        // Metodo usado para redefinir a senha, email com nova senha é enviado ao email do usuario.
        [HttpPost]
        public async Task<IActionResult> EnviarLinkParaRedefinirSenha(RedefinirSenhaModel redefinirSenhaModel)
        {
            // Verifica se os dados enviados são válidos
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Os dados não são válidos, tente novamente.";
                return View("RedefinirSenha");
            }

            try
            {
                // Busca o usuário no banco de dados pelo email fornecido
                var usuarioDb = await _usuarioRepository.BuscarUsuarioPorEmailOuUsernameAsync(redefinirSenhaModel.Email);

                if (usuarioDb == null)
                {
                    TempData["Message"] = "Email inválido, tente novamente.";
                    return View("RedefinirSenha");
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
                    return View("RedefinirSenha");
                }

                TempData["Message"] = "Não conseguimos enviar o e-mail. Tente novamente mais tarde.";
                return View("RedefinirSenha");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar redefinir senha.");
                TempData["Message"] = "Erro interno ao redefinir senha. Tente novamente mais tarde.";
                return View("RedefinirSenha");
            }
        }
    }
}
