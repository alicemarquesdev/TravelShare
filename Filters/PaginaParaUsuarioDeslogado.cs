using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TravelShare.Filters
{
    public class PaginaParaUsuarioDeslogado : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string sessaoDoUsuario = context.HttpContext.Session.GetString("sessaoUsuarioLogado");

            if (!string.IsNullOrEmpty(sessaoDoUsuario))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
            }

            base.OnActionExecuting(context);
        }
    }
}