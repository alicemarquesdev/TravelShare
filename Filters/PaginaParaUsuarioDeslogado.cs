using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TravelShare.Filters
{
    // Filtro de ação que verifica se o usuário está logado.
    // Caso o usuário esteja logado, o acesso a página é autorizado caso contrário o usuário é redirecionado para página inicial.
    public class PaginaParaUsuarioDeslogado : ActionFilterAttribute
    {
        // Método que é executado antes da ação ser processada.
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Recupera o valor da sessão do usuário logado. O valor é uma string que indica o estado da sessão.
            string sessaoDoUsuario = context.HttpContext.Session.GetString("sessaoUsuarioLogado");

            // Se a sessão do usuário não for nula ou vazia (significa que o usuário está logado),
            // então o usuário será redirecionado para a página inicial (controller "Home", action "Index").
            if (!string.IsNullOrEmpty(sessaoDoUsuario))
            {
                // Define o resultado da ação como um redirecionamento para a página inicial.
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "Index" }
                });
            }

            // Chama a implementação base para garantir que outros filtros possam ser executados.
            base.OnActionExecuting(context);
        }
    }
}
