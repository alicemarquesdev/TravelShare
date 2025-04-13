using Microsoft.AspNetCore.Mvc;
using TravelShare.Filters;
using TravelShare.Helper.Interfaces;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    // Controller responsável por adicionar e remover cidades da lista de cidades visitadas de um usuário.
    // Métodos:
    // 1. AddCidade: Recebe uma cidade do usuário e adiciona à sua lista de cidades visitadas, verificando se a cidade já foi visitada.
    // 2. RemoverCidade: Remove uma cidade da lista de cidades visitadas do usuário.
    [PaginaParaUsuarioLogado] // - Acesso apenas para usuários logados
    public class CidadesVisitadasController : Controller
    {
        private readonly ICidadesVisitadasRepository _cidadesVisitadasRepository;
        private readonly ISessao _sessao;
        private readonly ILogger<CidadesVisitadasController> _logger;

        // Construtor da classe, recebe as dependências através da injeção de dependência.
        // Adicionamos tratamento de exceções no construtor (throws) para capturar falhas de dependência.
        public CidadesVisitadasController(ICidadesVisitadasRepository cidadesVisitadasRepository, ISessao sessao, ILogger<CidadesVisitadasController> logger)
        {

            _cidadesVisitadasRepository = cidadesVisitadasRepository ?? throw new ArgumentNullException(nameof(cidadesVisitadasRepository), "O repositório de cidades visitadas não pode ser nulo.");
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao), "A sessão do usuário não pode ser nula.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "O logger não pode ser nulo.");

        }

        // Classe auxiliar para receber a cidade através da requisição
        public class CidadeRequest
        {
            public required string Cidade { get; set; }
        }

        // Adiciona uma cidade à lista de cidades visitadas do usuário.
        // Verifica se o usuário está logado e se a cidade não foi adicionada anteriormente.
        [HttpPost]
        public async Task<IActionResult> AddCidade([FromBody] CidadeRequest request)
        {
            try
            {
                var usuario = _sessao.BuscarSessaoDoUsuario();

                // Verifica se a cidade e o usuário são válidos
                if (string.IsNullOrEmpty(request.Cidade) || usuario == null)
                {
                    _logger.LogWarning("Tentativa de adicionar cidade inválida ou sem usuário logado.");
                    return Json(new { success = false, message = "Cidade inválida ou usuário não encontrado." });
                }

                // Verifica se a cidade já foi visitada pelo usuário
                var cidadeExistente = await _cidadesVisitadasRepository.VerificarCidadeVisitadaPeloUsuarioAsync(usuario.Id, request.Cidade);

                if (cidadeExistente)
                {
                    TempData["Message"] = $"A cidade {request.Cidade} já está na lista de cidades visitadas de {usuario.Nome}.";
                    return Json(new
                    {
                        success = false,
                        message = $"A cidade {request.Cidade} já está na lista de cidades visitadas de {usuario.Nome}."
                    });
                }

                // Adiciona a cidade à lista do usuário
                var sucesso = await _cidadesVisitadasRepository.AddCidadeAsync(usuario.Id, request.Cidade);

                if (sucesso)
                {
                    _logger.LogInformation($"Cidade {request.Cidade} adicionada com sucesso para o usuário {usuario.Id}.");
                }
                else
                {
                    _logger.LogError($"Falha ao adicionar a cidade {request.Cidade} para o usuário {usuario.Id}.");
                }

                return Json(new { success = sucesso });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar cidade.");
                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["Message"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["Message"] = "Ocorreu um erro ao adicionar a cidade. Tente novamente mais tarde.";
                }

                return Json(new { success = false, message = "Ocorreu um erro ao adicionar a cidade. Tente novamente mais tarde." });
            }
        }

        // Remove uma cidade da lista de cidades visitadas do usuário.
        // Verifica se a cidade e o usuário são válidos antes de tentar removê-la.
        [HttpPost]
        public async Task<IActionResult> RemoverCidade([FromBody] CidadeRequest request)
        {
            try
            {
                var usuario = _sessao.BuscarSessaoDoUsuario();

                // Verifica se a cidade e o usuário são válidos
                if (string.IsNullOrEmpty(request.Cidade) || usuario == null)
                {
                    _logger.LogWarning("Tentativa de remover cidade inválida ou sem usuário logado.");
                    return Json(new { success = false, message = "Cidade inválida ou usuário não encontrado." });
                }

                // Remove a cidade da lista do usuário
                var sucesso = await _cidadesVisitadasRepository.RemoveCidadeAsync(usuario.Id, request.Cidade);

                if (sucesso)
                {
                    _logger.LogInformation($"Cidade {request.Cidade} removida com sucesso para o usuário {usuario.Id}.");
                }
                else
                {
                    _logger.LogError($"Falha ao remover a cidade {request.Cidade} para o usuário {usuario.Id}.");
                }

                return Json(new { success = sucesso });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover cidade.");
                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["Message"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["Message"] = "Ocorreu um erro ao remover a cidade. Tente novamente mais tarde.";
                }

                return Json(new { success = false, message = "Ocorreu um erro ao remover a cidade. Tente novamente mais tarde." });
            }
        }
    }
}
