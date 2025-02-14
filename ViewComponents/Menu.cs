using Microsoft.AspNetCore.Mvc;
using TravelShare.Helper;

namespace TravelShare.ViewComponents
{
    public class Menu : ViewComponent
    {
        private readonly ISessao _sessao;

        public Menu(ISessao sessao)
        {
            _sessao = sessao;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var usuario = _sessao.BuscarSessaoDoUsuario();
            return View(usuario);
        }
    }
}