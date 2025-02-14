using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TravelShare.Filters;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    [PaginaParaUsuarioLogado]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICaminhoImagem _caminhoImagem;
        private readonly ISessao _sessao;

        public PostController(IPostRepository postRepository, IUsuarioRepository usuarioRepository, ISessao sessao, ICaminhoImagem caminhoImagem)
        {
            _postRepository = postRepository;
            _usuarioRepository = usuarioRepository;
            _caminhoImagem = caminhoImagem;
            _sessao = sessao;
        }

        public IActionResult CriarPost()
        {
            var usuario = _sessao.BuscarSessaoDoUsuario();

            ViewBag.UsuarioLogadoId = usuario.Id;

            return View();
        }

        public async Task<IActionResult> EditarPost(string id)
        {
            var post = await _postRepository.BuscarPostPorIdAsync(id);

            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(post.UsuarioId);

            ViewBag.UsuarioLogadoId = usuario.Id;

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> CriarPost(PostModel post, IList<IFormFile> ImagemPost)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Lista para armazenar os caminhos das imagens
                    var caminhosImagens = new List<string>();

                    // Processa cada imagem da lista e gera o caminho
                    foreach (var imagem in ImagemPost)
                    {
                        try
                        {
                            var caminhoImagem = await _caminhoImagem.GerarCaminhoImagemAsync(imagem);
                            if (caminhoImagem == null)
                            {
                                TempData["Message"] = "Ocorreu um erro ao salvar a imagem.";
                                return View(post);
                            }
                            caminhosImagens.Add(caminhoImagem);
                        }
                        catch (Exception ex)
                        {
                            TempData["Message"] = "Erro ao processar a imagem: " + ex.Message;
                            return View(post);
                        }
                    }

                    // Adiciona os caminhos das imagens à propriedade ImagemPost
                    post.ImagemPost = caminhosImagens;
                    post.Id = ObjectId.GenerateNewId().ToString();

                    // Chama o repositório para adicionar o post
                    await _postRepository.AddPostAsync(post);

                    TempData["Message"] = "Post publicado com sucesso!";
                    return RedirectToAction("Perfil", "Usuario", new { id = post.UsuarioId });
                }
                TempData["Message"] = "Verifique os dados inseridos, não foi possível publicar o post.";
                return View(post);
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return View(post);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarPost(PostModel post)
        {
            if (ModelState.IsValid)
            {
                // Recuperar imagens existentes (caso necessário)
                var postExistente = await _postRepository.BuscarPostPorIdAsync(post.Id);

                if (postExistente != null)
                {
                    // Atualizar o post
                    await _postRepository.AtualizarPostAsync(post);
                    TempData["Message"] = "Post atualizado com sucesso";
                    return RedirectToAction("EditarPost", new { id = post.Id });
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
                return RedirectToAction("Perfil", "Usuario", new { id = postExistente.Usuario.Id });
            }
            else
            {
                TempData["Message"] = "Não foi possível deletar o post.";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}