using Microsoft.AspNetCore.Mvc;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    public class CurtidaController : Controller
    {
        private readonly ICurtidaRepository _curtidaRepository;

        public CurtidaController(ICurtidaRepository curtidaRepository)
        {
            _curtidaRepository = curtidaRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CurtirEDescurtir(string postId, string usuarioId)
        {
            if (string.IsNullOrEmpty(postId) || string.IsNullOrEmpty(usuarioId))
            {
                TempData["Message"] = "Não foi possível curtir o post.";
            }
            else
            {
                var curtidaExistente = await _curtidaRepository.BuscarCurtidaExistenteAsync(postId, usuarioId);

                if (curtidaExistente)
                {
                    await _curtidaRepository.RemoveCurtidaAsync(postId, usuarioId);
                }
                else
                {
                    await _curtidaRepository.AddCurtidaAsync(postId, usuarioId);
                }
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}