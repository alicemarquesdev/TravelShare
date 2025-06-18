using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TravelShare.Filters;
using TravelShare.Helper;
using TravelShare.Helper.Interfaces;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;
using TravelShare.ViewModels;

namespace TravelShare.Controllers
{
    // Controller responsável por gerenciar as ações relacionadas ao perfil e gerenciamento do usuário.
    // Métodos:
    // 1. Perfil: Exibe o perfil de um usuário, incluindo informações pessoais, posts, seguidores e seguindo.
    // 2. EditarPerfil (GET): Exibe a página de edição de perfil para o usuário, permitindo que ele altere seus dados pessoais.
    // 3. EditarPerfil (POST): Atualiza os dados do usuário no banco de dados, incluindo a possibilidade de atualizar a imagem de perfil.
    // 4. DeletarConta: Exclui a conta do usuário, removendo todos os dados associados ao perfil.
    [PaginaParaUsuarioLogado] // - Acesso apenas para usuários logados
    public class UsuarioController : Controller
    {
        private readonly string _googleAPIKey;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICaminhoImagem _caminhoImagem;
        private readonly ISessao _sessao;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(IOptions<GoogleAPISettings> googleAPISettings, IUsuarioRepository usuarioRepository, IPostRepository postRepository,
                                 ICaminhoImagem caminhoImagem, ISessao sessao, ILogger<UsuarioController> logger)
        {
            _googleAPIKey = googleAPISettings.Value.ApiKey ?? throw new ArgumentNullException("A chave da API do Google não foi configurada corretamente.");
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _caminhoImagem = caminhoImagem ?? throw new ArgumentNullException(nameof(caminhoImagem));
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        // Método para exibir o perfil de um usuário
        public async Task<IActionResult> Perfil(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("Id é nulo");
            }

            try
            {
                var usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                var usuarioPerfil = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

                // Se o usuário não for encontrado
                if (usuarioPerfil == null)
                {
                    throw new Exception("Usuário não encontrado no banco de dados");
                }

                var viewModel = new UsuarioViewModel
                {
                    UsuarioLogadoId = usuarioLogado.Id,
                    PerfilDoUsuarioLogado = usuarioLogado.Id == usuarioPerfil.Id,
                    UsuarioPerfil = usuarioPerfil,
                    Posts = await _postRepository.BuscarTodosOsPostsDoUsuarioAsync(usuarioPerfil.Id),
                    Seguidores = await _usuarioRepository.BuscarVariosUsuariosPorIdAsync(usuarioPerfil.Seguidores),
                    Seguindo = await _usuarioRepository.BuscarVariosUsuariosPorIdAsync(usuarioPerfil.Seguindo),
                    GoogleAPIKey = _googleAPIKey
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exibir o perfil do usuário.");
                TempData["Message"] = "Ocorreu um erro ao carregar o perfil.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Método para exibir a página de edição do perfil
        public async Task<IActionResult> EditarPerfil()
        {
            var sessaoUsuario = _sessao.BuscarSessaoDoUsuario();

            try
            {
                var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(sessaoUsuario.Id);

                // Se o usuário não for encontrado
                if (usuario == null)
                {
                    throw new Exception("Usuário não encontrado no banco de dados");
                }

                var usuarioSemSenha = new UsuarioSemSenhaModel
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Username = usuario.Username,
                    Email = usuario.Email,
                    DataNascimento = usuario.DataNascimento,
                    CidadeDeNascimento = usuario.CidadeDeNascimento,
                    Bio = usuario.Bio,
                    ImagemPerfil = usuario.ImagemPerfil,
                    CidadesVisitadas = usuario.CidadesVisitadas
                };

                ViewBag.GoogleApiKey = _googleAPIKey;

                return View(usuarioSemSenha);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar a página de edição de perfil.");
                TempData["Message"] = "Erro ao carregar a página de edição de perfil.";
                return RedirectToAction("Perfil", new { id = sessaoUsuario.Id });
            }
        }

        // Método POST para editar o perfil
        [HttpPost]
        [ValidateAntiForgeryToken] // Validar Token
        public async Task<IActionResult> EditarPerfil(UsuarioSemSenhaModel usuario, IFormFile? imagem)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Message"] = "Preencha os campos corretamente";
                    return View(usuario);
                }
                _logger.LogInformation($"Data recebida: {usuario.DataNascimento}");


                var usuarioDb = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuario.Id);

