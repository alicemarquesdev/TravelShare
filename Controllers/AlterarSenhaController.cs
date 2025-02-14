using Microsoft.AspNetCore.Mvc;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    public class AlterarSenhaController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAlterarSenhaRepository _alterarSenhaRepository;

        public AlterarSenhaController(IUsuarioRepository usuarioRepository, IAlterarSenhaRepository alterarSenhaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _alterarSenhaRepository = alterarSenhaRepository;
        }

        public async Task<IActionResult> AlterarSenha(string id)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            if (usuario == null)
            {
                TempData["Message"] = "Usuário não encontrado.";
                return RedirectToAction("Perfil", "Usuario");
            }

            ViewBag.UsuarioId = id;

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> AlterarSenha(AlterarSenhaModel alterarSenha)
        {
            var usuarioDb = await _usuarioRepository.BuscarUsuarioPorIdAsync(alterarSenha.Id);

            if (usuarioDb == null)
            {
                TempData["Message"] = "Usuário não encontrado";
                return RedirectToAction("Login", "Login");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    var senhaAlterada = await _alterarSenhaRepository.AlterarSenhaAsync(alterarSenha);

                    if (senhaAlterada)
                    {
                        TempData["Message"] = "Senha alterada com sucesso!";
                    }
                    else
                    {
                        TempData["Message"] = "Não foi possível alterar a senha.";
                    }

                    return RedirectToAction("AlterarSenha", "AlterarSenha");
                }

                TempData["Message"] = "Houve um erro, verifique os dados inseridos.";
                return RedirectToAction("AlterarSenha", "AlterarSenha");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("AlterarSenha", "AlterarSenha");
            }

        }
    }
}
