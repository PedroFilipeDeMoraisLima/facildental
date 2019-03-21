using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gestaoclinica.Models;

namespace gestaoclinica.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session["UsuarioLogado"] = null;

            return RedirectToAction("Index", "Login");
        }

        public ActionResult Autenticar(string Email, string Senha)
        {
            Usuario UsuarioAutenticado = new Usuario();

            UsuarioAutenticado = UsuarioAutenticado.Autenticar(Email, Senha); 

            if (UsuarioAutenticado.Codigo != 0)
            {
                Session["UsuarioLogado"] = UsuarioAutenticado;
                Session["CodigoClinica"] = UsuarioAutenticado.Clinica.Codigo;

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["MensagemValidacao"] = "Usuário e/ou senha incorretos.";

                return RedirectToAction("Index", "Login");
            }
        }

    }
}