                if (usuarioDb == null)
                {
                    throw new Exception("Usuário não encontrado no banco de dados");
                }

                var emailExistente = await _usuarioRepository.BuscarUsuarioPorEmailOuUsernameAsync(usuario.Email);
                var usernameExistente = await _usuarioRepository.BuscarUsuarioPorEmailOuUsernameAsync(usuario.Username);

                if (emailExistente != null && emailExistente.Id != usuario.Id)
                {
                    TempData["Message"] = "Já existe uma conta com esse email.";
                    return View(usuario);
                }

                if (usernameExistente != null && usernameExistente.Id != usuario.Id)
                {

                    TempData["Message"] = "Já existe uma conta com esse username.";
                    return View(usuario);
                }

                if (imagem != null)
                {
                    var caminhoImagem = await _caminhoImagem.GerarCaminhoArquivoAsync(imagem);
                    if (!string.IsNullOrEmpty(caminhoImagem))
                    {
                        usuario.ImagemPerfil = caminhoImagem;

                        // Remover imagem antiga 
                        if (usuarioDb.ImagemPerfil != "~/image/profile-img.jpg")
                        {
                            await _caminhoImagem.RemoverImagemAntiga(usuarioDb.ImagemPerfil);
                        }
                    }
                }
                else
                {
                    usuario.ImagemPerfil = usuarioDb.ImagemPerfil;
                }

                var usuarioAtualizado = await _usuarioRepository.AtualizarUsuarioAsync(usuario);

                if (usuarioAtualizado)
                {
                    TempData["Message"] = "Os dados foram atualizados com sucesso.";
                    _logger.LogInformation($"Perfil do usuário {usuario.Id} atualizado com sucesso.");
                }
                else
                {
                    TempData["Message"] = "Nenhum dado alterado.";
                    return RedirectToAction("EditarPerfil","Usuario");
                }

                return RedirectToAction("Perfil", new { id = usuario.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar o perfil do usuário.");
                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["Message"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["Message"] = "Ocorreu um erro ao atualizar os dados. Tente novamente.";
                }

                return View(usuario); // Retorna a view com o modelo que contém os erros
            }
        }
        

        // Método POST para deletar a conta do usuário
        [HttpPost]
        [ValidateAntiForgeryToken] // Validar Token
        public async Task<IActionResult> DeletarConta()
        {
            try
            {
                var usuario = _sessao.BuscarSessaoDoUsuario();

                // armazenar string da foto do perfil do usuário
                var usuarioImagemPerfil = usuario.ImagemPerfil;

                // Deleta o usuário do banco de dados
                var usuarioDeletado = await _usuarioRepository.DeletarUsuarioAsync(usuario.Id);

                // Deletar foto do perfil no servidor
                if (usuarioDeletado)
                {
                    if (usuario.ImagemPerfil != "~/image/profile-img.jpg")
                    {
                        await _caminhoImagem.RemoverImagemAntiga(usuarioImagemPerfil);
                    }
                }

                TempData["Message"] = "Conta excluída com sucesso.";

                _logger.LogInformation($"Usuário {usuario.Id} excluiu sua conta com sucesso.");
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar excluir a conta do usuário.");
                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["Message"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["Message"] = "Ocorreu um erro ao tentar excluir a conta. Tente novamente.";
                }

                return RedirectToAction("Perfil", new { id = _sessao.BuscarSessaoDoUsuario().Id });
            }
        }
    }
}
