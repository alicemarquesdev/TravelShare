using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using TravelShare.Models;

namespace TravelShare.Filters
{
    // Filtro de ação que garante que apenas usuários logados possam acessar certas páginas.
    // Caso o usuário não esteja logado, ele é redirecionado para a página de login.
    public class PaginaParaUsuarioLogado : ActionFilterAttribute
    {
        // Método que é executado antes da ação ser processada.
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Recupera o valor da sessão do usuário logado. A sessão contém informações sobre o estado de login.
            string sessaoDoUsuario = context.HttpContext.Session.GetString("SessaoDoUsuarioLogado");

            // Verifica se o usuário não está logado (a sessão está vazia ou nula).
            if (string.IsNullOrEmpty(sessaoDoUsuario))
            {
                // Se o usuário não estiver logado, ele é redirecionado para a página de login.
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", "Login" } });
            }
            else
            {
                // Caso a sessão exista, ela é desserializada para um objeto do tipo UsuarioModel.
                // Isso transforma a string armazenada na sessão de volta para um objeto.
                UsuarioModel usuario = JsonConvert.DeserializeObject<UsuarioModel>(sessaoDoUsuario);

                // Se o objeto usuário for nulo após a desserialização (isso pode acontecer se a sessão estiver corrompida ou inválida),
                // o usuário também é redirecionado para a página de login.
                if (usuario == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", "Login" } });
                }
            }

            // Chama a implementação base para garantir que outros filtros possam ser executados.
            base.OnActionExecuting(context);
        }
    }
}
    