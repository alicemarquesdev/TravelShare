using Microsoft.AspNetCore.Mvc;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICaminhoImagem _caminhoImagem;
        private readonly ISessao _sessao;

        public PostsController(IPostRepository postRepository, IUsuarioRepository usuarioRepository, ISessao sessao, ICaminhoImagem caminhoImagem)
        {
            _postRepository = postRepository;
            _usuarioRepository = usuarioRepository;
            _caminhoImagem = caminhoImagem;
            _sessao = sessao;
        }

        public IActionResult CriarPost()
        {
            // Recupera o usuário da sessão
            var usuario = _sessao.BuscarSessaoDoUsuario();

            if (usuario == null)
            {
                // Se não encontrar o usuário na sessão, redireciona para o login
                return RedirectToAction("Login", "Login");
            }

            // Caso encontre o usuário na sessão, passa o ID para a View
            ViewBag.UsuarioId = usuario.Id;

            return View();
        }

        public async Task<IActionResult> EditarPost(string id)
        {
            // Recupera o usuário da sessão
            var usuario = _sessao.BuscarSessaoDoUsuario();

            if (usuario == null)
            {
                // Se não encontrar o usuário na sessão, redireciona para o login
                return RedirectToAction("Login", "Login");
            }

            // Caso encontre o usuário na sessão, passa o ID para a View
            ViewBag.UsuarioId = usuario.Id;

            var post = await _postRepository.BuscarPostPorIdAsync(id);

            post.FotoPost = post.FotoPost ?? new List<string>();

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> CriarPost(PostsModel post, IList<IFormFile> FotoPost)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                TempData["Message"] = "Erro ao criar post: " + string.Join("; ", errors);
                return View(post);
            }
            if (ModelState.IsValid)
            {
                // Lista para armazenar os caminhos das imagens
                var caminhosImagens = new List<string>();

                // Processa cada imagem da lista e gera o caminho
                foreach (var imagem in FotoPost)
                {
                    var caminhoImagem = await _caminhoImagem.GerarCaminhoImagemAsync(imagem);
                    caminhosImagens.Add(caminhoImagem);
                }

                // Adiciona os caminhos das imagens à propriedade FotoPost
                post.FotoPost = caminhosImagens;

                // Chama o repositório para adicionar o post
                await _postRepository.AddPostAsync(post);
                TempData["Message"] = "Post publicado com sucesso!";
                return View();
            }
            TempData["Message"] = "Verifique os dados inseridos, não foi possível publicar o post.";
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> EditarPost(PostsModel post, IList<IFormFile> FotoPost)
        {
            if (ModelState.IsValid)
            {
                // Recuperar imagens existentes (caso necessário)
                var postExistente = await _postRepository.BuscarPostPorIdAsync(post.Id);

                if (postExistente != null)
                {
                    post.FotoPost = postExistente?.FotoPost ?? new List<string>();

                    // Processar novas imagens
                    if (FotoPost != null && FotoPost.Any())
                    {
                        foreach (var file in FotoPost)
                        {
                            if (file.Length > 0)
                            {
                                var caminhoImagem = await _caminhoImagem.GerarCaminhoImagemAsync(file);
                                post.FotoPost.Add(caminhoImagem);
                            }
                        }
                    }

                    // Atualizar o post
                    await _postRepository.AtualizarPostAsync(post);
                    TempData["Message"] = "Post atualizado com sucesso";
                    return View();
                }
                TempData["Message"] = "Post não encontrado.";
                return View(post);
            }
            TempData["Message"] = "Não conseguimos atualizar os dados do post, verifique os dados inseridos.";
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> DeletarPost(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Message"] = "O ID do post não foi fornecido.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            // Verifica se o post existe
            var postExistente = await _postRepository.BuscarPostPorIdAsync(id);
            if (postExistente == null)
            {
                TempData["Message"] = "Post não encontrado.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            // Deletar o post
            var sucesso = await _postRepository.DeletarPostAsync(id);
            if (sucesso)
            {
                TempData["Message"] = "Post deletado com sucesso.";
            }
            else
            {
                TempData["Message"] = "Não foi possível deletar o post.";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public string CalcularTempoPassado(DateTime dataCriacao)
        {
            var agora = DateTime.UtcNow;
            var diferenca = agora - dataCriacao;

            if (diferenca.TotalDays >= 1)
            {
                return $"{diferenca.Days} dia{(diferenca.Days > 1 ? "s" : "")} atrás";
            }
            else if (diferenca.TotalHours >= 1)
            {
                return $"{diferenca.Hours} hora{(diferenca.Hours > 1 ? "s" : "")} atrás";
            }
            else if (diferenca.TotalMinutes >= 1)
            {
                return $"{diferenca.Minutes} minuto{(diferenca.Minutes > 1 ? "s" : "")} atrás";
            }
            else
            {
                return "Agora mesmo";
            }
        }
    }
}