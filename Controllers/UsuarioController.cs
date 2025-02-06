using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPostRepository _postRepository;
        private readonly IComentarioRepository _comentarioRepository;
        private readonly ISessao _sessao;
        private readonly ICaminhoImagem _caminhoImagem;
        private readonly ICurtidaRepository _curtidaRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository, ISessao sessao, IPostRepository postRepository,
                                    ICaminhoImagem caminhoImagem, IComentarioRepository comentarioRepository,
                                     ICurtidaRepository curtidaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _comentarioRepository = comentarioRepository;
            _sessao = sessao;
            _postRepository = postRepository;
            _caminhoImagem = caminhoImagem;
            _curtidaRepository = curtidaRepository;
        }

        public async Task<IActionResult> Perfil(string id)
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            if (usuario == null || usuarioLogado == null)
            {
                return RedirectToAction("Login", "Login");
            }

            bool perfilLogado = usuarioLogado == usuario;

            ViewBag.PerfilLogado = perfilLogado;
            ViewBag.UsuarioLogado = usuarioLogado.Id.ToString();

            ViewBag.PostsDoUsuario = await _postRepository.BuscarTodosOsPostsDoUsuarioAsync(usuario.Id);
            ViewBag.PaisesVisitados = usuario.PaisesVisitados.ToList();

            return View(usuario);
        }

        public async Task<IActionResult> EditarPerfil(string id)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            ViewBag.Senha = usuario.Senha;
            return View(usuario);
        }

        public async Task<IActionResult> Seguidores(string id)
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            if (usuario?.Seguidores == null || usuario.Seguidores.Count == 0)
            {
                return View(new List<UsuarioModel>());
            }

            ViewBag.UsuarioId = usuario.Id.ToString();
            ViewBag.UsuarioLogadoId = usuarioLogado.Id;

            var seguidores = await _usuarioRepository.BuscarVariosUsuariosPorIdAsync(usuario.Seguidores);

            return View(seguidores);
        }

        public async Task<IActionResult> Seguindo(string id)
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            if (usuario?.Seguindo == null || usuario.Seguindo.Count == 0)
            {
                return View(new List<UsuarioModel>());
            }

            ViewBag.UsuarioId = usuario.Id.ToString();
            ViewBag.UsuarioLogadoId = usuarioLogado.Id;

            var seguindo = await _usuarioRepository.BuscarVariosUsuariosPorIdAsync(usuario.Seguindo);

            return View(seguindo);
        }

        [HttpPost]
        public async Task<IActionResult> EditarPerfil(UsuarioModel usuario, IFormFile? foto)
        {
            try
            {
                var usuarioDb = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuario.Id);

                if (usuario != null)
                {
                    usuario.Senha = usuarioDb.Senha; // Mantenha a senha do banco
                    ModelState.Remove("Senha"); // Remova a validação para o campo Senha
                    ModelState.Remove("SenhaConfirmacao"); // Remova a validação para o campo SenhaConfirmacao
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    TempData["ValidationErrors"] = errors;
                    return View(usuario);
                }

                if (ModelState.IsValid)
                {
                    // Se uma foto foi enviada, atualize a FotoPerfil
                    if (foto != null)
                    {
                        usuario.FotoPerfil = await _caminhoImagem.GerarCaminhoImagemAsync(foto);
                    }

                    // Comparar se a nova foto é diferente da armazenada
                    if (usuario.FotoPerfil == usuarioDb.FotoPerfil)
                    {
                        throw new Exception("A nova foto é igual à atual.");
                    }

                    await _usuarioRepository.AtualizarUsuarioAsync(usuario);

                    TempData["Message"] = "Os dados foram atualizados com sucesso.";
                    return RedirectToAction("EditarPerfil"); // Redireciona para a página de edição do perfil, sem precisar passar o id
                }

                TempData["Message"] = "Houve um erro nos dados inseridos.";
                return View(usuario); // Retorna a view com o modelo que contém os erros
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Erro ao atualizar perfil: {ex.Message}";
                return View(usuario);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletarConta()
        {
            var usuario = _sessao.BuscarSessaoDoUsuario();

            if (usuario == null)
            {
                TempData["Message"] = "Usuário não encontrado.";
                return RedirectToAction("EditarPerfil");
            }

            // Adicionar deletação de post e comentários e todas as collections relacionadas
            await _usuarioRepository.DeletarUsuarioAsync(usuario.Id);
            TempData["Message"] = "Conta excluída com sucesso.";

            return RedirectToAction("Login", "Login");
        }
    }
}