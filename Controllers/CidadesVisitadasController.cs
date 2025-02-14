using Microsoft.AspNetCore.Mvc;
using TravelShare.Helper;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Controllers
{
    public class CidadesVisitadasController : Controller
    {
        private readonly ICidadesVisitadasRepository _cidadesVisitadasRepository;
        private readonly ISessao _sessao;

        public CidadesVisitadasController(ICidadesVisitadasRepository cidadesVisitadasRepository, ISessao sessao)
        {
            _cidadesVisitadasRepository = cidadesVisitadasRepository;
            _sessao = sessao;
        }

        public class CidadeRequest
        {
            public string Cidade { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> AddCidade([FromBody] CidadeRequest request)
        {
            var usuario = _sessao.BuscarSessaoDoUsuario();

            if (string.IsNullOrEmpty(request.Cidade) || usuario == null)
            {
                return Json(new { success = false, message = "Cidade inválida." });
            }

            var cidadeExistente = await _cidadesVisitadasRepository.VerificarCidadeVisitadaPeloUsuarioAsync(usuario.Id, request.Cidade);

            if (cidadeExistente)
            {
                return Json(new { success = false, message = "A cidade já está na lista." });
            }

            var sucesso = await _cidadesVisitadasRepository.AddCidadeAsync(usuario.Id, request.Cidade);

            return Json(new { success = sucesso });
        }

        [HttpPost]
        public async Task<IActionResult> RemoverCidade([FromBody] CidadeRequest request)
        {
            var usuario = _sessao.BuscarSessaoDoUsuario();

            if (string.IsNullOrEmpty(request.Cidade) || usuario == null)
            {
                return Json(new { success = false, message = "Cidade inválida." });
            }

            var sucesso = await _cidadesVisitadasRepository.RemoveCidadeAsync(usuario.Id, request.Cidade);

            return Json(new { success = sucesso });
        }


    }
}