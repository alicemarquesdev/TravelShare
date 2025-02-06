using Microsoft.AspNetCore.Mvc;
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
            ViewBag.PaisesVisitados = usuario.CidadesVisitadas.ToList();

            return View(usuario);
        }

        public async Task<IActionResult> EditarPerfil(string id)
        {
            // Buscar o usuário pelo ID
            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            if (usuario == null)
            {
                TempData["Message"] = "Usuário não encontrado.";
                return RedirectToAction("Index"); // Ou para uma página de erro
            }

            // Mapear os dados de UsuarioModel para UsuarioSemSenhaModel
            var usuarioSemSenha = new UsuarioSemSenhaModel
            {
                Nome = usuario.Nome,
                Username = usuario.Username,
                Email = usuario.Email,
                DataNascimento = usuario.DataNascimento,
                CidadeDeNascimento = usuario.CidadeDeNascimento,
                Bio = usuario.Bio,
                CidadesVisitadas = usuario.CidadesVisitadas,
                Seguindo = usuario.Seguindo,
                Seguidores = usuario.Seguidores,
                FotoPerfil = usuario.FotoPerfil // Mapeando a FotoPerfil também
            };

            // Passar o usuarioSemSenha para a View
            return View(usuarioSemSenha);
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
        public async Task<IActionResult> EditarPerfil(UsuarioSemSenhaModel usuario, IFormFile? foto)
        {
            try
            {
                var usuarioDb = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuario.Id);

                if (usuario == null)
                {
                    return RedirectToAction("Login", "Login");
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

        // METODOS ADICIONAR E REMOVER CIDADE

        [HttpPost]
        public async Task<IActionResult> AddCidadeVisitada(string usuarioId, List<string> cidadesVisitadas)
        {
            // Buscar o usuário pelo ID
            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuarioId);

            if (usuario == null)
            {
                TempData["Message"] = "Usuário não encontrado.";
                return RedirectToAction("EditarPerfil");
            }

            // Mapear os dados do UsuarioModel para UsuarioSemSenhaModel
            var usuarioSemSenha = new UsuarioSemSenhaModel
            {
                // Aqui você mapeia os campos necessários, como CidadesVisitadas
                CidadesVisitadas = cidadesVisitadas
            };

            // Atualizar o usuário no repositório com o UsuarioSemSenhaModel
            await _usuarioRepository.AtualizarUsuarioAsync(usuarioSemSenha);

            return Ok(new { mensagem = "Cidades visitadas atualizadas com sucesso!" });
        }
    }
}