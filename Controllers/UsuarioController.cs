using Microsoft.AspNetCore.Mvc;
using TravelShare.Filters;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    [PaginaParaUsuarioLogado]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICaminhoImagem _caminhoImagem;
        private readonly ISessao _sessao;

        public UsuarioController(IUsuarioRepository usuarioRepository, IPostRepository postRepository,
                                 ICaminhoImagem caminhoImagem, ISessao sessao)
        {
            _usuarioRepository = usuarioRepository;
            _postRepository = postRepository;
            _caminhoImagem = caminhoImagem;
            _sessao = sessao;
        }

     
        public async Task<IActionResult> Perfil(string id)
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            var usuarioPerfil = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            bool perfilDoUsuarioLogado = usuarioLogado == usuarioPerfil;

            ViewBag.PerfilDoUsuarioLogado = perfilDoUsuarioLogado;
            ViewBag.UsuarioLogadoId = usuarioLogado.Id;
            ViewBag.PostsDoUsuario = await _postRepository.BuscarTodosOsPostsDoUsuarioAsync(usuarioPerfil.Id);

            return View(usuarioPerfil);
        }

        public async Task<IActionResult> EditarPerfil(string id)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            if (usuario == null)
            {
                TempData["Message"] = "Usuário não encontrado.";
                return RedirectToAction("Index", "Home");
            }

            var usuarioSemSenha = new UsuarioSemSenhaModel
            {
                Id =  usuario.Id,
                Nome = usuario.Nome,
                Username = usuario.Username,
                Email = usuario.Email,
                DataNascimento = usuario.DataNascimento,
                CidadeDeNascimento = usuario.CidadeDeNascimento,
                Bio = usuario.Bio,
                ImagemPerfil = usuario.ImagemPerfil
            };

            ViewBag.CidadesVisitadas = usuario.CidadesVisitadas ?? new List<string>();

            return View(usuarioSemSenha);
        }


        public async Task<IActionResult> Seguidores(string id)
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            var usuarioPerfil = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            if (usuarioPerfil?.Seguidores == null || usuarioPerfil.Seguidores.Count == 0)
            {
                return View(new List<UsuarioModel>());
            }

            ViewBag.UsuarioPerfil = usuarioPerfil;
            ViewBag.UsuarioLogadoId = usuarioLogado.Id;

            var seguidores = await _usuarioRepository.BuscarVariosUsuariosPorIdAsync(usuarioPerfil.Seguidores);

            return View(seguidores);
        }

        public async Task<IActionResult> Seguindo(string id)
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            var usuarioPerfil = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            if (usuarioPerfil?.Seguindo == null || usuarioPerfil.Seguindo.Count == 0)
            {
                return View(new List<UsuarioModel>());
            }

            ViewBag.UsuarioPerfil = usuarioPerfil;
            ViewBag.UsuarioLogado = usuarioLogado;

            var seguindo = await _usuarioRepository.BuscarVariosUsuariosPorIdAsync(usuarioPerfil.Seguindo);

            return View(seguindo);
        }

        [HttpPost]
        public async Task<IActionResult> EditarPerfil(UsuarioSemSenhaModel usuario, IFormFile? imagem)
        {
            try
            {
                var usuarioDb = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuario.Id);

                if (usuarioDb == null)
                {
                    TempData["Message"] = "Usuário não encontrado.";
                    return RedirectToAction("Index", "Home");
                }
                if (!ModelState.IsValid)
                {
                    var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    foreach (var erro in erros)
                    {
                        Console.WriteLine(erro);
                    }
                    return View(usuario);  // Retorna para a mesma view com os erros
                }
                if (ModelState.IsValid)
                {
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

                    // Se uma foto foi enviada, atualize a ImagemPerfil
                    if (imagem != null)
                    {
                        var caminhoImagem = await _caminhoImagem.GerarCaminhoImagemAsync(imagem);
                        if (!string.IsNullOrEmpty(caminhoImagem))
                        {
                            usuario.ImagemPerfil = caminhoImagem;
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
                    }
                    else
                    {
                        TempData["Message"] = "Nenhum dado alterado.";
                        return RedirectToAction("EditarPerfil", new { id = usuario.Id });

                    }
                    return RedirectToAction("Perfil", new { id = usuario.Id });
                }
               

                TempData["Message"] = "Houve um erro nos dados inseridos.";
                return View(usuario); // Retorna a view com o modelo que contém os erros
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"{ex.Message}";
                return View(usuario);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletarConta()
        {
            var usuario = _sessao.BuscarSessaoDoUsuario();

            await _usuarioRepository.DeletarUsuarioAsync(usuario.Id);
            TempData["Message"] = "Conta excluída com sucesso.";

            return RedirectToAction("Login", "Login");
        }


     

    }
}