using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using TravelShare.Filters;
using TravelShare.Helper;
using TravelShare.Helper.Interfaces;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    // Controlador responsável por gerenciar as ações relacionadas aos posts na aplicação.
    // Métodos:
    // 1. CriarPost (GET): Retorna a página de criação de um novo post.,    
    // 2. EditarPost (GET): Exibe o post a ser editado, recuperando-o pelo ID.
    // 3. CriarPost (POST): Processa a criação de um novo post, validando os dados, gerenciando imagens e salvando no banco.
    // 4. EditarPost (POST): Atualiza o post existente no banco de dados.
    // 5. DeletarPost (POST): Deleta um post, removendo também as imagens associadas ao post.

    [PaginaParaUsuarioLogado] // - Restringe o acesso a usuários logados para garantir que apenas usuários autenticados possam interagir com os posts.
    public class PostController : Controller
    {
        private readonly string _googleAPIKey;
        private readonly IPostRepository _postRepository;
        private readonly ISessao _sessao;
        private readonly ICaminhoImagem _caminhoImagem;
        private readonly ICidadesVisitadasRepository _cidadesVisitadasRepository;
        private readonly ILogger<PostController> _logger;

        // Construtor do controlador, responsável por injetar as dependências.
        // Se qualquer uma das dependências for nula, lança exceção para evitar erros.
        public PostController(IOptions<GoogleAPISettings> googleAPISettings,
                              IPostRepository postRepository,
                              ISessao sessao,
                              ICaminhoImagem caminhoImagem,
                              ICidadesVisitadasRepository cidadesVisitadasRepository,
                              ILogger<PostController> logger)
        {
            _googleAPIKey = googleAPISettings.Value.ApiKey ?? throw new ArgumentNullException("A chave da API do Google não foi configurada corretamente.");
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository), "Repositório de posts não pode ser nulo.");
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao), "Sessão não pode ser nula.");
            _caminhoImagem = caminhoImagem ?? throw new ArgumentNullException(nameof(caminhoImagem), "Caminho da imagem não pode ser nulo.");
            _cidadesVisitadasRepository = cidadesVisitadasRepository ?? throw new ArgumentNullException(nameof(cidadesVisitadasRepository), "Cidades Visitadas Repositorio não pode ser nulo.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger não pode ser nulo.");
        }

        // Método responsável por exibir a página de criação de um novo post
        public IActionResult CriarPost()
        {
            ViewBag.GoogleApiKey = _googleAPIKey;

            return View();
        }

        // Método para exibir o post para edição, buscando o post por ID.
        public async Task<IActionResult> EditarPost(string id)
        {
            try
            {
                // Busca o post pelo ID
                var post = await _postRepository.BuscarPostPorIdAsync(id);

                // Se o post não for encontrado, exibe uma mensagem e redireciona
                if (post == null)
                {
                    throw new Exception("Post não encontrado");
                }

                ViewBag.GoogleApiKey = _googleAPIKey;

                // Caso o post seja encontrado, retorna a view de edição com o post
                return View(post);
            }
            catch (Exception ex)
            {
                // Em caso de erro, registra o erro no log e exibe uma mensagem amigável ao usuário
                _logger.LogError(ex, "Erro ao buscar o post para edição.");
                TempData["Message"] = "Erro ao carregar o post para edição.";
                return RedirectToAction("Perfil", "Usuario");  // Redireciona para o perfil do usuário
            }
        }

        // Método para criar um novo post, recebendo os dados do post e imagens
        [HttpPost]
        [ValidateAntiForgeryToken] // Validar Token
        public async Task<IActionResult> CriarPost(PostModel post, IList<IFormFile> ImagemPost)
        {
            try
            {
                // Busca o usuário logado na sessão
                var usuario = _sessao.BuscarSessaoDoUsuario();
                if (usuario == null)
                {
                    throw new Exception("Usuario não encontrado no banco de dados.");
                }

                post.UsuarioId = usuario.Id;  // Atribui o ID do usuário ao post
                ModelState.Clear(); // Isso força a revalidação do modelo
                TryValidateModel(post); // Revalida o modelo manualmente

                // Verifica se o estado do modelo é válido (se todos os campos obrigatórios foram preenchidos corretamente)
                if (ModelState.IsValid)
                {
                    // Lista para armazenar os caminhos das imagens que serão salvas
                    var caminhosImagens = new List<string>();

                    // Processa cada imagem recebida no formulário
                    foreach (var imagem in ImagemPost)
                    {

                        // Gera o caminho do arquivo da imagem
                        var caminhoImagem = await _caminhoImagem.GerarCaminhoArquivoAsync(imagem);

                        // Se o caminho for nulo, significa que houve erro ao salvar a imagem
                        if (caminhoImagem == null)
                        {
                            throw new Exception("Erro ao salvar imagem");
                        }

                        // Adiciona o caminho da imagem à lista
                        caminhosImagens.Add(caminhoImagem);
                    }

                    // Atribui os caminhos das imagens ao post
                    post.ImagemPost = caminhosImagens;
                    // Gera um novo ID para o post
                    post.Id = ObjectId.GenerateNewId().ToString();

                    // Chama o repositório para salvar o post no banco de dados
                    await _postRepository.AddPostAsync(post);

                    // Adicionar a localizacao do post no mapa.
                    if(post.Localizacao != null)
                    {
                        await _cidadesVisitadasRepository.AddCidadeAsync(post.UsuarioId, post.Localizacao);
                    }

                    // Exibe mensagem de sucesso e redireciona para o perfil do usuário
                    TempData["Message"] = "Post publicado com sucesso!";
                    return RedirectToAction("Perfil", "Usuario", new { id = post.UsuarioId });
                }

                TempData["Message"] = "Preencha os dados corretamente.";
                return View(post);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar o post.");
                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["Message"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["Message"] = "Ocorreu um erro ao criar o post. Tente novamente.";
                }

                return View(post);  // Retorna à view com o post
            }
        }

        // Método para editar um post existente
        [HttpPost]
        [ValidateAntiForgeryToken] // Validar Token
        public async Task<IActionResult> EditarPost(PostModel post)
        {
            try
            {
                // Verifica se os dados do modelo são válidos antes de tentar salvar
                if (!ModelState.IsValid)
                {
                    TempData["Message"] = "Preencha os campos corretamente";
                    return View(post);
                }

                // Busca o post existente pelo ID
                var postExistente = await _postRepository.BuscarPostPorIdAsync(post.Id);
                if (postExistente == null)
                {
                    throw new Exception("Post não encontrado no banco de dados.");
                }

                // Atualiza o post existente com os novos dados
                await _postRepository.AtualizarPostAsync(post);

                if (post.Localizacao != null)
                {
                    var cidadeVisitada = await _cidadesVisitadasRepository.VerificarCidadeVisitadaPeloUsuarioAsync(post.UsuarioId, post.Localizacao);
                    if(!cidadeVisitada)
                    {
                        await _cidadesVisitadasRepository.AddCidadeAsync(post.UsuarioId, post.Localizacao);
                    }
                }
                TempData["Message"] = "Post atualizado com sucesso.";  // Exibe mensagem de sucesso

                return RedirectToAction("EditarPost", new { id = post.Id });  // Redireciona para a página de edição

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar o post.");
                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["Message"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["Message"] = "Ocorreu um erro ao editar o post. Tente novamente.";
                }

                return View(post);  // Retorna à view com o post
            }
        }

        // Método para deletar um post
        [HttpPost]
        [ValidateAntiForgeryToken] // Validar Token
        public async Task<IActionResult> DeletarPost(string id)
        {
            try
            {
                // Verifica se o ID do post foi fornecido
                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("ID do post não fornecido.");
                                   }

                // Busca o post existente pelo ID
                var postExistente = await _postRepository.BuscarPostPorIdAsync(id);

                // Se o post não for encontrado
                if (postExistente == null)
                {
                    throw new Exception("Post não encontrado no banco de dados.");
                }

                // Cria lista para armazenar os caminhos das imagens do post
                var postsCaminho = new List<string>(postExistente.ImagemPost);

                // Deleta o post do repositório
                var sucesso = await _postRepository.DeletarPostAsync(id);

                // Se a exclusão for bem-sucedida, remove as imagens antigas
                if (sucesso)
                {
                    foreach (var imagem in postsCaminho)
                    {
                        await _caminhoImagem.RemoverImagemAntiga(imagem);  // Remove as imagens do sistema de arquivos
                    }

                    TempData["Message"] = "Post deletado com sucesso.";  // Exibe mensagem de sucesso
                    return RedirectToAction("Perfil", "Usuario", new {id = postExistente.UsuarioId});  // Redireciona para o perfil do usuário
                }

                // Caso não tenha sido possível deletar o post, exibe mensagem de erro
                throw new Exception("Erro ao deletar o post.");
                            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar o post.");
                if (ex.InnerException is InvalidOperationException || ex is InvalidOperationException)
                {
                    TempData["Message"] = ex.Message;  // Exibe a mensagem amigável
                }
                else
                {
                    TempData["Message"] = "Ocorreu um erro ao deletar o post. Tente novamente.";
                }

                return Redirect(Request.Headers["Referer"].ToString());  // Redireciona para a página anterior
            }
        }
    }
}
