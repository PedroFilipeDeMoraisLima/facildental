using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace gestaoclinica.Filtros
{
    public class AutorizacaoFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string Controller = Convert.ToString(HttpContext.Current.Request.RequestContext.RouteData.Values["Controller"]);

            object UsuarioLogado = filterContext.HttpContext.Session["UsuarioLogado"];

            if (UsuarioLogado == null && Controller != "Login" && Controller != "Clinica")
            {
                filterContext.Result = new RedirectToRouteResult(
                          new RouteValueDictionary(
                              new { action = "Index", controller = "Login" }));
            }
        }
    }
}