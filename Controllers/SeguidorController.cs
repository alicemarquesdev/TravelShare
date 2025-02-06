using Microsoft.AspNetCore.Mvc;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    public class SeguidorController : Controller
    {
        private readonly ISeguidorRepository _seguidorRepository;

        public SeguidorController(ISeguidorRepository seguidorRepository)
        {
            _seguidorRepository = seguidorRepository;
        }

        public IActionResult Seguidores()
        {
            return View();
        }

        public IActionResult Seguindo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SeguirEDeseguir(string usuarioId, string seguindoId)
        {
            if (string.IsNullOrEmpty(usuarioId) || string.IsNullOrEmpty(seguindoId))
            {
                TempData["Message"] = "Não foi possível executar essa ação.";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var usuarioSeguindo = await _seguidorRepository.BuscarSeguindoAsync(usuarioId, seguindoId);

                if (usuarioSeguindo == null)
                {
                    await _seguidorRepository.SeguirUsuarioAsync(usuarioId, seguindoId);
                }
                else
                {
                    await _seguidorRepository.DeseguirUsuarioAsync(usuarioId, seguindoId);
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> RemoverSeguidor(string usuarioId, string seguidorId)
        {
            if (string.IsNullOrEmpty(usuarioId) || string.IsNullOrEmpty(seguidorId))
            {
                TempData["Message"] = "Não foi possível executar essa ação.";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                var seguidor = await _seguidorRepository.BuscarSeguidorAsync(usuarioId, seguidorId);

                if (seguidor != null)
                {
                    await _seguidorRepository.RemoverSeguidorAsync(usuarioId, seguidorId);
                }
                else
                {
                    TempData["Message"] = "Não foi possível remover o seguidor.";
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}