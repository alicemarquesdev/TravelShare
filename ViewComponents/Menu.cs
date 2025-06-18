using Microsoft.AspNetCore.Mvc;
using TravelShare.Helper.Interfaces;

namespace TravelShare.ViewComponents
{
    // ViewComponent responsável por renderizar o menu
    public class Menu : ViewComponent
    {
        // Dependência de ISessao para obter informações sobre a sessão do usuário
        private readonly ISessao _sessao;

        // Construtor que injeta a dependência do serviço ISessao
        public Menu(ISessao sessao)
        {
            _sessao = sessao;
        }

        // Método responsável por renderizar o componente de menu de forma assíncrona
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Busca a sessão do usuário, retornando informações relacionadas ao usuário logado
            var usuario = _sessao.BuscarSessaoDoUsuario();

            // Retorna a view do componente, passando as informações do usuário (caso exista) para a visualização
            return View(usuario);
        }
    }
}
