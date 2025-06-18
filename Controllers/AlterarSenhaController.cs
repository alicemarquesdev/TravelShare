using Microsoft.AspNetCore.Mvc;
using TravelShare.Filters;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    // Controller responsável por Alterar Senha do usuário.
    // GET - AlterarSenha(string id) - Exibe a view
    // POST - AlterarSenha(AlterarSenhaModel alterarSenha)
    [PaginaParaUsuarioLogado] // - Acesso apenas para usuários logados
    public class AlterarSenhaController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAlteracaoSenhaRepository _alterarSenhaRepository;
        private readonly ILogger<AlterarSenhaController> _logger;

        // Adicionando throws para garantir que as dependências sejam não nulas
        public AlterarSenhaController(
            IUsuarioRepository usuarioRepository,
            IAlteracaoSenhaRepository alterarSenhaRepository,
            ILogger<AlterarSenhaController> logger)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository), "Usuário Repository não pode ser nulo.");
            _alterarSenhaRepository = alterarSenhaRepository ?? throw new ArgumentNullException(nameof(alterarSenhaRepository), "Alteração de Senha Repository não pode ser nulo.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger não pode ser nulo.");
        }

        // Método GET para alterar senha
        public async Task<IActionResult> AlterarSenha(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id é nulo");
            }

            try
            {
                var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

                if (usuario == null)
                {
                    throw new ArgumentNullException("Usuário não encontrado no banco de dados.");

                }

                var alterarSenhaModel = new AlterarSenhaModel
                {
                    Id = usuario.Id,
                    SenhaAtual = string.Empty,
                    NovaSenha = string.Empty,
                    NovaSenhaConfirmacao = string.Empty,
                };

                return View(alterarSenhaModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar carregar AlterarSenha.");
                TempData["Message"] = "Desculpe, erro ao tentar carregar a página.";
                return RedirectToAction("Perfil", "Usuario", new { id = id});
            }
        }

        // Método POST para alterar senha
        [HttpPost]
        [ValidateAntiForgeryToken] // Validar Token
        public async Task<IActionResult> AlterarSenha(AlterarSenhaModel alterarSenha)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var usuarioDb = await _usuarioRepository.BuscarUsuarioPorIdAsync(alterarSenha.Id);

                    if (usuarioDb == null)
                    {
                        throw new ArgumentNullException("Usuário não encontrado no banco de dados.");
                    }


                    var senhaAlterada = await _alterarSenhaRepository.AlterarSenhaAsync(alterarSenha);

                    if (!senhaAlterada)
                    {
                        throw new ArgumentNullException("Erro ao alterar senha no repositorio.");
                    }

                    _logger.LogInformation($"Senha alterada com sucesso para o usuário {alterarSenha.Id}.");
                    TempData["Message"] = "Senha alterada com sucesso!";

                    return RedirectToAction("AlterarSenha", "AlterarSenha");
                }

                _logger.LogWarning("Modelo de dados inválido para alteração de senha.");
                throw new InvalidOperationException("Os dados inseridos não são válidos");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar alterar a senha.");
                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["Message"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["Message"] = "Ocorreu um erro ao tentar alterar a senha. Tente novamente.";
                }

                return RedirectToAction("AlterarSenha", "AlterarSenha");
            }

        }
    }
}
